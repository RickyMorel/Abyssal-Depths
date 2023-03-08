//#define DISTANCE_DEBUGS

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

#region Helper Classes

public class SubGoal
{
    public Dictionary<string, int> Sgoals;
    public bool Remove;

    public SubGoal(string key, int value, bool remove)
    {
        Sgoals = new Dictionary<string, int>();
        Sgoals.Add(key, value);
        Remove = remove;
    } 
}

#endregion

[RequireComponent(typeof(AIInteractionController))]
[RequireComponent(typeof(AIStateMachine))]
public class GAgent : MonoBehaviour
{
    #region Public Properties

    public List<GAction> Actions = new List<GAction>();
    public Dictionary<SubGoal, int> Goals = new Dictionary<SubGoal, int>();
    public GInventory Inventory = new GInventory();
    public WorldStates Beliefs = new WorldStates();
    public GAction CurrentAction;

    public bool IsMoving => _isMoving;

    public event Action OnDoAction;
    public event Action OnExitAction;

    #endregion

    #region Editor Fields

    [SerializeField] private float _goalDistance = 2f;

    #endregion

    #region Private Variables

    private GPlanner _planner;
    private Queue<GAction> _actionQueue;
    private SubGoal _currentGoal;

    private Vector3 _destination = Vector3.zero;
    private bool _invoked = false;
    private bool _isMoving = false;

    private Rigidbody _rb;
    private AIStateMachine _aiStateMachine;
    private AIInteractionController _interactionController;
    private float _initialGoalDistance;

    #endregion

    #region Unity Loops

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _aiStateMachine = GetComponent<AIStateMachine>();
        _interactionController = GetComponent<AIInteractionController>();

        _initialGoalDistance = _goalDistance;
    }
   
    public virtual void Start()
    {
        GAction[] acts = GetComponents<GAction>();

        foreach (GAction act in acts)
        {
            Actions.Add(act);
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = Vector3.zero;
    }


    private void LateUpdate()
    {
        if (!_aiStateMachine.CanMove) { _isMoving = false; _aiStateMachine.Agent.enabled = false; }

        else if(!_interactionController.IsInteracting()) { _aiStateMachine.Agent.enabled = true; }

        if (CurrentAction != null && CurrentAction.IsRunning)
        {
#if DISTANCE_DEBUGS
            Debug.Log(CurrentAction.ActionName + " is Running");
#endif
            //if target moved, update desitination
            if (CurrentAction.Target != null && CurrentAction.Target.transform.position != _destination) 
            { _destination = CurrentAction.Target.transform.position; }

            if (CurrentAction.Agent.isOnNavMesh) { CurrentAction.Agent.SetDestination(_destination); }

            float distanceToTarget = Vector3.Distance(_destination, transform.position);
#if DISTANCE_DEBUGS
            Debug.Log(CurrentAction.ActionName + $"{distanceToTarget} < {_goalDistance}?");
#endif
            if (distanceToTarget < _goalDistance)
            {
                if (CurrentAction.Agent.isOnNavMesh) { CurrentAction.Agent.ResetPath(); }

                _isMoving = false;

                if (!_invoked)
                {
                    Invoke(nameof(CompleteAction), CurrentAction.Duration);
                    _invoked = true;
                    OnDoAction?.Invoke();
                }
            }
            return;
        }

        CalculateCurrentGoal();

        CheckIfRemoveGoal();

        TryPerformGoal();
    }

    public void SetGoalDistance(float newGoalDistance)
    {
        _goalDistance = newGoalDistance;
    }

    public void ResetGoalDistance()
    {
        _goalDistance = _initialGoalDistance;
    }

    private void TryPerformGoal()
    {
        if (_actionQueue == null || _actionQueue.Count < 1) { return; }

        CurrentAction = _actionQueue.Dequeue();
        if (CurrentAction.PrePerform())
        {
            if (CurrentAction.Target == null && CurrentAction.TargetTag != "")
                CurrentAction.Target = GameObject.FindWithTag(CurrentAction.TargetTag);

            if (CurrentAction.Target != null)
            {
                CurrentAction.IsRunning = true;

                _destination = CurrentAction.Target.transform.position;
                Transform dest = CurrentAction.Target.transform.Find("Destination");
                if (dest != null) { _destination = dest.position; }

                CurrentAction.Agent.SetDestination(_destination);

                if (_aiStateMachine.CanMove) { _isMoving = true; }
            }
        }
        else
        {
            _actionQueue = null;
        }
    }

    private void CheckIfRemoveGoal()
    {
        if (_actionQueue != null && _actionQueue.Count == 0)
        {
            if (_currentGoal.Remove)
            {
                Goals.Remove(_currentGoal);
            }
            _planner = null;
        }
    }

    private void CalculateCurrentGoal()
    {
        if (_planner != null && _actionQueue != null) { return; }

        _planner = new GPlanner();

        var sortedGoals = from entry in Goals orderby entry.Value descending select entry;

        foreach (KeyValuePair<SubGoal, int> subGoal in sortedGoals)
        {
            _actionQueue = _planner.Plan(Actions, subGoal.Key.Sgoals, Beliefs);
            if (_actionQueue != null)
            {
                _currentGoal = subGoal.Key;
                break;
            }
        }
    }

    #endregion

    public void CancelPreviousActions()
    {
        CurrentAction?.PostPeform();
        CurrentAction = null;
        CancelInvoke(nameof(CompleteAction));
        _invoked = false;
    }

    private void CompleteAction()
    {
        CurrentAction.IsRunning = false;
        CurrentAction.PostPeform();
        _invoked = false;

        OnExitAction?.Invoke();
    }

}

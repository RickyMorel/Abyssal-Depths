using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class GAction : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private WorldState[] _preConditionsInspector;
    [SerializeField] private WorldState[] _effectsInspector;

    #endregion

    #region Private Variables

    private GAgent _gAgent;
    private NavMeshAgent _agent;

    #endregion

    #region Public Properties

    public string ActionName = "Action";
    public float Cost = 1f;
    public GameObject Target;
    public string TargetTag;
    public float Duration;

    public Dictionary<string, int> Preconditions;
    public Dictionary<string, int> Effects;

    public WorldStates AgentBeliefs;
    public GInventory Inventory;
    public WorldStates Beliefs;
    public bool IsRunning = false;

    public GAgent GAgent => _gAgent;
    public NavMeshAgent Agent => _agent;

    #endregion


    public GAction()
    {
        Preconditions = new Dictionary<string, int>();
        Effects = new Dictionary<string, int>();
    }

    #region Unity Loops

    private void Awake()
    {
        _gAgent = GetComponent<GAgent>();
        _agent = GetComponent<NavMeshAgent>();
        Inventory = _gAgent.Inventory;
        Beliefs = _gAgent.Beliefs;

        LoadPreConditions();
        LoadEffects();
    }

    public virtual void Start()
    {
        //For Child Classes
    }

    #endregion

    private void LoadPreConditions()
    {
        if (_preConditionsInspector == null) { return; }

        foreach (WorldState worldState in _preConditionsInspector)
        {
            Preconditions.Add(worldState.key, worldState.value);
        }
    }

    private void LoadEffects()
    {
        if (_effectsInspector == null) { return; }

        foreach (WorldState worldState in _effectsInspector)
        {
            Effects.Add(worldState.key, worldState.value);
        }
    }

    public bool IsAchievable()
    {
        return true;
    }

    public bool IsAchievableGiven(Dictionary<string, int> conditions)
    {
        foreach (KeyValuePair<string, int> condition in Preconditions)
        {
            if (!conditions.ContainsKey(condition.Key))
                return false;
        }

        return true;
    }

    public abstract bool PrePerform();
    public virtual bool Perform()
    {
        //Interactable interactable = GAgent.InteractionController.CurrentInteractable;
        Interactable interactable = null;

        if(interactable == null) { Debug.Log("GOT ITEM FROM OBJECT REFERENCE!"); interactable = Target.GetComponent<Interactable>(); }

        if (interactable != null)
        {
            Debug.Log("Interacted with item!");
            GAgent.InteractionController.SetCurrentInteractable(interactable);

            GAgent.InteractionController.HandleInteraction(Duration);
        }

        return true;
    }
    public abstract bool PostPeform();
}

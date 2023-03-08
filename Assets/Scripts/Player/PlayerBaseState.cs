
using UnityEngine;

[System.Serializable]
public abstract class PlayerBaseState
{
    protected bool _isRootState = false;
    protected BaseStateMachine _context;
    protected PlayerStateFactory _factory;
    protected PlayerBaseState _currentSuperState;
    protected PlayerBaseState _currentSubState;

    public PlayerBaseState(BaseStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        _context = currentContext;
        _factory = playerStateFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubStates();

    public void UpdateStates() 
    {
        UpdateState();
        if(_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        //current state exits state
        ExitState();

        //new state enters state
        newState.EnterState();

        if (_isRootState)
        {
            //switch current state of context
            _context.CurrentState = newState;
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
        }
    }
    protected void SetSuperState(PlayerBaseState newSuperState) 
    {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(PlayerBaseState newSubState) 
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}

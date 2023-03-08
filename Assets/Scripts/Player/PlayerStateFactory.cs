
public class PlayerStateFactory
{
    private BaseStateMachine _context;

    public PlayerStateFactory(BaseStateMachine currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState Idle() { return new PlayerIdleState(_context, this); }
    public PlayerBaseState Walk() { return new PlayerWalkState(_context, this); }
    public PlayerBaseState Run() { return new PlayerRunState(_context, this); }
    public PlayerBaseState Jump() { return new PlayerJumpState(_context, this); }
    public PlayerBaseState Fall() { return new PlayerFallState(_context, this); }
    public PlayerBaseState Grounded() { return new PlayerGroundedState(_context, this); }
    public PlayerBaseState Ragdoll() { return new PlayerRagdollState(_context, this); }
    public PlayerBaseState Attack() { return new PlayerAttackState(_context, this); }
    public PlayerBaseState Carry() { return new PlayerCarryState(_context, this); }
}

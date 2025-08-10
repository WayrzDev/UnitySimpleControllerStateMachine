using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerStateMachine stateMachine, PlayerMovement controller) 
        : base(stateMachine, controller) { }

    public override void Enter()
    {
        Controller.Jump();
    }

    public override void PhysicsUpdate()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"), 
            Input.GetAxisRaw("Vertical")
        );
        Controller.Move(input);
    }

    public override void LogicUpdate()
    {
        if (Controller.Velocity.y <= 0)
        {
            StateMachine.TransitionTo(Controller.InAirState);
        }
    }
}
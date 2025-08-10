using UnityEngine;

public class InAirState : PlayerState
{
    public InAirState(PlayerStateMachine stateMachine, PlayerMovement controller) 
        : base(stateMachine, controller) { }

    public override void PhysicsUpdate()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"), 
            Input.GetAxisRaw("Vertical")
        );
        Controller.Move(input, Controller.AirControlMultiplier);
    }

    public override void LogicUpdate()
    {
        if (Controller.IsGrounded)
        {
            Vector2 input = new Vector2(
                Input.GetAxisRaw("Horizontal"), 
                Input.GetAxisRaw("Vertical")
            );
            
            if (input.sqrMagnitude > 0.01f)
            {
                CheckMovementInputTransition(input);
            }
            else
            {
                StateMachine.TransitionTo(Controller.IdleState);
            }
        }
    }

    public override void Exit()
    {
        Controller.Velocity = new Vector3(
            Controller.Velocity.x,
            0f,
            Controller.Velocity.z
        );
    }
}
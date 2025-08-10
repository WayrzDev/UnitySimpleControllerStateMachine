using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerStateMachine stateMachine, PlayerMovement controller) 
        : base(stateMachine, controller) { }

    public override void HandleInput()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"), 
            Input.GetAxisRaw("Vertical")
        );

        if (input.sqrMagnitude > 0.01f)
        {
            CheckMovementInputTransition(input);
            return;
        }

        if (Input.GetButton("Jump") && Controller.CanJump())
        {
            StateMachine.TransitionTo(Controller.JumpState);
            return;
        }

        if (Input.GetButton("Crouch"))
        {
            StateMachine.TransitionTo(Controller.CrouchState);
        }
    }

    public override void LogicUpdate()
    {
        CheckGroundedTransition();
    }
}
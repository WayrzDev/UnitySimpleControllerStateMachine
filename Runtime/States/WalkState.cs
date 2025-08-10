using UnityEngine;

public class WalkState : PlayerState
{
    public WalkState(PlayerStateMachine stateMachine, PlayerMovement controller) 
        : base(stateMachine, controller) { }

    public override void Enter()
    {
        Controller.SetMoveSpeed(Controller.WalkSpeed);
    }

    public override void HandleInput()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"), 
            Input.GetAxisRaw("Vertical")
        );

        if (Input.GetButton("Jump") && Controller.CanJump())
        {
            StateMachine.TransitionTo(Controller.JumpState);
            return;
        }

        if (Input.GetButton("Crouch"))
        {
            StateMachine.TransitionTo(Controller.CrouchState);
            return;
        }

        if (input.sqrMagnitude <= 0.01f)
        {
            StateMachine.TransitionTo(Controller.IdleState);
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            StateMachine.TransitionTo(Controller.RunState);
        }
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
        CheckGroundedTransition();
    }
}
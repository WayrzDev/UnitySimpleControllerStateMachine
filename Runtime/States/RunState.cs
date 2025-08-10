using UnityEngine;

public class RunState : PlayerState
{
    public RunState(PlayerStateMachine stateMachine, PlayerMovement controller) 
        : base(stateMachine, controller) { }

    public override void Enter()
    {
        Controller.SetMoveSpeed(Controller.RunSpeed);
    }

    public override void HandleInput()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"), 
            Input.GetAxisRaw("Vertical")
        );

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            StateMachine.TransitionTo(Controller.WalkState);
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
            return;
        }

        if (input.sqrMagnitude <= 0.01f)
        {
            StateMachine.TransitionTo(Controller.IdleState);
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
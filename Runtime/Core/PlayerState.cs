using UnityEngine;

public abstract class PlayerState
{
    protected readonly PlayerStateMachine StateMachine;
    protected readonly PlayerMovement Controller;

    protected PlayerState(PlayerStateMachine stateMachine, PlayerMovement controller)
    {
        StateMachine = stateMachine;
        Controller = controller;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void HandleInput() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }

    protected void CheckGroundedTransition()
    {
        if (!Controller.IsGrounded)
        {
            StateMachine.TransitionTo(Controller.InAirState);
        }
    }

    protected void CheckMovementInputTransition(Vector2 input)
    {
        if (input.sqrMagnitude <= 0.01f)
        {
            StateMachine.TransitionTo(Controller.IdleState);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            StateMachine.TransitionTo(Controller.RunState);
        }
        else
        {
            StateMachine.TransitionTo(Controller.WalkState);
        }
    }
}
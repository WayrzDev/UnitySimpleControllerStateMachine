using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }
    private readonly PlayerMovement _controller;

    public PlayerStateMachine(PlayerMovement controller)
    {
        _controller = controller;
    }

    public void Initialize(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void TransitionTo(PlayerState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }
}
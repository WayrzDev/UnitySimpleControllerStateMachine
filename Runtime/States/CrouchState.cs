using UnityEngine;

public class CrouchState : PlayerState
{
    private float _originalHeight;
    private Vector3 _originalCenter;
    private const float CrouchHeight = 1f;

    public CrouchState(PlayerStateMachine stateMachine, PlayerMovement controller) 
        : base(stateMachine, controller) { }

    public override void Enter()
    {
        _originalHeight = Controller.CharacterController.height;
        _originalCenter = Controller.CharacterController.center;

        Controller.CharacterController.height = CrouchHeight;
        Controller.CharacterController.center = new Vector3(
            _originalCenter.x, 
            CrouchHeight / 2f, 
            _originalCenter.z
        );
        Controller.SetMoveSpeed(Controller.CrouchSpeed);
    }

    public override void HandleInput()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"), 
            Input.GetAxisRaw("Vertical")
        );

        if (!Input.GetButton("Crouch") && CanStandUp())
        {
            if (input.sqrMagnitude > 0.01f)
            {
                StateMachine.TransitionTo(Controller.WalkState);
            }
            else
            {
                StateMachine.TransitionTo(Controller.IdleState);
            }
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

    public override void Exit()
    {
        Controller.CharacterController.height = _originalHeight;
        Controller.CharacterController.center = _originalCenter;
    }

    private bool CanStandUp()
    {
        float checkDistance = (_originalHeight - CrouchHeight) * 0.9f;
        Vector3 start = Controller.transform.position + Vector3.up * CrouchHeight;
        return !Physics.Raycast(start, Vector3.up, checkDistance);
    }
}
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController CharacterController;
    public Transform Orientation;

    [Header("Movement Settings")]
    public float WalkSpeed = 4f;
    public float RunSpeed = 7f;
    public float CrouchSpeed = 2f;
    public float AirControlMultiplier = 0.5f;
    public float moveSpeed;
    public float CurrentMoveSpeed { get; private set; }

    [Header("Jump Settings")]
    public float JumpForce = 5f;
    public float JumpCooldown = 0.2f;
    public float Gravity = -9.81f;

    [Header("Slope Settings")]
    [Tooltip("Ângulo máximo de subida em graus")]
    public float maxSlopeAngle = 45f;
    [Tooltip("Força extra para evitar que o jogador escorregue em rampas")]
    public float slopeForce = 5f;
    [Tooltip("Distância extra para detecção de slopes")]
    public float slopeRayExtraDistance = 0.5f;

    private RaycastHit _slopeHit;
    private bool _isOnSlope;

    public bool IsOnSlope => _isOnSlope;
    public float CurrentSlopeAngle { get; private set; }

    [Header("Ground Check")]
    public float GroundCheckRadius = 0.25f;
    public LayerMask GroundMask;

    // States
    public WalkState WalkState { get; private set; }
    public RunState RunState { get; private set; }
    public JumpState JumpState { get; private set; }
    public InAirState InAirState { get; private set; }
    public CrouchState CrouchState { get; private set; }
    public IdleState IdleState { get; private set; }

    public PlayerStateMachine StateMachine { get; private set; }
    public Vector3 Velocity { get; internal set; }
    public bool IsGrounded { get; private set; }

    private float _lastJumpTime;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        StateMachine = new PlayerStateMachine(this);

        InitializeStates();
    }

    private void InitializeStates()
    {
        WalkState = new WalkState(StateMachine, this);
        RunState = new RunState(StateMachine, this);
        JumpState = new JumpState(StateMachine, this);
        InAirState = new InAirState(StateMachine, this);
        CrouchState = new CrouchState(StateMachine, this);
        IdleState = new IdleState(StateMachine, this);
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        UpdateGroundCheck();
        UpdateSlopeDetection();
        StateMachine.CurrentState.HandleInput();
        StateMachine.CurrentState.LogicUpdate();
        ApplyGravity();
        StateMachine.CurrentState.PhysicsUpdate();
    }

    public void SetMoveSpeed(float speed)
    {
       moveSpeed = speed;
    }

    public void Move(Vector2 input, float speedMultiplier = 1f)
{
    Vector3 moveDirection = Orientation.forward * input.y + Orientation.right * input.x;
    moveDirection.Normalize();

    // Aplica ajuste de slope se estiver em uma rampa
    if (_isOnSlope)
    {
        moveDirection = GetSlopeMoveDirection(moveDirection);
    }

    Velocity = new Vector3(
        moveDirection.x * moveSpeed * speedMultiplier,
        Velocity.y,
        moveDirection.z * moveSpeed * speedMultiplier
    );

    CharacterController.Move(Velocity * Time.deltaTime);

    // Aplica força extra para evitar escorregar em rampas
    ApplySlopeForce();
}

   public void Jump()
{
    if (CanJump())
    {
        Vector3 jumpDirection = Vector3.up;

        // Se estiver na rampa, ajustar o vetor do pulo para a normal da rampa
        if (_isOnSlope)
        {
            jumpDirection = _slopeHit.normal;
        }

        // Aplica o impulso do pulo na direção ajustada
        Velocity = new Vector3(Velocity.x, 0, Velocity.z) + jumpDirection * Mathf.Sqrt(JumpForce * -2f * Gravity);

        _lastJumpTime = Time.time;
    }
}


    public bool CanJump()
    {
        return IsGrounded && Time.time - _lastJumpTime > JumpCooldown;
    }

  private void ApplyGravity()
{
    if (IsGrounded && Velocity.y < 0 && Time.time - _lastJumpTime > 0.1f)
    {
        Vector3 vel = Velocity;
        vel.y = -2f;

        if (_isOnSlope && Velocity.y <= 0f)
        {
            vel.y -= slopeForce * Mathf.Sin(CurrentSlopeAngle * Mathf.Deg2Rad);
        }

        Velocity = vel;
    }
    else
    {
        Velocity += Vector3.up * Gravity * Time.deltaTime;
    }
}



    private void UpdateGroundCheck()
{
    Vector3 groundCheckPos = transform.position 
                             + CharacterController.center 
                             - Vector3.up * (CharacterController.height / 2f - CharacterController.skinWidth + 0.05f);
    IsGrounded = Physics.CheckSphere(groundCheckPos, GroundCheckRadius, GroundMask);
}


    private void UpdateSlopeDetection()
{
    Vector3 origin = transform.position + Vector3.up * 0.1f;
    float rayDistance = CharacterController.height / 2 * 1.5f + slopeRayExtraDistance;
    
    if (Physics.Raycast(origin, Vector3.down, out _slopeHit, rayDistance, GroundMask))
    {
        CurrentSlopeAngle = Vector3.Angle(_slopeHit.normal, Vector3.up);
        _isOnSlope = CurrentSlopeAngle > 0f && CurrentSlopeAngle <= maxSlopeAngle;
    }
    else
    {
        _isOnSlope = false;
        CurrentSlopeAngle = 0f;
    }
}

private Vector3 GetSlopeMoveDirection(Vector3 moveDirection)
{
    return Vector3.ProjectOnPlane(moveDirection, _slopeHit.normal).normalized;
}

private void ApplySlopeForce()
{
    if (_isOnSlope && IsGrounded && Velocity.y <= 0f)
    {
        float horizontalSpeed = new Vector3(Velocity.x, 0, Velocity.z).magnitude;
        CharacterController.Move(Vector3.down *
            CharacterController.height / 2 *
            slopeForce *
            Mathf.Sin(CurrentSlopeAngle * Mathf.Deg2Rad) *
            horizontalSpeed *
            Time.deltaTime);
    }
    // não aplicar se estiver pulando para não reduzir o impulso
}



}
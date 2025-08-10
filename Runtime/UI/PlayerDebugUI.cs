using UnityEngine;
using TMPro;

public class PlayerDebugUI : MonoBehaviour
{
    public PlayerMovement playerController;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI groundedText;

    private void Update()
    {
        // Get horizontal velocity (ignore vertical component)
        Vector3 horizontalVel = playerController.Velocity;
        horizontalVel.y = 0;

        float speed = horizontalVel.magnitude;
        speedText.text = $"Current Velocity: {speed:F2} m/s";

        // Current state
        string stateName = playerController.StateMachine.CurrentState.GetType().Name;
        stateText.text = $"State: {stateName}";

        // Grounded status
        bool grounded = playerController.IsGrounded;
        groundedText.text = $"Grounded: {grounded}";
        groundedText.color = grounded ? Color.green : Color.red;
    }
}
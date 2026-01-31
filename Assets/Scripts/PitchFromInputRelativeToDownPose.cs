using UnityEngine;

/// <summary>
/// Tilts the model forward/backward based on vertical movement input.
/// Attach to a pitch pivot that is a child of the yaw/facing pivot.
/// Tilts around the X-axis relative to the facing direction.
/// </summary>
public class PitchFromInputRelativeToDownPose : MonoBehaviour
{
    public PlayerController controller;

    [Header("Pitch setup")]
    [Tooltip("Neutral X rotation when not moving")]
    public float neutralPitch = 0f;
    [Tooltip("Max tilt angle when moving toward movement direction")]
    public float tiltAmount = 12f;
    public float smooth = 10f;
    public float deadZone = 0.05f;

    void LateUpdate()
    {
        if (!controller) return;

        Vector2 dir = controller.movement;
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        // Calculate how much we're moving "forward" in our facing direction
        // Use magnitude for forward tilt intensity (moving = tilt forward)
        float forwardAmount = dir.magnitude;
        if (forwardAmount < deadZone) forwardAmount = 0f;

        // Tilt forward (negative X) when moving
        float desiredPitch = neutralPitch - forwardAmount * tiltAmount;

        Quaternion target = Quaternion.Euler(desiredPitch, 0f, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, smooth * Time.deltaTime);
    }
}

using UnityEngine;

/// <summary>
/// Rotates the pivot around the Z-axis so a 2D/2.5D model faces the movement direction.
/// Attach to a parent pivot (e.g., YawPivot) above the model.
/// For a model pointing "down" at rest, movement down = 0°, right = 90°, up = 180°, left = -90°.
/// </summary>
public class Face2DMovementDirection : MonoBehaviour
{
    public PlayerController controller;

    [Header("Tuning")]
    private float smoothTime = 0.08f;
    private float deadZone = 0.05f;

    [Header("Offsets")]
    [Tooltip("Add degrees to align model's 'forward' with down direction")]
    private float angleOffsetDegrees = 0f;

    Vector2 lastDir = Vector2.down;
    float angularVel;

    void LateUpdate()
    {
        if (!controller) return;

        Vector2 dir = controller.movement;
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        if (dir.sqrMagnitude >= deadZone * deadZone)
            lastDir = dir.normalized;

        // Calculate angle: down=(0,-1)=0°, right=(1,0)=90°, up=(0,1)=180°, left=(-1,0)=-90°
        float targetAngle = Mathf.Atan2(lastDir.x, -lastDir.y) * Mathf.Rad2Deg + angleOffsetDegrees;

        float currentAngle = transform.localEulerAngles.z;
        float newAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref angularVel, smoothTime);

        // Rotate around Z-axis for 2D facing direction
        transform.localRotation = Quaternion.Euler(0f, 0f, newAngle);
    }
}

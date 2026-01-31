using UnityEngine;

public class YawFromInputNoFlip : MonoBehaviour
{
    public PlayerController controller;

    [Header("Tuning")]
    public float smoothTime = 0.06f;
    public float deadZone = 0.05f;

    [Header("Offsets / Fixes")]
    public float yawOffsetDegrees = 0f; // use this to align "down" correctly
    public bool invertX = false;
    public bool invertY = false;

    Vector2 lastDir = Vector2.down;
    float yawVel;

    void LateUpdate()
    {
        if (!controller) return;

        Vector2 dir = controller.movement;
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        if (dir.sqrMagnitude >= deadZone * deadZone)
            lastDir = dir.normalized;

        float x = invertX ? -lastDir.x : lastDir.x;
        float y = invertY ? -lastDir.y : lastDir.y;

        // Angle where:
        // (0, 1) = 0 deg, (1, 0) = 90 deg, (0, -1) = 180 deg, (-1, 0) = -90 deg
        float targetYaw = Mathf.Atan2(x, y) * Mathf.Rad2Deg + yawOffsetDegrees;

        float currentYaw = transform.localEulerAngles.y;
        float newYaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref yawVel, smoothTime);

        transform.localRotation = Quaternion.Euler(0f, newYaw, 0f);
    }
}

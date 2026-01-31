using UnityEngine;

public class Pitch2p5DFromInput : MonoBehaviour
{
    public PlayerController controller;

    [Header("Pitch setup")]
    public float neutralX = 90f;     // "neutral" pitch
    public float tiltAmount = 50f;   // 90 -> 140 when down, 90 -> 40 when up
    public float smooth = 12f;
    public float deadZone = 0.05f;

    Vector2 lastDir = Vector2.down;

    void LateUpdate()
    {
        if (!controller) return;

        Vector2 dir = controller.Movement;
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        if (dir.sqrMagnitude >= deadZone * deadZone)
            lastDir = dir.normalized;

        float desiredX = neutralX - lastDir.y * tiltAmount;

        Quaternion target = Quaternion.Euler(desiredX, 0f, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, smooth * Time.deltaTime);
    }
}

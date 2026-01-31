using UnityEngine;

public class BroccoliHopWalk : MonoBehaviour
{
    public PlayerController controller;

    [Header("Hop timing")]
    public float hopsPerSecondAtFullInput = 3.2f;
    public float deadZone = 0.05f;

    [Header("Jump feel")]
    public float hopHeight = 0.35f;          // make it obvious
    public float anticipationDip = 0.10f;    // small dip before launch
    public float landDip = 0.08f;            // small dip on landing

    [Header("Squash & stretch")]
    public float squashAmount = 0.20f;       // on ground
    public float stretchAmount = 0.15f;      // at apex

    [Header("Smoothing back to idle")]
    public float returnSpeed = 12f;

    Vector3 startPos;
    Vector3 startScale;
    float phase;

    void Awake()
    {
        startPos = transform.localPosition;
        startScale = transform.localScale;
    }

    void Update()
    {
        if (!controller) return;

        Vector2 input = controller._movement;
        if (input.sqrMagnitude > 1f) input.Normalize();
        float m = Mathf.Clamp01(input.magnitude);

        if (m < deadZone)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, returnSpeed * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, startScale, returnSpeed * Time.deltaTime);
            return;
        }

        phase += (hopsPerSecondAtFullInput * m) * Time.deltaTime;
        float t = Mathf.Repeat(phase, 1f); // 0..1 hop cycle

        // Phase split:
        // 0..0.15: anticipation dip
        // 0.15..0.85: flight (up then down)
        // 0.85..1: landing dip
        float yOffset = 0f;

        if (t < 0.15f)
        {
            float u = t / 0.15f;
            yOffset = -anticipationDip * Smooth01(u);
        }
        else if (t < 0.85f)
        {
            float u = (t - 0.15f) / 0.70f; // 0..1
            yOffset = hopHeight * Mathf.Sin(u * Mathf.PI); // 0->1->0
        }
        else
        {
            float u = (t - 0.85f) / 0.15f;
            yOffset = -landDip * (1f - Smooth01(u));
        }

        transform.localPosition = startPos + new Vector3(0f, yOffset, 0f);

        // Squash near ground, stretch near apex
        float flightU = Mathf.Clamp01((t - 0.15f) / 0.70f);
        float apex = Mathf.Sin(flightU * Mathf.PI); // 0..1..0
        float groundness = 1f - apex;

        float yScale = 1f - squashAmount * groundness + stretchAmount * apex;
        float xzScale = 1f + squashAmount * groundness * 0.6f - stretchAmount * apex * 0.4f;

        transform.localScale = new Vector3(startScale.x * xzScale, startScale.y * yScale, startScale.z * xzScale);
    }

    static float Smooth01(float x)
    {
        // Smoothstep-ish
        x = Mathf.Clamp01(x);
        return x * x * (3f - 2f * x);
    }
}

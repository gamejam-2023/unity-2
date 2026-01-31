using UnityEngine;

public class ShuffleWalkVisual : MonoBehaviour
{
    public PlayerController controller;

    [Header("Hop timing")]
    public float hopsPerSecondAtFullInput = 2.5f;
    public float deadZone = 0.05f;

    [Header("Jump feel")]
    public float hopHeight = 0.25f;
    public float anticipationDip = 0.06f;
    public float landDip = 0.05f;

    [Header("Squash & stretch")]
    public float squashAmount = 0.12f;
    public float stretchAmount = 0.08f;

    [Header("Smoothing")]
    public float returnSpeed = 8f;
    public float blendSpeed = 6f;

    Vector3 startPos;
    Vector3 startScale;
    float phase;
    float currentYOffset;
    Vector3 currentScale;
    float activeBlend; // 0 = idle, 1 = fully hopping

    void Awake()
    {
        startPos = transform.localPosition;
        startScale = transform.localScale;
        currentScale = startScale;
    }

    void LateUpdate()
    {
        if (!controller) return;

        Vector2 input = controller.movement;
        if (input.sqrMagnitude > 1f) input.Normalize();
        float m = Mathf.Clamp01(input.magnitude);

        bool isMoving = m >= deadZone;
        float targetBlend = isMoving ? 1f : 0f;
        activeBlend = Mathf.MoveTowards(activeBlend, targetBlend, blendSpeed * Time.deltaTime);

        if (isMoving)
        {
            phase += hopsPerSecondAtFullInput * m * Time.deltaTime;
        }

        float t = Mathf.Repeat(phase, 1f);

        // Smooth sine-based hop curve (no harsh phase transitions)
        float hopCurve = Mathf.Sin(t * Mathf.PI * 2f);
        float targetYOffset;

        if (hopCurve > 0f)
        {
            // Airborne: smooth arc up
            targetYOffset = hopCurve * hopHeight;
        }
        else
        {
            // Ground contact: small dip
            targetYOffset = hopCurve * anticipationDip;
        }

        // Smooth the Y offset to avoid stuttering
        currentYOffset = Mathf.Lerp(currentYOffset, targetYOffset * activeBlend, returnSpeed * Time.deltaTime);
        transform.localPosition = startPos + new Vector3(0f, currentYOffset, 0f);

        // Squash & stretch based on hop curve
        float stretchFactor = Mathf.Clamp01(hopCurve);
        float squashFactor = Mathf.Clamp01(-hopCurve);

        float yScale = 1f + stretchAmount * stretchFactor * activeBlend - squashAmount * squashFactor * activeBlend;
        float xzScale = 1f - stretchAmount * stretchFactor * 0.3f * activeBlend + squashAmount * squashFactor * 0.4f * activeBlend;

        Vector3 targetScale = new Vector3(startScale.x * xzScale, startScale.y * yScale, startScale.z * xzScale);
        currentScale = Vector3.Lerp(currentScale, targetScale, returnSpeed * Time.deltaTime);
        transform.localScale = currentScale;
    }
}

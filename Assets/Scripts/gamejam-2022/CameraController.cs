using UnityEngine;

/// <summary>
/// Diablo 3: Reaper of Souls style camera.
/// - Smooth follow without centering/reset
/// - Subtle drift in movement direction that stays
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    
    [Header("Follow Smoothing")]
    [Tooltip("How quickly camera follows target")]
    public float followSpeed = 5f;
    
    [Header("Look Ahead (Diablo style - no reset)")]
    [Tooltip("How much camera drifts in movement direction")]
    public float lookAheadAmount = 1f;
    
    [Tooltip("How quickly drift builds up")]
    public float driftSpeed = 1f;
    
    private Vector3 offset;
    private Vector3 currentDrift;
    private bool initialized;

    void Start()
    {
        if (target != null)
        {
            // Preserve the offset set in the scene
            offset = transform.position - target.position;
            initialized = true;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;
        
        if (!initialized)
        {
            offset = transform.position - target.position;
            initialized = true;
        }
        
        // Diablo-style: very subtle drift that doesn't reset
        Vector3 targetDrift = Vector3.zero;
        if (target.TryGetComponent<PlayerController>(out var pc))
        {
            Vector2 move = pc.RawInput;
            if (move.sqrMagnitude > 0.1f)
            {
                // Only drift while moving, but don't reset when stopping
                targetDrift = new Vector3(move.x, move.y, 0f) * lookAheadAmount;
            }
            else
            {
                // Keep current drift when stopped (Diablo style)
                targetDrift = currentDrift;
            }
        }
        
        // Slowly drift towards target (or stay if not moving)
        currentDrift = Vector3.Lerp(currentDrift, targetDrift, driftSpeed * Time.deltaTime);
        
        // Smoothly follow target + offset + drift
        Vector3 desiredPosition = target.position + offset + currentDrift;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}

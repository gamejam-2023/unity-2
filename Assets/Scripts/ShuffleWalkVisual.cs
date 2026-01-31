using UnityEngine;

public class ShuffleWalkVisual : MonoBehaviour
{
    public PlayerController controller;

    // Timing
    private const float MaxChargeTime = 0.125f;
    private const float MinChargeTime = 0.015f;
    private const float JumpTime = 0.5f;
    private const float LandTime = 0.01f;
    
    // Heights - scale with charge
    private const float MinJumpHeight = 0.15f;
    private const float MaxJumpHeight = 0.7f;
    private const float MinChargeDip = 0.03f;
    private const float MaxChargeDip = 0.15f;
    
    // Distance - scale with charge (multiplier on direction)
    private const float MinJumpPower = 0.25f;
    private const float MaxJumpPower = 1f;
    
    // Squash/stretch
    private const float ChargeSquash = 0.3f;
    private const float AirStretch = 0.15f;
    private const float LandSquash = 0.25f;
    
    private const float DeadZone = 0.05f;

    Vector3 startLocalPos;
    Vector3 startScale;
    
    public enum HopState { Idle, Charging, Airborne, Landing }
    public HopState State { get; private set; } = HopState.Idle;
    
    float stateTimer;
    float displayHeight;
    float displaySS;
    Vector3 displayScale;
    
    Vector2 committedDirection;
    Vector2 currentDirection;
    float chargeAmount;
    float currentJumpHeight;
    float currentJumpPower;

    public Vector2 MovementDirection => currentDirection;

    void Awake()
    {
        startLocalPos = transform.localPosition;
        startScale = transform.localScale;
        displayScale = startScale;
    }

    void LateUpdate()
    {
        if (!controller) return;

        float dt = Time.deltaTime;
        
        Vector2 input = controller.RawInput;
        if (input.sqrMagnitude > 1f) input.Normalize();
        
        bool wantsToMove = input.sqrMagnitude >= DeadZone * DeadZone;
        
        float targetHeight = 0f;
        float targetSS = 0f;
        
        switch (State)
        {
            case HopState.Idle:
                currentDirection = Vector2.zero;
                if (wantsToMove)
                {
                    State = HopState.Charging;
                    stateTimer = 0f;
                    chargeAmount = 0f;
                    committedDirection = input.normalized;
                }
                break;
                
            case HopState.Charging:
                stateTimer += dt;
                chargeAmount = Mathf.Clamp01(stateTimer / MaxChargeTime);
                
                float currentDip = Mathf.Lerp(MinChargeDip, MaxChargeDip, chargeAmount);
                targetHeight = -currentDip;
                targetSS = -ChargeSquash * chargeAmount;
                
                currentDirection = Vector2.zero;
                
                if (wantsToMove)
                    committedDirection = input.normalized;
                
                bool maxCharged = chargeAmount >= 1f;
                bool released = !wantsToMove && stateTimer >= MinChargeTime;
                
                if (maxCharged || released)
                {
                    // Launch with power based on charge
                    State = HopState.Airborne;
                    stateTimer = 0f;
                    currentJumpHeight = Mathf.Lerp(MinJumpHeight, MaxJumpHeight, chargeAmount);
                    currentJumpPower = Mathf.Lerp(MinJumpPower, MaxJumpPower, chargeAmount);
                    currentDirection = committedDirection * currentJumpPower;
                }
                else if (!wantsToMove && stateTimer < MinChargeTime)
                {
                    State = HopState.Idle;
                    stateTimer = 0f;
                }
                break;
                
            case HopState.Airborne:
                stateTimer += dt;
                float jumpT = stateTimer / JumpTime;
                
                float p = Mathf.Min(jumpT, 1f);
                float parabola = 4f * p * (1f - p);
                targetHeight = currentJumpHeight * parabola;
                targetSS = AirStretch * parabola * chargeAmount;
                
                // Air control - can steer but maintain power
                if (wantsToMove)
                {
                    Vector2 targetDir = input.normalized * currentJumpPower;
                    currentDirection = Vector2.Lerp(currentDirection, targetDir, 4f * dt);
                }
                
                if (jumpT >= 1f)
                {
                    // Instant transition if holding input
                    if (wantsToMove)
                    {
                        State = HopState.Charging;
                        stateTimer = 0f;
                        chargeAmount = 0f;
                        committedDirection = input.normalized;
                        currentDirection = Vector2.zero;
                    }
                    else
                    {
                        State = HopState.Landing;
                        stateTimer = 0f;
                        currentDirection = Vector2.zero;
                    }
                }
                break;
                
            case HopState.Landing:
                stateTimer += dt;
                float landT = Mathf.Min(stateTimer / LandTime, 1f);
                
                float landEase = 1f - landT;
                targetHeight = -MinChargeDip * landEase;
                targetSS = -LandSquash * landEase;
                
                currentDirection = Vector2.zero;
                
                if (landT >= 1f)
                {
                    if (wantsToMove)
                    {
                        State = HopState.Charging;
                        stateTimer = 0f;
                        chargeAmount = 0f;
                        committedDirection = input.normalized;
                    }
                    else
                    {
                        State = HopState.Idle;
                        stateTimer = 0f;
                    }
                }
                break;
        }
        
        displayHeight = Mathf.Lerp(displayHeight, targetHeight, 30f * dt);
        
        Vector3 localOffset = transform.parent != null 
            ? transform.parent.InverseTransformDirection(Vector3.up * displayHeight)
            : Vector3.up * displayHeight;
        transform.localPosition = startLocalPos + localOffset;
        
        displaySS = Mathf.Lerp(displaySS, targetSS, 30f * dt);
        float yScale = 1f + displaySS;
        float xzScale = 1f - displaySS * 0.5f;
        
        displayScale.x = Mathf.Lerp(displayScale.x, startScale.x * xzScale, 30f * dt);
        displayScale.y = Mathf.Lerp(displayScale.y, startScale.y * yScale, 30f * dt);
        displayScale.z = Mathf.Lerp(displayScale.z, startScale.z * xzScale, 30f * dt);
        transform.localScale = displayScale;
    }
}

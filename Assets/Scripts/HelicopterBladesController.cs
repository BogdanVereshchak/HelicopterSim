using UnityEngine;

public class HelicopterBladesController : MonoBehaviour
{
    private static readonly int PropellerSpinMultiplierHash = Animator.StringToHash("PropellerSpinMultiplier");
    private static readonly int TailRotorSpinMultiplierHash = Animator.StringToHash("TailRotorSpinMultiplier");

    Animator anim;
    HelicopterController helicopter;

    [Header("Rotor Speed Settings")]
    // Real speed of main propeller ~400 RPM. 400/60 = ~6.6 RPS. Animation have 1RPS
    [SerializeField] float maxMainRPS = 6.6f;
    // Tail Rotor spins at Approximately 4x speed of main
    [SerializeField] float tailRotorRatio = 4.0f;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (helicopter == null) helicopter = GetComponent<HelicopterController>();
    }

    void Update()
    {
        if (helicopter == null || anim == null) return;

        float normalizedRPM = helicopter.GetNormalizedRPM();
        float mainBladeMultiplier = maxMainRPS * normalizedRPM;
        float tailBladeMultiplier = maxMainRPS * tailRotorRatio * normalizedRPM;

        anim.SetFloat(PropellerSpinMultiplierHash, mainBladeMultiplier);
        anim.SetFloat(TailRotorSpinMultiplierHash, tailBladeMultiplier);
    }
}

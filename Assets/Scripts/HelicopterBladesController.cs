using UnityEngine;

public class HelicopterBladesController : MonoBehaviour
{
    private static readonly int PropellerSpinMultiplierHash = Animator.StringToHash("PropellerSpinMultiplier");
    private static readonly int TailRotorSpinMultiplierHash = Animator.StringToHash("TailRotorSpinMultiplier");

    Animator anim;
    HelicopterController helicopter;

    [Header("Rotor Speed Settings")]
    [SerializeField] float mainRPSFactor = 1f;
    [SerializeField] float tailRotorRatio = 4.0f;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (helicopter == null) helicopter = GetComponent<HelicopterController>();
    }

    void Update()
    {
        if (helicopter == null || anim == null) return;

        float mainRPS = helicopter.GetEngineRPM() / 60f;
        anim.SetFloat(PropellerSpinMultiplierHash, mainRPS * mainRPSFactor);
        anim.SetFloat(TailRotorSpinMultiplierHash, mainRPS * tailRotorRatio);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class HelicopterController : MonoBehaviour
{
    [Header("Engine")]
    [SerializeField] float maxEngineRPM = 450f;
    [SerializeField] float spoolUpRate = 90f;
    [SerializeField] float spoolDownRate = 60f;
    [SerializeField] bool engineOn = true;

    [Header("Main Rotor")]
    [SerializeField] float maxThrust = 26000f;
    [SerializeField] float collectiveRate = 0.5f;

    [Header("Attitude (cyclic)")]
    [SerializeField] float maxTiltAngle = 20f;
    [SerializeField] float attitudeStiffness = 8f;
    [SerializeField] float attitudeDamping = 4.5f;

    [Header("Yaw (tail rotor)")]
    [SerializeField] float maxYawRate = 1.2f;      // rad/s
    [SerializeField] float yawResponse = 6f;       // 1/s
    [SerializeField] float rotorReaction = 0.2f;

    [Header("Drag")]
    [SerializeField] Vector3 dragCdA = new Vector3(5f, 12f, 3f);
    [SerializeField] float airDensity = 1.2f;

    [SerializeField] float maxAngularVelocity = 3f;

    Rigidbody rb;
    Vector2 moveInput;
    float altitudeInput;
    float rotationInput;
    float collective;
    float currentEngineRPM;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        UpdateEngine(dt);
        UpdateCollective(dt);

        float rpm = GetNormalizedRPM();
        float thrust = maxThrust * collective * rpm * rpm;

        rb.AddRelativeForce(Vector3.up * thrust, ForceMode.Force);
        ApplyAttitudeControl(rpm);
        ApplyYawControl(rpm, thrust / maxThrust);
        ApplyDrag();
    }

    void UpdateEngine(float dt)
    {
        float target = engineOn ? maxEngineRPM : 0f;
        float rate = target > currentEngineRPM ? spoolUpRate : spoolDownRate;
        currentEngineRPM = Mathf.MoveTowards(currentEngineRPM, target, rate * dt);
    }

    void UpdateCollective(float dt)
    {
        collective = Mathf.Clamp01(collective + altitudeInput * collectiveRate * dt);
        if (!engineOn) collective = Mathf.MoveTowards(collective, 0f, collectiveRate * dt);
    }

    void ApplyAttitudeControl(float rpm)
    {
        Quaternion yawFrame = Quaternion.Euler(0f, rb.rotation.eulerAngles.y, 0f);
        Quaternion tilt = Quaternion.Euler(moveInput.y * maxTiltAngle, 0f, -moveInput.x * maxTiltAngle);
        Vector3 targetUp = yawFrame * tilt * Vector3.up;

        Vector3 errLocal = transform.InverseTransformDirection(Vector3.Cross(transform.up, targetUp));
        Vector3 angVelLocal = transform.InverseTransformDirection(rb.angularVelocity);

        Vector3 accel = new Vector3(
            errLocal.x * attitudeStiffness - angVelLocal.x * attitudeDamping,
            0f,
            errLocal.z * attitudeStiffness - angVelLocal.z * attitudeDamping) * rpm;

        rb.AddRelativeTorque(accel, ForceMode.Acceleration);
    }

    void ApplyYawControl(float rpm, float thrustNorm)
    {
        float yawRate = transform.InverseTransformDirection(rb.angularVelocity).y;
        float desired = rotationInput * maxYawRate;

        float reaction = -rotorReaction * thrustNorm;
        float tailRotor = (desired - yawRate) * yawResponse;

        rb.AddRelativeTorque(0f, reaction + tailRotor * rpm, 0f, ForceMode.Acceleration);
    }

    void ApplyDrag()
    {
        Vector3 v = transform.InverseTransformDirection(rb.linearVelocity);
        Vector3 f = new Vector3(
            -v.x * Mathf.Abs(v.x) * dragCdA.x,
            -v.y * Mathf.Abs(v.y) * dragCdA.y,
            -v.z * Mathf.Abs(v.z) * dragCdA.z) * (0.5f * airDensity);
        rb.AddRelativeForce(f, ForceMode.Force);
    }

    public void OnMove(InputValue v) => moveInput = v.Get<Vector2>();
    public void OnAltitude(InputValue v) => altitudeInput = v.Get<float>();
    public void OnRotation(InputValue v) => rotationInput = v.Get<float>();
    public void OnToggleEngine(InputValue v) { if (v.isPressed) engineOn = !engineOn; }

    public void SetEngineState(bool on) => engineOn = on;
    public float GetEngineRPM() => currentEngineRPM;
    public float GetNormalizedRPM() => maxEngineRPM > 0f ? currentEngineRPM / maxEngineRPM : 0f;
    public float GetCollective() => collective;
    public Vector3 GetLocalVelocity() => transform.InverseTransformDirection(rb.linearVelocity);
}
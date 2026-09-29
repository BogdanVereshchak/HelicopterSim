using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class HelicopterController : MonoBehaviour
{
    [Header("Engine & Rotor")]
    [SerializeField] float maxEngineRPM = 450f;
    [SerializeField] float spoolUpRate = 90f;
    [SerializeField] float spoolDownRate = 60f;
    [SerializeField] bool engineOn = true;
    

    [Header("Movement Settings")]
    [SerializeField] float maxLiftForce = 25000f;
    [SerializeField] float cyclicTorque = 10000f;
    [SerializeField] float tailRotorPower = 40000f;
    [SerializeField] float reactiveTorque = 8000f;
    [SerializeField] float maxAngularVelocity = 3f;

    Vector2 moveInput; // Pitch & Roll 
    Vector2 lookInput; 
    float altitudeInput; // Lift
    float rotationInput; // Yaw

    float currentEngineRPM = 0f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity; 
    }

    void FixedUpdate()
    {
        HandleEngineRPM();
        MoveHelicopter();
    }

    private void HandleEngineRPM()
    {
        float targetRPM = engineOn ? maxEngineRPM : 0f;
        float rate = (targetRPM > currentEngineRPM) ? spoolUpRate : spoolDownRate;

        currentEngineRPM = Mathf.MoveTowards(currentEngineRPM, targetRPM, rate * Time.fixedDeltaTime);
    }

    private void MoveHelicopter()
    {   
        float rpmRatio = (maxEngineRPM > 0f) ? (currentEngineRPM / maxEngineRPM) : 0f;
        float lift = Mathf.Max(0f, altitudeInput) * maxLiftForce * rpmRatio;
        Vector3 localLift = Vector3.up * lift;


        Vector3 localCyclic = new Vector3(moveInput.y, 0f, -moveInput.x) * (cyclicTorque * rpmRatio);

        float reactiveYaw = -reactiveTorque * rpmRatio;
        float pilotYaw = rotationInput * tailRotorPower * rpmRatio;
        Vector3 localYaw = new Vector3(0f, reactiveYaw + pilotYaw, 0f);

        rb.AddRelativeForce(localLift, ForceMode.Force);
        rb.AddRelativeTorque(localCyclic + localYaw, ForceMode.Force);
    }  

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    public void OnAltitude(InputValue value) => altitudeInput = value.Get<float>();
    public void OnRotation(InputValue value) => rotationInput = value.Get<float>();

    public void SetEngineState(bool isOn) => engineOn = isOn;
    public float GetEngineRPM() => currentEngineRPM;
    public float GetNormalizedRPM() => (maxEngineRPM > 0f) ? (currentEngineRPM / maxEngineRPM) : 0f;
    public float GetThrust() => Mathf.Max(0f, altitudeInput) * maxLiftForce * GetNormalizedRPM();
}

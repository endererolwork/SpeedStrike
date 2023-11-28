using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using Utilities;


namespace Race
{
    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
        public WheelFrictionCurve originalForwardFriction;
        public WheelFrictionCurve originalSidewaysFriction;
    }

    public class CarController : MonoBehaviour
    {
        [Header("Axle Information")] [SerializeField]
        AxleInfo[] axleInfos;

        [Header("Motor Attributes")] [SerializeField]
        float maxMotorTorque = 3000f;

        [SerializeField] float maxSpeed;

        [Header("Steering Attributes")] [SerializeField]
        float maxSteeringAngle = 30f;

        [SerializeField] private AnimationCurve turnCurve;
        [SerializeField] private float turnStrength = 1500f;

        [Header("Braking and Drifting Attributes")] [SerializeField]
        private float driftSteeringMultiplier = 1.5f; //change steering when drifting

        [SerializeField] float brakeTorque = 10000f;

        [Header("Physics")] [SerializeField] private Transform centerOfMass;
        [SerializeField] private float downForce = 100f;
        [SerializeField] private float gravity = Physics.gravity.y;
        [SerializeField] private float lateralGScale = 10f; //Scaling factor for lateral G forces;

        [Header("Banking")] [SerializeField] private float maxBankAngle = 5f;
        [SerializeField] private float bankSpeed = 2f;

        [Header("References")] [SerializeField]
        private InputReader input;
        

        Rigidbody rb;

        private float brakeVelocity;
        private Vector3 kartVelocity;
        private float driftVelocity;

        private RaycastHit hit;

        const float thresholdSpeed = 10f;
        private const float centerOfMassOffset = -0.5f;
        private Vector3 originalCentorOfMass;

        public bool IsGrounded = true;
        public Vector3 Velocity => kartVelocity;
        public float MaxSpeed => maxSpeed;



        void Start()
        {
            rb = GetComponent<Rigidbody>();
            input.Enable();

            rb.centerOfMass = centerOfMass.localPosition;
            originalCentorOfMass = centerOfMass.localPosition;

            foreach (AxleInfo axleInfo in axleInfos)
            {
                axleInfo.originalForwardFriction = axleInfo.leftWheel.forwardFriction;
                axleInfo.originalSidewaysFriction = axleInfo.leftWheel.sidewaysFriction;
            }
        }

        private void FixedUpdate()
        {
            float verticalInput = AdjustInput(input.Move.y);
            float horizontalInput = AdjustInput(input.Move.x);

            float motor = maxMotorTorque * verticalInput;
            float steering = maxSteeringAngle * horizontalInput;

            UpdateAxles(motor, steering);
            UpdateBanking(horizontalInput);

            kartVelocity = transform.InverseTransformDirection(rb.velocity);

            if (IsGrounded)
            {
                HandleGroundedMovement(verticalInput, horizontalInput);
            }
            else
            {
                HandleAirborneMovement(verticalInput, horizontalInput);
            }
        }

        private void UpdateBanking(float horizontalInput)
        {
            //Bank the Kart in the opposite direction of the turn
            float targetBankAngle = horizontalInput * -maxBankAngle;
            Vector3 currentEuler = transform.localEulerAngles;
            currentEuler.z = Mathf.LerpAngle(currentEuler.z, targetBankAngle, Time.deltaTime * bankSpeed);
            transform.localEulerAngles = currentEuler;
        }

        private void HandleGroundedMovement(float verticalInput, float horizontalInput)
        {
            if (Mathf.Abs(verticalInput) > 0.1f || Mathf.Abs(kartVelocity.z) > 1)
            {
                float turnMultiplier = Mathf.Clamp01(turnCurve.Evaluate(kartVelocity.magnitude / maxSpeed));
                rb.AddTorque(Vector3.up *
                             (horizontalInput * Mathf.Sign(kartVelocity.z) * turnStrength * 100f *
                              turnMultiplier)); // Turning logic
            }

            //Acceleration Logic
            if (!input.IsBraking)
            {
                float targetSpeed = verticalInput * maxSpeed;
                Vector3 forwardWithoutY = transform.forward.With(y: 0).normalized;
                rb.velocity = Vector3.Lerp(rb.velocity, forwardWithoutY * targetSpeed, Time.deltaTime);
            }
            //Downforce - always push the cart down, using lateral GS to scale the force if the Kart is moveing sideways fast and quick
            float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / maxSpeed);
            float lateralG = Mathf.Abs(Vector3.Dot(rb.velocity, transform.right));
            float downForceFactor = Mathf.Max(speedFactor, lateralG / lateralGScale);
            rb.AddForce(-transform.up * (downForce * rb.mass * downForceFactor));
            
            // Shift Center of Mass
            float speed = rb.velocity.magnitude;
            Vector3 centerOfMassAdjustment = (speed > thresholdSpeed)
                ? new Vector3(0f, 0f,
                    Mathf.Abs(verticalInput) > 0.1f ? Mathf.Sign(verticalInput) * centerOfMassOffset : 0f)
                : Vector3.zero;

            rb.centerOfMass = originalCentorOfMass + centerOfMassAdjustment;
        }

        private void HandleAirborneMovement(float verticalInput, float horizontalInput)
        {
            rb.velocity =
                Vector3.Lerp(rb.velocity, rb.velocity + Vector3.down * gravity,
                    Time.deltaTime * gravity); //apply gravity to the Karth while its airbone.
        }

        void UpdateAxles(float motor, float steering)
        {
            foreach (AxleInfo axleInfo in axleInfos)
            {
                HandleSteering(axleInfo, steering);
                HandleMotor(axleInfo, motor);
                HandleBreaksAndDrift(axleInfo);
                UpdateWheelVisiuals(axleInfo.leftWheel);
                UpdateWheelVisiuals(axleInfo.rightWheel);
            }
        }

        private void UpdateWheelVisiuals(WheelCollider collider)
        {
            if (collider.transform.childCount == 0) return;
            {
                Transform visualWheel = collider.transform.GetChild(0);

                Vector3 position;
                Quaternion rotation;

                collider.GetWorldPose(out position, out rotation);

                visualWheel.transform.position = position;
                visualWheel.transform.rotation = rotation;
            }
        }

        void HandleSteering(AxleInfo axleInfo, float steering)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
        }

        void HandleMotor(AxleInfo axleInfo, float motor)
        {
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }

        void HandleBreaksAndDrift(AxleInfo axleInfo)
        {
            if (axleInfo.motor)
            {
                if (input.IsBraking)
                {
                    rb.constraints = RigidbodyConstraints.FreezePositionX;

                    float newZ = Mathf.SmoothDamp(rb.velocity.z, 0, ref brakeVelocity, 1f);

                    rb.velocity = rb.velocity.With(z: newZ);

                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;
                    ApplyDriftFriction(axleInfo.leftWheel);
                    ApplyDriftFriction(axleInfo.rightWheel);
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.None;

                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;
                    ResetDriftFriction(axleInfo.leftWheel);
                    ResetDriftFriction(axleInfo.rightWheel);
                }
            }
        }

        private void ResetDriftFriction(WheelCollider wheel)
        {
            AxleInfo axleInfo = axleInfos.FirstOrDefault(axle => axle.leftWheel == wheel || axle.rightWheel
                == wheel);

            if (axleInfo == null) return;

            wheel.forwardFriction = axleInfo.originalForwardFriction;
            wheel.sidewaysFriction = axleInfo.originalSidewaysFriction;

        }

        private void ApplyDriftFriction(WheelCollider wheel)
        {
            if (wheel.GetGroundHit(out var hit))
            {
                wheel.forwardFriction = UpdateFriction(wheel.forwardFriction);
            }
        }

        WheelFrictionCurve UpdateFriction(WheelFrictionCurve wheelForwardFriction)
        {
            wheelForwardFriction.stiffness = input.IsBraking
                ? Mathf.SmoothDamp(
                    wheelForwardFriction.stiffness, .5f, ref
                    driftVelocity, Time.deltaTime)
                : 1f;
            return wheelForwardFriction;
        }

        float AdjustInput(float input)
        {
            return input switch
            {
                >= .7f => 1f,
                <= -0.7f => -1f,
                _ => input
            };
        }
    }
}
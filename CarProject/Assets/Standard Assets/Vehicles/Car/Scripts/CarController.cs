using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Vehicles.Car
{
    public class CarController : MonoBehaviour
    {

        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
        [SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
        [SerializeField] private Transform m_SteeringWheelAngle;
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_MaximumSteerAngle;
        [Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_Downforce = 100f;

        [SerializeField] private static int NoOfGears;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        [SerializeField] private float m_SlipLimit;

        private Quaternion[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private int m_GearNum;
        private float m_OldRotation;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;

        public bool Skidding { get; private set; }
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle{ get { return m_SteerAngle; }}
        public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude * 3.6f; } }
        public float Revs { get; private set; }
        public float AccelInput { get; private set; }
        public float MaxSpeed { get; private set; }

        public Text displaySpeed;
        public Text displayGear;
        public Text displayHour;
        private float[] m_MaxSpeedPerGear;

        public struct Torque {
            public float current;
            public float reverse;
            public float brake;
            public float fullTorque;
            public float maxHandbrake;
        };
        private Torque m_Torque;

        // Use this for initialization
        private void Start()
        {
            m_WheelMeshLocalRotations = new Quaternion[4];
            for (int i = 0; i < 4; i++) {
                m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
            }
            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_Torque.fullTorque = 1500;
            m_Torque.maxHandbrake = float.MaxValue;
            m_Torque.reverse = 1000;
            m_Torque.brake = 3000;

            m_Rigidbody = GetComponent<Rigidbody>();

            
            m_Torque.current = m_Torque.fullTorque - (m_TractionControl * m_Torque.fullTorque);

            m_GearNum = 1; // Start at gear 1, not 0

            m_MaxSpeedPerGear = new float[8];
            m_MaxSpeedPerGear[0] = 30;
            m_MaxSpeedPerGear[1] = 25;
            m_MaxSpeedPerGear[2] = 40;
            m_MaxSpeedPerGear[3] = 60;
            m_MaxSpeedPerGear[4] = 80;
            m_MaxSpeedPerGear[5] = 120;
            m_MaxSpeedPerGear[6] = 180;

            NoOfGears = m_MaxSpeedPerGear.Length - 1; // Because 0 is R and is not moving forward gear
        } // End of Start

        public void Update() { } // Do not remove, got from MonoBehaviour

        public void Move(float steering, float accel, float footbrake, float handbrake) {

            // Update the positions of the wheel meshes (the actual wheels that you can see)
            for (int i = 0; i < 4; i++) {
                Quaternion quat;
                Vector3 position;
                m_WheelColliders[i].GetWorldPose(out position, out quat);
                m_WheelMeshes[i].transform.position = position;
                m_WheelMeshes[i].transform.rotation = quat;
            }

            // Clamp input values 
            steering = Mathf.Clamp(steering, -1, 1); // if (steering < -1) steering = -1; else if (steering > 1) steering = 1;
            AccelInput = accel = Mathf.Clamp(accel, 0, 1); // You get the point.
            BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0); // Oh, and a = b = c means a = c; and b = c;
            handbrake = Mathf.Clamp(handbrake, 0, 1);

            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            m_SteerAngle = steering * m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;

            // Print the important values to the screen
            RotateSteeeringWheel(steering);
            UpdateUI(m_GearNum, CurrentSpeed, DateTime.Now, 25); // -> Text Components modified

            SteerHelper(m_WheelColliders, m_OldRotation, m_SteerHelper, m_Rigidbody); // -> (m_Rigidbody, m_OldRotation) modified
            ApplyDrive(accel, footbrake, m_GearNum, CurrentSpeed, m_MaxSpeedPerGear, m_Torque);

            //Set the handbrake.
            //Assuming that wheels 2 and 3 are the rear wheels.
            if (handbrake >= 0f) {
                var hbTorque = handbrake * m_Torque.maxHandbrake;
                m_WheelColliders[2].brakeTorque = hbTorque;
                m_WheelColliders[3].brakeTorque = hbTorque;
            }

            Revs = CalculateRevs(m_GearNum, NoOfGears, m_RevRangeBoundary);
            m_WheelColliders[0] = AddDownForce(m_WheelColliders[0], m_Downforce);
            CheckForWheelSpin();
            m_Torque = TractionControl(m_WheelColliders, m_Torque);
        } // End of Move

        private void RotateSteeeringWheel(float steering) {
            var rotateAngle = 450f;
            m_SteeringWheelAngle.rotation = Quaternion.Euler(new Vector3(0, 0, -steering * rotateAngle));
        }

        public void IncreaseGear() {            
            if (m_GearNum >= 1)
                m_GearNum = (m_GearNum + 1 < NoOfGears) ? m_GearNum + 1 : m_GearNum;
            if (m_GearNum == 0 && (0 < CurrentSpeed && CurrentSpeed < 0.5))
                m_GearNum = (m_GearNum + 1 < NoOfGears) ? m_GearNum + 1 : m_GearNum;
        } // IncreaseGear

        public void DecreaseGear() {
            if (m_GearNum == 1 && (0 < CurrentSpeed && CurrentSpeed < 0.5))
                m_GearNum = (m_GearNum - 1 >= 0) ? m_GearNum - 1 : m_GearNum;
            if (m_GearNum > 1)
                m_GearNum = (m_GearNum - 1 >= 0) ? m_GearNum - 1 : m_GearNum;            
        } // End of DecreaseGear

        private void UpdateUI(int gear, float speed, DateTime time, float temperature) {
            displayGear.text = String.Format("{1}\n{0:0000} rpm", Revs * 4000, (m_GearNum == 0) ? "R" : m_GearNum.ToString());
            displaySpeed.text = String.Format("{0:0.00} km/h", speed);
            displayHour.text = String.Format("{0:HH:mm tt}", time);
        } // End of UpdateGearUI
        
        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private static float CurveFactor(float factor) {
            return 1 - (1 - factor)*(1 - factor); // Same as return factor ^ 2 - 2 * factor;
        } // End of CurveFactor


        // unclamped version of Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value) {
            return (1.0f - value)*from + value*to;
        } // End of ULerp

        private float CalculateRevs(int currentGear, int numberOfGears, float revRangeBoundary) {
            return CurrentSpeed / (m_MaxSpeedPerGear[m_GearNum] / 1);
        } // End of CalculateRevs

        private Rigidbody CapSpeed(Rigidbody rigidbody, float maxSpeed) {
            float speed = rigidbody.velocity.magnitude * 3.6f; // Convert to Km/h from m/s

            if (speed > maxSpeed) {
                rigidbody.velocity = (maxSpeed / 3.6f) * rigidbody.velocity.normalized; // Convert back to m/s
            }

            return rigidbody;
        } // End of CarSpeed

        private void ApplyDrive(float accel, float footbrake, int currentGear, float currentSpeed, float[] maxSpeedPerGear, Torque torque) {
            
            float thrustTorque;
            var upperLimit = maxSpeedPerGear[currentGear];
            var lowerLimit = (currentGear == 0 || currentGear == 1)? 0 : maxSpeedPerGear[currentGear - 1] - 5;

            Func<float, float, float, float> torqueFactor = (speed, lower, upper) => {
                if (speed > upper) return 0;
                if (speed < lower) return Mathf.InverseLerp(0, lower, speed) + 0.1f;
                return 1;
            };

            if (currentGear == 0) thrustTorque = -torque.reverse * accel;
            else thrustTorque = (torque.current / 2f) * accel * torqueFactor(currentSpeed, lowerLimit, upperLimit);

            m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;

            // Apply brakes
            foreach (var wheelCollider in m_WheelColliders) {
                wheelCollider.brakeTorque = torque.brake * footbrake; // when footbrake is pressed
                wheelCollider.brakeTorque = (Revs > 1.2)? 500000f : wheelCollider.brakeTorque; // when Revs is too high
            }
        } // End of ApplyDrive

        private void SteerHelper(WheelCollider[] wheelColliders, float oldRotation, float steerHelper, Rigidbody rigidbody)
        {
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelhit;
                wheelColliders[i].GetGroundHit(out wheelhit);
                if (wheelhit.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(oldRotation - transform.eulerAngles.y) < 10f)
            {
                var turnadjust = (transform.eulerAngles.y - oldRotation) * steerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                m_Rigidbody.velocity = velRotation * rigidbody.velocity;
            }

            m_OldRotation = transform.eulerAngles.y;
        } // End of SteerHelper


        // this is used to add more grip in relation to speed
        private WheelCollider AddDownForce(WheelCollider wheelCollider, float downForce) {
            wheelCollider.attachedRigidbody.AddForce(-transform.up * downForce *
                                                         wheelCollider.attachedRigidbody.velocity.magnitude);
            return wheelCollider;
        } // End of WheelColider


        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tire skidding sounds
        // 3) leaves skidmarks on the ground
        // these effects are controlled through the WheelEffects class
        private void CheckForWheelSpin()
        {
            // loop through all wheels
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelHit;
                m_WheelColliders[i].GetGroundHit(out wheelHit);

                // is the tire slipping above the given threshhold
                if (Mathf.Abs(wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= m_SlipLimit)
                {
                    m_WheelEffects[i].EmitTyreSmoke();

                    // avoiding all four tires screeching at the same time
                    // if they do it can lead to some strange audio artefacts
                    if (!AnySkidSoundPlaying() && false) // REMOVE && FALSE <<--------------------------------------------OMGWTFBBQ klhfai bfklafbalb fbalw ebfjbawrfjbaerj
                    {
                        m_WheelEffects[i].PlayAudio();
                    }
                    continue;
                }

                // if it wasnt slipping stop all the audio
                if (m_WheelEffects[i].PlayingAudio)
                {
                    m_WheelEffects[i].StopAudio();
                }
                // end the trail generation
                m_WheelEffects[i].EndSkidTrail();
            }
        } // End of CheckForWheelSpin

        // crude traction control that reduces the power to wheel if the car's wheel spinning too much
        private Torque TractionControl(WheelCollider[] wheelColliders, Torque torque) {
            WheelHit wheelHit;

            wheelColliders[0].GetGroundHit(out wheelHit);
            torque = AdjustTorque(wheelHit.forwardSlip, torque);

            wheelColliders[1].GetGroundHit(out wheelHit);
            torque = AdjustTorque(wheelHit.forwardSlip, torque);

            return torque;
        } // End of TractionControl

        private Torque AdjustTorque(float forwardSlip, Torque torque) {

            if (forwardSlip >= m_SlipLimit && torque.current >= 0) {
                torque.current -= 10 * m_TractionControl;
            }
            else {
                torque.current += 10 * m_TractionControl;
                if (torque.current > torque.fullTorque) {
                    torque.current = torque.fullTorque;
                }
            }
            return torque;
        } // End of AdjustTorque

        private bool AnySkidSoundPlaying() {
            foreach (var wheelEffect in m_WheelEffects) {
                if (wheelEffect.PlayingAudio) {
                    return true;
                }
            }
            return false;
        } // End of AnySkidSoundsPlaying
    } // End of Class CarController
} // End of Namespace UnityStandardAssets.Vehicles.Car

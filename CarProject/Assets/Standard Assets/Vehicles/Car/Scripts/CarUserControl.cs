using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private bool incGearPressed = false;
        private bool decGearPressed = false;
        private CarController m_Car; // the car controller we want to use

        private void Awake() {
            // get the car controller
            m_Car = GetComponent<CarController>();
        } // End of Awake

        private void FixedUpdate() {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float brr = CrossPlatformInputManager.GetAxis("Vertical");
            float acc = CrossPlatformInputManager.GetAxis("ACC");
            float hPad = CrossPlatformInputManager.GetAxis("PADLR");
            float brrPad = CrossPlatformInputManager.GetAxis("PADS");
            float accPad = CrossPlatformInputManager.GetAxis("PADW");
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");

            acc = (accPad == 0) ? acc : accPad;
            brr = (brrPad == 0) ? brr : brrPad;
            h = (hPad ==0 ) ? h : hPad;

            m_Car.Move(h, acc, brr, handbrake);

            float gearAxis = Input.GetAxis("Gear");
            bool incGear = (gearAxis > 0);
            bool decGear = (gearAxis < 0);

            if (incGear && !incGearPressed) {
                m_Car.IncreaseGear();
                incGearPressed = true;
            }
            else if (!incGear) {
                incGearPressed = false;
            }

            if (decGear && !decGearPressed) {
                m_Car.DecreaseGear();
                decGearPressed = true;
            }
            else if (!decGear) {
                decGearPressed = false;
            }
        } // End of FixedUpdate
    } // End of Class CarUserControl
} // End of Namespace UnityStandardAssets.Vehicles.Car

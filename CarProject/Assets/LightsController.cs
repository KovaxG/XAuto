using System;
using System.Net;
using System.Text;

namespace Testsender
{
    /// <summary>
    /// Non - UWP version of light controller.
    /// </summary>
    class LightsController : ILightsControllerService
    {

        private bool[] _state = new bool[3]; // The light can be on or off
        private byte[] _bri = new byte[3]; // The brightness can be on or off

        private string _userName = "CQzXMSHE28ThCMWYKznWLBTGqCTOZEZpdSrexG2h"; // The username of the lightControllerBox
        private string _ipAddress = "172.27.94.101"; // The IP of the Philips Hue Bridge

        /// <summary>
        /// Because the actions in the constructor might fail, I decided to throw an exception to 
        /// let the user know if a connection could or could not be made.
        /// </summary>
        public LightsController(bool state, byte brightness)
        {

            // Initialize all the lights
            var succes = UpdateLightState(0, state, brightness) &&
                         UpdateLightState(1, state, brightness) &&
                         UpdateLightState(2, state, brightness);

            if (!succes) throw new Exception("Could not initialise Light State in LightsController Constructor.");
        } // End of Constructor

        /// <summary>
        /// Can change the state and brightness of a bulb.
        /// </summary>
        public bool UpdateLightState(int light, bool state, byte bri)
        {
            var stringData = string.Format("{{\"on\": {0}, \"bri\" : {1}}}", state.ToString().ToLower(), bri.ToString());
            return SendRequest(stringData, light, state, bri);
        } // End of LightState

        /// <summary>
        /// Same as UpdateLightState, except it can also change the hue and the saturation (if applicable).
        /// (TODO break light 2. Story Points: Over 9000. -> Done.)
        /// </summary>
        public bool UpdateLightState(int light, bool state, byte bri, int hue, int sat)
        {
            var stringData =
                (light == 2) ?
                string.Format("{{\"on\": {0}, \"bri\" : {1}, \"hue\" : {2}, \"sat\" : {3}}}", state.ToString().ToLower(), bri, hue, sat) :
                string.Format("{{\"on\": {0}, \"bri\" : {1}}}", state.ToString().ToLower(), bri.ToString());

            return SendRequest(stringData, light, state, bri);
        } // End of LightState

        /// <summary>
        /// Change the state of the light bulbs. 
        /// </summary>
        private bool SendRequest(string stringData, int light, bool state, byte bri)
        {
            try
            {
                using (var client = new WebClient())
                {

                    var address = string.Format("http://{0}/api/{1}/lights/{2}/state", _ipAddress, _userName, (light + 1));
                    var data = Encoding.ASCII.GetBytes(stringData);

                    client.UploadData(address, "PUT", data);

                    _state[light] = state;
                    _bri[light] = bri;
                }
            }
            catch { return false; }// Failure
            return true; // Succes!
        } // End of SendRequest

        public string GetState(int light) { return (_state[light]) ? "ON" : "OFF"; } // End of GetState
        public byte GetBrightness(int light) { return _bri[light]; } // End of GetBrightness

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        public string IPAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }
    } // End of Class LightsController
} // End of Namespace

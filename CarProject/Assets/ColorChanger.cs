using UnityEngine;
using Testsender;
using UnityEngine.UI;
using System;
using System.Threading;

/// <summary>
/// NAME IS MISLEADING!
/// It actually should have been named something like CarLightsEnabler or something.
/// </summary>
public class ColorChanger : MonoBehaviour
{

    private ILightsControllerService _lightController; // This class deals with sending stuff to the bulbs
    private Thread thread;
    private string _inputLabel; // The button that controls the lights
    public Text displayLabel; // The state of the lights
    private Func<string> _showLightStatus; // A function that actually tells us about the state of the bulbs
    private bool atStart = true;

    // Use this for initialization
    void Start()
    {
        
        _showLightStatus = () =>  "Connecting to lights...";

        _inputLabel = "ControlLumina";

        // This is done because Unity doesn't have async
        if (atStart) {
            atStart = false;
            thread = new Thread(() => {
                try {
                    _lightController = new LightsController(false, 50);

                    _showLightStatus = () => string.Format("LightBulb = {0}", _lightController.GetState(0));
                }
                catch (Exception e) {
                    _showLightStatus = () => "NO CONNECTION.";
                }
            });

            thread.Start();
        }
    }// End of Start

    // Update is called once per frame
    void Update() {
        displayLabel.text = _showLightStatus();

        if (Input.GetAxis(_inputLabel) > 0 && _lightController != null)
        {
            _lightController.UpdateLightState(0, true, 50);
            _lightController.UpdateLightState(1, true, 50);
        }

        if (Input.GetAxis(_inputLabel) < 0 && _lightController != null)
        {
            _lightController.UpdateLightState(0, false, 50);
            _lightController.UpdateLightState(1, false, 50);
        }
    } // End of Update
}

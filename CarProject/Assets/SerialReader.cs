using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System.Threading;

public class SerialReader : MonoBehaviour {

    private bool atStart = true;
    private SerialPort stream;

    public Thread thread;
    public Text temp;

    public int BaudRate = 9600;
    public string ComPort = "COM3";
    public string Message;
    public bool Running = true;

    // Use this for initialization
    void Start () {
        atStart = true;
        if (atStart) {
            atStart = false;
            
            try {
                stream = new SerialPort(ComPort, BaudRate);

                if (!stream.IsOpen) {
                    stream.Open();
                }

                thread = new Thread(() => {
                    while (Running) {
                        Message = stream.ReadLine();
                        stream.BaseStream.Flush();
                    }
                });
                thread.Start();
            }
            catch {
                Running = false;
                Message = "Could not Connect.";
            }
        }
	} // End of Start
	
	// Update is called once per frame
	void Update () {
        temp.text = Message;
    }

    void OnApplicationQuit() {
        Running = false; // Stop the thread
    }
}

using System;
using System.Collections.Generic;
using TermostatSimulator.Interfaces;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TermostatSimulator
{
    /// <summary>
    /// This class will be used to send and receive the data
    /// going to the XBox and comming from the XBox.
    /// </summary>
    class DataHandler
    {
        private DispatcherTimer _timer = new DispatcherTimer(); // The timer that prompts class to send events.

        private List<TextBox> _inputTextBoxes = new List<TextBox>(); // These will be sent to the Xbox
        private List<TextBox> _outputTextBoxes = new List<TextBox>(); // These com from the XBox

        private TextBlock _logger = null; // If not null, will write errors in it.

        private IClientService _clientService; // A client that can send messages.
        private IListenerService _listenerService; // A listener that receives messages.

        public DataHandler(IListenerService listenerService, IClientService clientService) {

            _clientService = clientService;
            _listenerService = listenerService;
            
            _listenerService.StartListener(UpdateOutputTextBoxesDuringEvents);

            _timer.Interval = TimeSpan.FromMilliseconds(5000); // Set the period of the events.
            _timer.Tick += sendData; // Attach handler

            _timer.Start();
        } // End of Constructor

        /// <summary>
        /// This will be executed every time the listener causes an exception.
        /// </summary>
        /// <param name="request">
        /// The message that the listener gets.
        /// </param>
        private async void UpdateOutputTextBoxesDuringEvents(string request)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    // Convert string to dictionary
                    var setValues = Decoder.StringDecoder.stringToDictionary(request);
                    for (int i = 0; i < _outputTextBoxes.Count; i++)
                    {
                        try { _outputTextBoxes[i].Text = setValues[i].ToString(); }
                        catch (Exception e) { Log("Error in SocketListener_ConnectionReceived() -> " + e.ToString()); }
                    }
                });
        } // End of UpdateOutputTextBoxesDuringEvents

        /// <summary>
        /// Clears the text and displays the message.
        /// Used to display errors if there is a textBlock provided.
        /// </summary>
        private void Log(string message) {
            // If there is no logger this will have no effect.
            if (_logger == null) return;

            _logger.Text = message + '\n';
        } // End of Log

        /// <summary>
        /// Read the text from all textBoxes and make a dictionary out of them.
        /// </summary>
        private Dictionary<int, double> readFromTextBoxes() {
            var data = new Dictionary<int, double>();

            // Go through the textboxes and convert their values to doubles.
            for (int i = 0; i <  _inputTextBoxes.Count; i++) {
                double value = 0.0;

                try { value = Convert.ToDouble(_inputTextBoxes[i].Text.Trim()); }
                catch { value = -273.15; }

                data.Add(i, value);
            }

            return data;
        } // End of readFromTextBoxes

        /// <summary>
        /// Increase or Decrease the actual temperature gradually.
        /// Serves no real purpose, just to have values that change.
        /// </summary>
        private void UpdateOutput() {
            try {
                for (int i = 0; i < _inputTextBoxes.Count; i++) {
                    double input = Convert.ToDouble(_inputTextBoxes[i].Text);
                    double output = Convert.ToDouble(_outputTextBoxes[i].Text);

                    _inputTextBoxes[i].Text = (input < output) ? "" + (input + 0.1) : _inputTextBoxes[i].Text;
                    _inputTextBoxes[i].Text = (input > output) ? "" + (input - 0.1) : _inputTextBoxes[i].Text;
                }
            }
            catch (Exception e) {
                Log("Error in UpdateOutput() -> " + e.ToString());
            }
        } // End of UpdateOutput

        /// <summary>
        /// This is an event handler called by the _timer DispatchTimer.
        /// This is called every few seconds, depending on _timeSpan.
        /// </summary>
        private void sendData(Object sender, Object eventOrWhatever)
        {
            UpdateOutput();

            // transform the UI data dictionary to a string.
            var message = Decoder.StringDecoder.dictionaryToString(readFromTextBoxes());
            Log("Sending : " + message);
            _clientService.TransmitString(message);
               
        } // End of sendData

        #region List_of_Setters_and_Getters

        public long Period {
            set { _timer.Interval = TimeSpan.FromMilliseconds(value);
            }
            get { return _timer.Interval.Milliseconds; }
        }

        public List<TextBox> InputTextBoxes {
            set { _inputTextBoxes = value; }
            get { return _inputTextBoxes; }
        }

        public List<TextBox> OutputTextBoxes {
            set { _outputTextBoxes = value; }
            get { return _outputTextBoxes; }
        }

        public TextBlock Logger {
            set { _logger = value; }
            get { return _logger; }
        }
        #endregion
    } // End of Class DataHandler
} // End of NameSpace TermostatSimulator

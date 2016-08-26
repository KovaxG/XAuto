using Mapsxbox.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Mapsxbox.DataTransmissions
{
    class DataHandler
    {
        private List<TextBlock> _outputs = new List<TextBlock>(); // A list of inputs
        private List<TextBox> _inputs = new List<TextBox>(); // A list of outputs

        private IClientService _clientService; // A client that can send messages.
        private IListenerService _listenerService; // A listener that receives messages.

        public DataHandler(IListenerService listenerService, IClientService clientService)
        {
            _clientService = clientService;
            _listenerService = listenerService;

            _listenerService.StartListener(ReceivedInformation);
        } // End of Constructor

        /// <summary>
        /// Calls a function to encode the information
        /// contained in the inputs, and sends it via the 
        /// lsitenerService.
        /// </summary>
        public void sendData()
        {
            string message = EncodeDataFromTextBoxes(_inputs);
            _clientService.TransmitString(message);
        }

        /// <summary>
        /// This method is called, when the Listener receives information. 
        /// The received message is the parameter of the function.
        /// This method changes the values of the outputs.
        /// </summary>
        public async void ReceivedInformation(string request)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                 {
                     var dictionary = StringDecoder.stringToDictionary(request);

                     dictionary.ToList().ForEach(pair => _outputs[pair.Key].Text = pair.Value.ToString());
                 });
        }

        /// <summary>
        /// Takes a list of textBoxes, try to convert each element into a double,
        /// if an element is not a double, skip it, and send the dictionary into
        /// the stringDecoder class, and return the result.
        /// </summary>
        public string EncodeDataFromTextBoxes(List<TextBox> textBoxes)
        {
            var dictionary = new Dictionary<int, double>();

            var values = textBoxes.Select(textBox => textBox.Text).ToList();

            for (int i = 0; i < values.Count; i++) {
                try {
                    double temperature = Convert.ToDouble(values[i]);
                    dictionary.Add(i, temperature);
                }
                catch { /* If text is not a double, ignore. */ }
            }

            return StringDecoder.dictionaryToString(dictionary);
        } // End of TakeInfoFromTextBoxes


        public List<TextBox> Inputs
        {
            set {  _inputs = value; }
            get { return _inputs; }
        }

        public List<TextBlock> Outputs
        {
            set { _outputs = value; }
            get { return _outputs; }
        }
    } // End of Class DataHandler
} // End of Namespace Mapsxbox.DataTransmissions

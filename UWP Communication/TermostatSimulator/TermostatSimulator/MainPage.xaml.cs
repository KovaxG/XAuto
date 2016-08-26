using System;
using TermostatSimulator.Classes.DataTransmissions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TermostatSimulator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DataHandler _dataHandler;

        public MainPage()
        {
            this.InitializeComponent();

            int initialSpeed = 2000;

            // REM should be able to modify while tunning. Maybe in the future.
            var hostIp = "172.27.94.18"; // You can change the IP here.
            var portNumber = 1235; // You can change the port here.

            var client = new TcpClient(hostIp, portNumber);
            var listener = new TcpListener(portNumber);

            _dataHandler = new DataHandler(listener, client);

            _dataHandler.Period = initialSpeed; // in milliseconds.    
            textRefreshTime.Text = initialSpeed.ToString();

            _dataHandler.InputTextBoxes.Add(this.temp0_text);
            _dataHandler.InputTextBoxes.Add(this.temp1_text);
            _dataHandler.InputTextBoxes.Add(this.temp2_text);
            _dataHandler.InputTextBoxes.Add(this.temp3_text);

            _dataHandler.OutputTextBoxes.Add(this.temp0_text_xbox);
            _dataHandler.OutputTextBoxes.Add(this.temp1_text_xbox);
            _dataHandler.OutputTextBoxes.Add(this.temp2_text_xbox);
            _dataHandler.OutputTextBoxes.Add(this.temp3_text_xbox);

            _dataHandler.Logger = this.errorBlock;

            ///hard-code input
            this.temp0_text.Text = "78.0";
            this.temp1_text.Text = "29.1";
            this.temp2_text.Text = "43.2";
            this.temp3_text.Text = "15.3";

            this.temp0_text_xbox.Text = "24.0";
            this.temp1_text_xbox.Text = "24.1";
            this.temp2_text_xbox.Text = "24.2";
            this.temp3_text_xbox.Text = "24.3";
            ///


        }
        private void textRefreshTime_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try {
                long millis = Convert.ToInt64(textRefreshTime.Text);
                _dataHandler.Period = millis;
            }
            catch { /* Exception not handled. */ }
        }

        
    }
}

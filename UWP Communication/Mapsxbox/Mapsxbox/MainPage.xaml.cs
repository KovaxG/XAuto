using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Mapsxbox.DataTransmissions;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Mapsxbox
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

            Uri uri = new Uri("ms-appx:///Assets/Map-petra.png");
            BitmapImage bmi = new BitmapImage(uri);

            this.backgroundImage.Source = bmi;

            var host = "172.27.94.20";
            var port = 1235;

            TcpClient client = new TcpClient(host, port);
            TcpListener listener = new TcpListener(port);

            _dataHandler = new DataHandler(listener, client);

            // A list of textboxes used to send data.
            _dataHandler.Inputs.Add(term0_text);
            _dataHandler.Inputs.Add(term1_text);
            _dataHandler.Inputs.Add(term2_text);
            _dataHandler.Inputs.Add(term3_text);
            _dataHandler.Inputs.Add(term4_text);
            _dataHandler.Inputs.Add(term5_text);
            _dataHandler.Inputs.Add(term6_text);
            _dataHandler.Inputs.Add(term7_text);
            _dataHandler.Inputs.Add(term8_text);
            _dataHandler.Inputs.Add(term9_text);
            _dataHandler.Inputs.Add(term10_text);
            _dataHandler.Inputs.Add(term11_text);
            _dataHandler.Inputs.Add(term12_text);

            // A list of textBlocks used to receive and show data.
            _dataHandler.Outputs.Add(cmr0);
            _dataHandler.Outputs.Add(cmr1);
            _dataHandler.Outputs.Add(cmr2);
            _dataHandler.Outputs.Add(cmr3);
            _dataHandler.Outputs.Add(cmr4);
            _dataHandler.Outputs.Add(cmr5);
            _dataHandler.Outputs.Add(cmr6);
            _dataHandler.Outputs.Add(cmr7);
            _dataHandler.Outputs.Add(cmr8);
            _dataHandler.Outputs.Add(cmr9);
            _dataHandler.Outputs.Add(cmr10);
            _dataHandler.Outputs.Add(cmr11);
            _dataHandler.Outputs.Add(cmr12);
        
            // Hardcoded values to fill the textBoxes
            term0_text.Text = "25.0";
            term1_text.Text = "25.1";
            term2_text.Text = "25.2";
            term3_text.Text = "25.3";
            term4_text.Text = "25.4";
            term5_text.Text = "25.5";
            term6_text.Text = "25.6";
            term7_text.Text = "25.7";
            term8_text.Text = "25.8";
            term9_text.Text = "25.9";
            term10_text.Text = "25.10";
            term11_text.Text = "25.11";
            term12_text.Text = "25.12";
        }

        /// <summary>
        /// When the textboxes loose focus, the value of the textBoxes are sent out.
        /// </summary>
        public void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _dataHandler.sendData();
        } // End of textBox_LostFocus

        #region Stuff_that_doesn't_do_anything
        private void textBlock_output_SelectionChanged(object sender, RoutedEventArgs e) { }
        private void cmr0_SelectionChanged(object sender, RoutedEventArgs e) { }
        private void cmr2_Copy9_SelectionChanged(object sender, RoutedEventArgs e) { }
        private void cmr2_Copy8_SelectionChanged(object sender, RoutedEventArgs e) { }
        private void cmr2_Copy3_SelectionChanged(object sender, RoutedEventArgs e) { }
        private void cmr2_Copy4_SelectionChanged(object sender, RoutedEventArgs e) { }
        private void cmr2_SelectionChanged(object sender, RoutedEventArgs e) { }
        private void term1_text_TextChanged(object sender, TextChangedEventArgs e) { }
        #endregion
    } // End of Class MainPage
} // End of Namespace Mapsxbox

 


       

           
    


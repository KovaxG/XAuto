using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Gaming.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Control_Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Gamepad controller; // Represents the actual controller

        private int numberOfControllers = 0;

        private DispatcherTimer timer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();
            button.Content = "Apasa-ma"; // Just for fun; ignore this

            // How many controllers are there?
            numberOfControllers = Gamepad.Gamepads.Count();

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(200);

            timer.Interval = timeSpan;
            timer.Tick += loop;
            timer.Start();

        }

        /// <summary>
        /// Prints the state of the controller to the screen for ever.
        /// </summary>
        private void loop(object sender, object e) {
            

            string message = "I don't see any controllers, do you? Nope.\nThere is no controller, the controller is a lie!";

            numberOfControllers = Gamepad.Gamepads.Count;

                if (Gamepad.Gamepads.Count > 0)
                {
                    controller = Gamepad.Gamepads.First();

                    GamepadReading r = controller.GetCurrentReading();

                    message = String.Format(" Number of controllers: {0}\n\n LeftThumbStick (X, Y): ({1}, {2})\nRightThumbStick (X, Y): ({3}, {4})\nLeftTrigger: {5}\nRightTrigger{6}",
                        numberOfControllers,
                        r.LeftThumbstickX,
                        r.LeftThumbstickY,
                        r.RightThumbstickX,
                        r.RightThumbstickY,
                        r.LeftTrigger,
                        r.RightTrigger);
                }

                textBox.Text = message;
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            button.Content = "Inca o data!";
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}


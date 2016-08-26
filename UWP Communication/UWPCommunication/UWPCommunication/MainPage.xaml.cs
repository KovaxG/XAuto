using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPCommunication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Call this method to print text to the screen.
        /// </summary>
        public void Log(string message) {
            textBox.Text += message + "\n";
        }
        public void ClearLog() {
            textBox.Text = "";
        }

        public MainPage()
        {
            this.InitializeComponent();
            ClearLog();

            
        }

        private string _portNumber = "1234"; // The free port.
        private string _hostName = "172.27.94.20";//"172.27.94.3"; // The host

        public async void StartClient() {
            try
            {
                //Create the StreamSocket and establish a connection to the echo server.
                StreamSocket socket = new StreamSocket();

                //The server hostname that we will be establishing a connection to. We will be running the server and client locally,
                //so we will use localhost as the hostname.
                HostName serverHost = new HostName(_hostName);

                //Every protocol typically has a standard port number. For example HTTP is typically 80, FTP is 20 and 21, etc.
                //For the echo server/client application we will use a random port 1337.
                await socket.ConnectAsync(serverHost, _portNumber);

                //Write data to the echo server.
                Stream streamOut = socket.OutputStream.AsStreamForWrite();
                StreamWriter writer = new StreamWriter(streamOut);
                string request = "1:10;2:5;3:1;";
                await writer.WriteLineAsync(request);
                await writer.FlushAsync();

                Log("Sent Succesfully!");

                /*
                //Read data from the echo server.
                Stream streamIn = socket.InputStream.AsStreamForRead();
                StreamReader reader = new StreamReader(streamIn);
                string response = await reader.ReadLineAsync();
                */
            }
            catch (Exception e)
            {
                //Handle exception here.     
                Log("Exception in StartClient -> " + e.ToString());       
            }

        }

        /// <summary>
        /// Start the connection.
        /// </summary>
        public async void StartListener()
        {
            Log("Port: " + _portNumber);

            try
            {
                //Create a StreamSocketListener to start listening for TCP connections.
                StreamSocketListener socketListener = new StreamSocketListener();

                //Hook up an event handler to call when connections are received.
                socketListener.ConnectionReceived += SocketListener_ConnectionReceived;

                //Start listening for incoming TCP connections on the specified port. You can specify any port that' s not currently in use.
                await socketListener.BindServiceNameAsync(_portNumber);
                
            }
            catch (Exception e)
            {
                //Handle exception.
                Log("Exception in StartListener() -> " + e.ToString());
            }
        }

        /// <summary>
        /// Event raised when a connection is received.
        /// </summary>
        public async void SocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args) {
            //Read line from the remote client.
            Stream inStream = args.Socket.InputStream.AsStreamForRead();
            StreamReader reader = new StreamReader(inStream);
            string request = await reader.ReadLineAsync();

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,() => this.textBox.Text += request + "\n");

            /*
            //Send the line back to the remote client.
            Stream outStream = args.Socket.OutputStream.AsStreamForWrite();
            StreamWriter writer = new StreamWriter(outStream);
            await writer.WriteLineAsync(request);
            await writer.FlushAsync();
            */
        }

        private void senderButton_Click(object sender, RoutedEventArgs e)
        {
            Log("Attempting to Send Text (as Client)...");
            StartClient();
        }

        private void receiverButton_Click(object sender, RoutedEventArgs e)
        {
            Log("Waiting to Receive Message (as Server)...");
            StartListener();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
        }
    }
}

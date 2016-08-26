using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TermostatSimulator.Interfaces;
using Windows.Networking.Sockets;

namespace TermostatSimulator.Classes.DataTransmissions
{
    class TcpListener : IListenerService
    {
        private string _portNumber; // The listener will listen to this port.
        private Action<string> _action; // When a connection is made this action will be called.

        public TcpListener(int port) {
            _portNumber = port.ToString();
        } // End of Constructor

        public async Task StartListener(Action<string> action)
        {
            _action = action;

            try
            {
                //Create a StreamSocketListener to start listening for TCP connections.
                StreamSocketListener socketListener = new StreamSocketListener();

                //Hook up an event handler to call when connections are received.
                socketListener.ConnectionReceived += SocketListener_ConnectionReceived;

                //Start listening for incoming TCP connections on the specified port. You can specify any port that' s not currently in use.
                await socketListener.BindServiceNameAsync(_portNumber);

            }
            catch 
            {
                Debug.WriteLine("Babamfasza");
                /* Exceptions Not Handled! */
            }
        }

        private async void SocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            //Read line from the remote client.
            Stream inStream = args.Socket.InputStream.AsStreamForRead();
            StreamReader reader = new StreamReader(inStream);
            string request = await reader.ReadLineAsync();

            _action(request);
        }
    }
}

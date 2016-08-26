using System;
using System.IO;
using System.Threading.Tasks;
using TermostatSimulator.Interfaces;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace TermostatSimulator.Classes.DataTransmissions
{
    class TcpClient : IClientService
    {
        private HostName _hostName;
        private string _portNumber;

        public TcpClient(string host, int port) {
            _hostName = new HostName(host);
            _portNumber = port.ToString();
        }

        public async Task TransmitString(string message)
        {
            try
            {
                //Create the StreamSocket and establish a connection to the echo server.
                StreamSocket socket = new StreamSocket();

                //Every protocol typically has a standard port number. For example HTTP is typically 80, FTP is 20 and 21, etc.
                //For the echo server/client application we will use a random port 1337.
                await socket.ConnectAsync(_hostName, _portNumber);

                //Write data to the echo server.
                Stream streamOut = socket.OutputStream.AsStreamForWrite();
                StreamWriter writer = new StreamWriter(streamOut);
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }
            catch
            {
                /* Not Yet Implemented! */
            }
        }
    }
}

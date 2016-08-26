using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

using Mapsxbox.Interfaces;

namespace Mapsxbox.DataTransmissions
{
    class TcpClient : IClientService
    {
        private HostName _hostName;
        private string _portNumber;

        public TcpClient(string host, int port) {
            _hostName = new HostName(host);
            _portNumber = port.ToString();
        } // End of Constructor

        public async Task<bool> TransmitString(string message)
        {
            try {
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

                return true;
            }
            catch  {
                return false;
            }
        } // End of TransmitString
    } // End of Class TcpClient
} // End of Namespace Mapsxbox.DataTransmissions

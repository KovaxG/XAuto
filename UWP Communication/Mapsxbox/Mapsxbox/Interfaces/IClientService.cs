using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapsxbox.Interfaces
{
    /// <summary>
    /// A service that allows one to send messages trough an ip adress.
    /// The TransmitString takes a message, and returns a bool that 
    /// signifies if the message was sent or not.
    /// </summary>
    interface IClientService
    {
        Task<bool> TransmitString(string message);
    }
}

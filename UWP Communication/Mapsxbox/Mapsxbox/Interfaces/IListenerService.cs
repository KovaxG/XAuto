using System;
using System.Threading.Tasks;

namespace Mapsxbox.Interfaces
{
    /// <summary>
    /// A service used to implement a listener that listens on a port.
    /// The StartListener method takes a Action<string>, the string
    /// that it receives as parameter when the function is called.
    /// The function is called when a message is sent.
    /// </summary>
    interface IListenerService
    {
        Task StartListener(Action<string> action);
    }
}

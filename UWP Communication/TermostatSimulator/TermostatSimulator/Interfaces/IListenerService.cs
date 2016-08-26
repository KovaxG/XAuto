using System;
using System.Threading.Tasks;

namespace TermostatSimulator.Interfaces
{
    interface IListenerService
    {
        Task StartListener(Action<string> action);
    }
}

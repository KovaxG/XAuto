using System.Threading.Tasks;

namespace TermostatSimulator.Interfaces
{
    interface IClientService
    {
        Task TransmitString(string message);
    }
}

using System.Threading.Tasks;

namespace GoDaddyDDNS.Abstractions
{
    internal interface IPublicIPTracker
    {
        Task<string> Get();
    }
}

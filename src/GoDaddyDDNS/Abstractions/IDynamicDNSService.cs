using System.Threading.Tasks;

namespace GoDaddyDDNS.Abstractions
{
    public interface IDynamicDNSService
    {
        Task Initialize();
        Task Execute();
    }
}

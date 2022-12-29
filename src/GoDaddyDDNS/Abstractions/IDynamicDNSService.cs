using System.Threading;
using System.Threading.Tasks;

namespace GoDaddyDDNS.Abstractions
{
    public interface IDynamicDNSService
    {
        Task Initialize(CancellationToken cancellationToken);
        Task Execute(CancellationToken cancellationToken);
    }
}

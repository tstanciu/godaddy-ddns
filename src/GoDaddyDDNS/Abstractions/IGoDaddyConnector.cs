using GoDaddyDDNS.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GoDaddyDDNS.Abstractions
{
    internal interface IGoDaddyConnector
    {
        Task<DNSRecord[]> GetRecords(CancellationToken cancellationToken);
        Task UpdateRecords(DNSRecord[] records, CancellationToken cancellationToken);
    }
}

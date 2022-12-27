using GoDaddyDDNS.Models;
using System.Threading.Tasks;

namespace GoDaddyDDNS.Abstractions
{
    internal interface IGoDaddyConnector
    {
        Task<DNSRecord[]> GetRecords();
        Task UpdateRecords(DNSRecord[] records);
    }
}

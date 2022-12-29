using GoDaddyDDNS.Abstractions;
using GoDaddyDDNS.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoDaddyDDNS.Services
{
    internal class DynamicDNSService : IDynamicDNSService
    {
        private DNSRecord[] DNSRecords;
        private string PublicIP;

        private readonly ILogger<DynamicDNSService> _logger;
        private readonly IPublicIPTracker _publicIPTracker;
        private readonly IGoDaddyConnector _goDaddyConnector;

        public DynamicDNSService(ILogger<DynamicDNSService> logger, IPublicIPTracker publicIPTracker, IGoDaddyConnector goDaddyConnector)
        {
            _logger=logger;
            _publicIPTracker=publicIPTracker;
            _goDaddyConnector=goDaddyConnector;
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            // check records
            var records = await _goDaddyConnector.GetRecords(cancellationToken);
            if (records == null || records.Length == 0)
                throw new Exception("No valid record found.");
            DNSRecords = records;
            PublicIP = records.First().Data;
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            var publicIP = await _publicIPTracker.Get(cancellationToken);
            if (PublicIP != publicIP)
            {
                _logger.LogWarning($"[{DateTime.Now}]: Public IP has changed from '{PublicIP}' to '{publicIP}'");
                var newRecords = DNSRecords.Select(record =>
                {
                    record.Data = publicIP;
                    return record;
                }).ToArray();

                await _goDaddyConnector.UpdateRecords(newRecords, cancellationToken);

                PublicIP = publicIP;
                DNSRecords = newRecords;
            }
            else
            {
                _logger.LogInformation($"[{DateTime.Now}]: Public IP has not changed.");
            }
        }
    }
}

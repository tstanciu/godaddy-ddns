using GoDaddyDDNS.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GoDaddyDDNS.Services
{
    internal class PublicIPTracker : IPublicIPTracker
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PublicIPTracker> _logger;
        private readonly string[] _publicIPProviders;

        public PublicIPTracker(HttpClient httpClient, ILogger<PublicIPTracker> logger, IConfiguration configuration)
        {
            _httpClient=httpClient;
            _logger=logger;
            _publicIPProviders = configuration.GetSection("PublicIPProviders").Get<string[]>();
        }

        public async Task<string> Get(CancellationToken cancellationToken)
        {
            if (_publicIPProviders == null || _publicIPProviders.Length == 0)
                throw new Exception("No public IP provider is configured.");

            foreach (var provider in _publicIPProviders)
            {
                var ip = await GetIP(provider, cancellationToken);
                if (ip != null)
                    return ip;
            }

            throw new Exception("The public IP could not be obtained from any of the configured providers.");
        }

        private async Task<string> GetIP(string providerUrl, CancellationToken cancellationToken)
        {
            string ip = null;
            using (var response = await _httpClient.GetAsync(providerUrl, cancellationToken).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                    ip = await response.Content.ReadAsStringAsync(cancellationToken);
                else
                    return null;
            }

            var valid = ValidateIP(ip);
            if (valid)
                return ip;

            return null;
        }

        private bool ValidateIP(string ip)
        {
            var valid = IPAddress.TryParse(ip, out var address);
            if (!valid)
            {
                _logger.LogWarning($"'{ip}' is not a valid IP adress.");
                return false;
            }

            switch (address.AddressFamily)
            {
                case System.Net.Sockets.AddressFamily.InterNetwork:
                    return true;

                case System.Net.Sockets.AddressFamily.InterNetworkV6:
                    _logger.LogError($"IPv6 is not implemented: '{ip}'");
                    return false;

                default:
                    _logger.LogError($"IP '{ip}' is valid but isn't IPv4 or IPv6.");
                    return false;
            }

        }
    }
}

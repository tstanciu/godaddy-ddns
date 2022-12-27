using GoDaddyDDNS.Abstractions;
using GoDaddyDDNS.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GoDaddyDDNS.Services
{
    internal class GoDaddyConnector : IGoDaddyConnector
    {
        private const string _recordType = "A";
        private const string _recordRoute = "/v1/domains/{domain}/records/{type}/{name}";

        private readonly ILogger<GoDaddyConnector> _logger;
        private readonly string _domain;
        private readonly string[] _records;
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GoDaddyConnector(ILogger<GoDaddyConnector> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            _domain = configuration.GetValue<string>("Domain");
            _records = configuration.GetSection("Records").Get<string[]>();

            var key = configuration.GetValue<string>("Key");
            var secret = configuration.GetValue<string>("Secret");
            _apiKey = $"sso-key {key}:{secret}";

            httpClient.BaseAddress = new Uri("https://api.godaddy.com");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", _apiKey);
            _httpClient = httpClient;
        }

        private string GetRecordRoute(string name) => _recordRoute.Replace("{domain}", _domain).Replace("{type}", _recordType).Replace("{name}", name);

        public async Task<DNSRecord[]> GetRecords()
        {
            var list = new List<DNSRecord>();
            foreach (var name in _records)
            {
                var dnsRecord = await GetRecord(name);
                if (dnsRecord == null)
                    _logger.LogWarning($"The record '{name}' was not found and will be ignored.");
                else
                    list.Add(dnsRecord);
            }
            return list.ToArray();
        }

        private async Task<DNSRecord> GetRecord(string name)
        {
            var route = GetRecordRoute(name);
            using (var response = await _httpClient.GetAsync(route))
            {
                var resultAsString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<DNSRecord[]>(resultAsString);
                    if (result.Length > 0)
                        return result.First();
                    else
                        return null;
                }
                else
                {
                    var error = JsonConvert.DeserializeObject<ErrorBody>(resultAsString);
                    throw new Exception($"{error.Code}: {error.Message}");
                }
            }
        }

        public async Task UpdateRecords(DNSRecord[] records)
        {
            foreach (var record in records)
                await UpdateRecord(record);
        }

        private async Task UpdateRecord(DNSRecord record)
        {
            var route = GetRecordRoute(record.Name);
            var data = JsonConvert.SerializeObject(new DNSRecord[] { record }, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            using (var response = await _httpClient.PutAsync(route, content))
            {
                if (response.IsSuccessStatusCode)
                    return;
                
                var resultAsString = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorBody>(resultAsString);
                throw new Exception($"{error.Code}: {error.Message}");
            };
        }
    }
}

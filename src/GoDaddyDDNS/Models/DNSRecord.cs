namespace GoDaddyDDNS.Models
{
    internal class DNSRecord
    {
        public string Name { get; set; }
        public string Data { get; set; }
        public int Ttl { get; set; }
        public string Type { get; set; }
    }
}

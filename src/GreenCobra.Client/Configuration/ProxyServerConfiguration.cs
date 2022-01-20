using System.Net;
using System.Text.Json.Serialization;

namespace GreenCobra.Client.Configuration
{
    public sealed class ProxyServerConfiguration
    {
        [JsonPropertyName("id")]
        public string DomainName { get; init; }

        [JsonPropertyName("max_conn_count")]
        public int ConnectionLimit { get; init; }

        [JsonPropertyName("port")]
        public int Port { get; init; }

        [JsonPropertyName("url")]
        public Uri Url { get; init; }

        [JsonIgnore]
        public IPEndPoint IpEndPoint { get; set; }

        public override string ToString()
        {
            return $"ID: {DomainName}; MaxConnections: {ConnectionLimit}; " +
                   $"Server Url: {Url}; Server Port: {Port};";
        }
    }
}
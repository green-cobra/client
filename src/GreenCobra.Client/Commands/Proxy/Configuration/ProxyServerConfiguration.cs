using System.Text.Json.Serialization;

namespace GreenCobra.Client.Commands.Proxy.Configuration;

public record ProxyConnectionConfiguration(
    [property: JsonPropertyName("id")] string Domain,
    [property: JsonPropertyName("max_conn_count")] int MaxConnections,
    [property: JsonPropertyName("port")] int ServerPort,
    [property: JsonPropertyName("url")] Uri ServerUrl);
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using GreenCobra.Common;

namespace GreenCobra.Client.Services.ServerCommunication.Models;

public record ProxyPointSetupResponse(
    [property: JsonPropertyName("id")] string DistributedDomain,
    [property: JsonPropertyName("max_conn_count")] int ParallelConnections,
    [property: JsonPropertyName("port")] int ProxyPointPort,
    [property: JsonPropertyName("url")] Uri ProxyPointHost)
{
    // todo: fix as this is not called by JsonDeserializer
    [OnDeserialized]
    private void ValidateDeserialization()
    {
        Guard.AgainstNull(ProxyPointHost);
        Guard.AgainstNullOrEmpty(DistributedDomain);
        Guard.Satisfy(ProxyPointPort != 0);
        Guard.Satisfy(ParallelConnections > 0);
    }
};

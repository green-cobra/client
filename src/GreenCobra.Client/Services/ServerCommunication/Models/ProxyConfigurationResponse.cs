using System.Net;
using System.Text.Json.Serialization;

namespace GreenCobra.Client.Services.ServerCommunication.Models;

public record ProxyConfigurationResponse(
    [property: JsonPropertyName("id")] string Domain,
    [property: JsonPropertyName("max_conn_count")] int MaxConnections,
    [property: JsonPropertyName("port")] int ServerPort,
    [property: JsonPropertyName("url")] Uri ServerHost,
    [property: JsonIgnore] EndPoint? ServerEndPoint)
{
    public void Deconstruct(out EndPoint? serverEndPoint, out int maxConnections)
    {
        serverEndPoint = ServerEndPoint;
        maxConnections = MaxConnections;
    }
}
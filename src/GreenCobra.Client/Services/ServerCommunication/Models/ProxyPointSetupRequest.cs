using System.Text.Json.Serialization;

namespace GreenCobra.Client.Services.ServerCommunication.Models;

public record ProxyPointSetupRequest(
    [property: JsonIgnore] Uri SetupUrl,
    [property: JsonPropertyName("name")] string DesiredDomain);

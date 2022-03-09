using System.Text.Json.Serialization;

namespace GreenCobra.Client.Services.ServerCommunication.Models;

public record ProxyConfigurationRequest(
    [property: JsonIgnore] Uri ServerUrl,
    [property: JsonPropertyName("name")] string DomainRequest);

﻿using System.Net;
using System.Text.Json.Serialization;

namespace GreenCobra.Client.Services.ServerCommunication.Models;

public record ProxyServerConfigurationDto(
    [property: JsonPropertyName("id")] string Domain,
    [property: JsonPropertyName("max_conn_count")] int MaxConnections,
    [property: JsonPropertyName("port")] int ServerPort,
    [property: JsonPropertyName("url")] Uri ServerUrl)
{
    [JsonIgnore] public EndPoint? ServerEndPoint { get; set; }
};
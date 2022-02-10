using GreenCobra.Client.Services.ServerCommunication.Models;

namespace GreenCobra.Client.Logging.States;

public record ProxyServerConfigurationConstructedState(
    ProxyServerConfigurationDto ServerConfiguration) : ILoggerState
{
    public string Format() =>
        $"Received Domain    : {ServerConfiguration.Domain}\r\n" +
        $"Application URL    : {ServerConfiguration.ServerUrl}\r\n" +
        $"Server end point   : {ServerConfiguration.ServerEndPoint}";
}
using System.Text;
using GreenCobra.Client.Commands.Proxy.Configuration;
using GreenCobra.Client.Logging.States.Interfaces;
using GreenCobra.Client.Proxy;

namespace GreenCobra.Client.Logging.States;

public record TaskProxiedDataState(ProxyConfiguration ProxyConfiguration, ProxyResult ProxyResult)
    : IState, IStateFormatter<TaskProxiedDataState>
{
    public LoggingEventId EventId => LoggingEventId.DataProxied;

    public Func<TaskProxiedDataState, Exception?, string> Formatter { get; } = (state, ex) =>
    {
        var requestHeaders = ConvertBytesToString(state.ProxyResult.ServerMessageHeading);
        var responseHeaders = ConvertBytesToString(state.ProxyResult.ClientMessageHeading);


        return $"Proxied: {state.ProxyConfiguration.ServerEndPoint} <=====> {state.ProxyConfiguration.ClientEndPoint} \r\n" +
               $" - Request : {ParseHttp(requestHeaders)}\r\n" +
               $" - Response: {ParseHttp(responseHeaders)}\r\n";
    };

    private static string ConvertBytesToString(byte[]? bytes)
    {
        var dataAsString = Encoding.UTF8.GetString(bytes);

        var httpHeaders = dataAsString
            .Split("\r\n\r\n")
            .First();

        return httpHeaders;
    }

    // todo: temp solution
    private static string ParseHttp(string flatHeaders)
    {
        const string methodKeyName = "method";
        const string requestUrlKeyName = "Request URL";
        const string hostKeyName = "host";

        
        var headers = flatHeaders
            .Split("\r\n")
            .Select(x => x.Split(':', 2))
            .ToDictionary(
                strings => strings.Length < 2 ? methodKeyName : strings[0],
                strings => strings.Length < 2 ? strings[0] : strings[1]);

        //headers.TryGetValue(hostKeyName, out var host);
        //headers.TryGetValue(methodKeyName, out var method);
        //headers.TryGetValue(hostKeyName, out var host);

        return $"";
    }
}
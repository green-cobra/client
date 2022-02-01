using System.Net;
using System.Text;

namespace GreenCobra.Client.Logging.States;

public class TaskProxiedDataState : IState, IStateFormatter<TaskProxiedDataState>
{
    public LoggingEventId EventId { get; set; } = LoggingEventId.DataProxied;

    public EndPoint From { get; set; }
    public EndPoint To { get; set; }
    //public string RequestUrl { get; set; }
    public byte[] RequestData { get; set; }
    public byte[] ResponseData { get; set; }
    //public int Length => Data.Length;
    //public int CorrelationId { get; set; }

    //public Func<TaskProxiedDataState, Exception?, string>? Formatter { get; } = (state, exception) =>
    //    $"{{Request ID: {state.CorrelationId, 5}}} " +
    //    $"Proxied: {state.From, 25} =====> {state.To, 25} " +
    //    $"{{{state.Length, 10} bytes}}\r\n";
    public Func<TaskProxiedDataState, Exception?, string>? Formatter { get; } = (state, exception) =>
    {
        var requestHeaders = ConvertBytesToString(state.RequestData);
        var responseHeaders = ConvertBytesToString(state.ResponseData);


        return $"Proxied: {state.From} <=====> {state.To} \r\n" +
               $" - Request : {ParseHttp(requestHeaders)}\r\n" +
               $" - Response: {ParseHttp(responseHeaders)}\r\n";
    };

    private static string ConvertBytesToString(byte[] bytes)
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
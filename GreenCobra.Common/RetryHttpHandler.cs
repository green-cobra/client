namespace GreenCobra.Common;

public class RetryHttpHandler : DelegatingHandler
{
    private readonly int _maxRetries = 3;

    public RetryHttpHandler() : base(new HttpClientHandler()) { }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        for (int i = 0; i < _maxRetries; i++)
        {
            if (response.IsSuccessStatusCode)
                return response;

            // todo: add logic for some status codes e.g. Network connection failed
        }

        return response;
    }
}
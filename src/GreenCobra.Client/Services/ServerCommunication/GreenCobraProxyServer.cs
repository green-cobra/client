using System.Net.Http.Json;
using System.Text.Json;
using GreenCobra.Client.Services.ServerCommunication.Models;
using GreenCobra.Common;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Services.ServerCommunication;

public class GreenCobraProxyServer
{
     private readonly ILogger<GreenCobraProxyServer> _logger;
     private readonly HttpClient _httpClient;
     public GreenCobraProxyServer(ILogger<GreenCobraProxyServer> logger, HttpClient httpClient)
     {
         Guard.AgainstNull(logger);
         Guard.AgainstNull(httpClient);
         _logger = logger;
         _httpClient = httpClient;
     }

    public async Task<ProxyPointSetupResponse> SetupProxyPointAsync(ProxyPointSetupRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(request));
            var response = await _httpClient.PostAsync(request.SetupUrl, content, cancellationToken);
        
            _logger.LogDebug(Resources.Traces.SetupProxyPoint_ServerResponse, request, $"{(int)response.StatusCode} - {response.ReasonPhrase}");
        
            response.EnsureSuccessStatusCode();
            var proxyPoint = await response.Content.ReadFromJsonAsync<ProxyPointSetupResponse>(cancellationToken: cancellationToken);

            _logger.LogDebug(Resources.Traces.SetupProxyPoint_ParsedServerResponse, proxyPoint);
        
            Guard.AgainstNull(proxyPoint);
            return proxyPoint;
        }
        catch (Exception ex)
        {
            if (ex is HttpRequestException or InvalidOperationException)
                _logger.LogCritical(Resources.Errors.ProxyServerInvalidResponse, ex.Message, request);

            if (ex is ArgumentNullException)
                _logger.LogCritical(Resources.Errors.ProxyServerInvalidResponse, null, request);
                
            throw;
        }
    }
}
using System.Net;
using GreenCobra.Client.Proxy.Abstract;

namespace GreenCobra.Client.Proxy;

public class ProxyConnection : IDisposable
{
    public readonly Guid Id = Guid.NewGuid();

    private readonly ProxyStream _serverStream;
    private readonly ProxyStream _clientStream;

    public ProxyConnection(IPEndPoint serverEndPoint, IPEndPoint clientEndPoint)
    {
        var dumbLogger = DumbProxyLogger.Default;
        _serverStream = new ProxyStream(serverEndPoint, dumbLogger);
        _clientStream = new ProxyStream(clientEndPoint, dumbLogger);
    }

    public async Task ProxyAsync()
    {
        var serverToClient = _serverStream.CopyAsync(_clientStream);
        var clientToServer = _clientStream.CopyAsync(_serverStream);

        await Task.WhenAll(serverToClient, clientToServer);
    }

    public void Dispose()
    {
        _serverStream.Dispose();
        _clientStream.Dispose();
    }
}
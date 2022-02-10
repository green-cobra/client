using System.Net;
using GreenCobra.Common;

namespace GreenCobra.Proxy;

public class ProxyConnection
{
    private readonly EndPoint _clientEndPoint;
    private readonly EndPoint _serverEndPoint;

    public ProxyConnection(EndPoint clientEndPoint, EndPoint serverEndPoint)
    {
        Guard.AgainstNull(serverEndPoint);
        Guard.AgainstNull(clientEndPoint);

        _clientEndPoint = clientEndPoint;
        _serverEndPoint = serverEndPoint;
    }

    public async Task<ProxyResult> ProxyAsync(CancellationToken cancellationToken)
    {
        using var clientStream = new ProxyStream(_clientEndPoint);
        using var serverStream = new ProxyStream(_serverEndPoint);

        var serverToClientTask = serverStream.CopyAsync(clientStream, cancellationToken);
        var clientToServerTask = clientStream.CopyAsync(serverStream, cancellationToken);

        await Task.WhenAll(serverToClientTask, clientToServerTask);

        return new ProxyResult(await serverToClientTask, await clientToServerTask);
    }
}
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using GreenCobra.Client.Logging;

namespace GreenCobra.Client.Proxy;

public class ProxyStream : IDisposable
{
    private readonly Socket _socket;
    private readonly IPEndPoint _endPoint;

    private readonly ILoggerAdapter<ProxyStream, byte[]> _logger;

    public ProxyStream(IPEndPoint endPoint, ILoggerAdapter<ProxyStream, byte[]> logger)
    {
        _endPoint = endPoint;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task CopyAsync(ProxyStream destination, CancellationToken cancellationToken)
    {
        await ConnectIfNotConnectedAsync();
        
        int bufferSize = 64 * 1024;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

        try
        {
            int bytesRead;
            while ((bytesRead = await ReadAsync(buffer, cancellationToken)) != 0)
            {
                //_logger.LogInformation(buffer[..bytesRead]);
                
                _logger.LogInformation(EventIds.DataProxied, buffer[..bytesRead]);
                
                await destination.WriteAsync(buffer[..bytesRead], cancellationToken);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private async Task ConnectIfNotConnectedAsync()
    {
        if (!_socket.Connected)
            await _socket.ConnectAsync(_endPoint);
    }

    public async Task<int> WriteAsync(byte[] buffer, CancellationToken cancellationToken)
    {
        await ConnectIfNotConnectedAsync();

        var sendCount = await _socket.SendAsync(buffer, SocketFlags.None, cancellationToken);

        return sendCount;
    }

    public async Task<int> ReadAsync(byte[] buffer, CancellationToken cancellationToken)
    {
        await ConnectIfNotConnectedAsync();

        var receivedCount = await _socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);

        return receivedCount;
    }

    public void Dispose()
    {
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
        _socket.Dispose();
    }
}
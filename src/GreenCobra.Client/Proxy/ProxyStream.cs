using System.Buffers;
using System.Net;
using System.Net.Sockets;

namespace GreenCobra.Client.Proxy;

// todo: inherit ProxyStream from Stream
public class ProxyStream : IDisposable
{
    public EndPoint? DestinationEndPoint => _socket.RemoteEndPoint;

    private readonly Socket _socket;
    private readonly IPEndPoint _endPoint;

    private readonly int _defaultBufferSize = 32 * 1024;

    public ProxyStream(IPEndPoint endPoint)
    {
        _endPoint = endPoint;
        _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task<byte[]> CopyAsync(ProxyStream destination, CancellationToken cancellationToken)
    {
        await ConnectIfNotConnectedAsync();

        byte[]? messageHeading = null;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(_defaultBufferSize);
        try
        {
            int bytesRead;
            while ((bytesRead = await ReadAsync(buffer, cancellationToken)) != 0)
            {
                var valuableBytes = buffer[..bytesRead];
                await destination.WriteAsync(valuableBytes, cancellationToken);

                messageHeading ??= valuableBytes;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        return messageHeading;
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
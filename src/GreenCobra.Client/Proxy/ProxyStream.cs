using System.Buffers;
using System.Net;
using System.Net.Sockets;
using GreenCobra.Client.Logging;
using GreenCobra.Client.Logging.States;

namespace GreenCobra.Client.Proxy;

public class ProxyStream : IDisposable
{
    public EndPoint? DestinationEndPoint => _socket.RemoteEndPoint;

    private readonly Socket _socket;
    private readonly IPEndPoint _endPoint;
    
    private readonly ILoggerAdapter<ProxyStream>? _logger;

    public ProxyStream(IPEndPoint endPoint) : this(endPoint, null) { }

    public ProxyStream(IPEndPoint endPoint, ILoggerAdapter<ProxyStream>? logger)
    {
        _endPoint = endPoint;
        _logger = logger;  
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
                var valuableBytes = buffer[..bytesRead];
                await destination.WriteAsync(valuableBytes, cancellationToken);

                _logger?.LogInformation(new ProxyStreamState
                {
                    EventId = LoggingEventId.ProxyStream_DataProxied,
                    Data = valuableBytes,
                    From = _socket.RemoteEndPoint,
                    To = destination.DestinationEndPoint,
                    //TaskId = Task.CurrentId.Value
                });
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
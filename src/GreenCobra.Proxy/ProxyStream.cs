using System.Buffers;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using GreenCobra.Common;

namespace GreenCobra.Proxy;

// todo: inherit ProxyStream from Stream
public class ProxyStream : IDisposable
{
    private readonly Socket _socket;
    private readonly EndPoint _endPoint;

    private const int DefaultBufferSize = 32 * 1024;

    public ProxyStream(EndPoint endPoint)
    {
        Guard.AgainstNull(endPoint);

        _endPoint = endPoint;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task<byte[]?> CopyAsync(ProxyStream destination, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(destination);
        await ConnectIfNotConnectedAsync();

        byte[]? messageHeading = null;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(DefaultBufferSize);
        try
        {
            int bytesRead;
            while ((bytesRead = await ReadAsync(buffer, cancellationToken)) != 0)
            {
                var valuableBytes = buffer[..bytesRead];

                void CrackHttps(byte[] bytes)
                {
                    Console.WriteLine("Bytes:");
                    foreach (var b in bytes)
                    {
                        Console.Write(b);   
                    }
                    Console.WriteLine("");
                    Console.WriteLine("----------------");

                    // var headers = flatHeaders
                    //     .Split("\r\n")
                    //     .Select(x => x.Split(':', 2))
                    //     .ToDictionary(
                    //         strings => strings.Length < 2 ? methodKeyName : strings[0],
                    //         strings => strings.Length < 2 ? strings[0] : strings[1]);
                }


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
        Guard.AgainstNull(buffer);
        await ConnectIfNotConnectedAsync();

        var sendCount = await _socket.SendAsync(buffer, SocketFlags.None, cancellationToken);

        return sendCount;
    }

    public async Task<int> ReadAsync(byte[] buffer, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(buffer);
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
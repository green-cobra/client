using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GreenCobra.Client.ProxyStream;

public class ProxyStream : IDisposable
{
    public readonly Guid Id = Guid.NewGuid();
    private readonly ProxyStreamType _type;
    
    private readonly Socket _socket;
    private readonly IPEndPoint _endPoint;

    public ProxyStream(IPEndPoint endPoint, ProxyStreamType type)
    {
        _endPoint = endPoint;
        _type = type;
        _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task CopyAsync(ProxyStream destination)
    {
        await ConnectIfNotConnectedAsync();

        int bufferSize = 64 * 1024;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

        try
        {
            int bytesRead;
            while ((bytesRead = await ReadAsync(buffer)) != 0)
            {
                LogCopyToConsole(buffer[..bytesRead]);
                
                await destination.WriteAsync(buffer[..bytesRead]);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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

    public async Task<int> WriteAsync(byte[] buffer)
    {
        await ConnectIfNotConnectedAsync();

        var sendCount = await _socket.SendAsync(buffer, SocketFlags.None);

        return sendCount;
    }

    public async Task<int> ReadAsync(byte[] buffer)
    {
        await ConnectIfNotConnectedAsync();

        var receivedCount = await _socket.ReceiveAsync(buffer, SocketFlags.None);

        return receivedCount;
    }

    public void LogCopyToConsole(byte[] data)
    {
        var delimiter = new string('-', 50);

        Console.WriteLine();
        Console.WriteLine(delimiter);
        Console.WriteLine($"Data length: {data.Length}");
        Console.WriteLine(delimiter);

        var str = Encoding.UTF8.GetString(data);
        var segment = str.Split("\r\n\r\n");

        Console.WriteLine(segment[0]);
        Console.WriteLine(delimiter);
    }

    public void Dispose()
    {
        //$"Proxy Stream '{Type}' Disposed".WriteInfoLine();

        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
        _socket.Dispose();
    }
}
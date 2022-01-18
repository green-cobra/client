using System.Buffers;
using System.Net;
using System.Net.Sockets;

namespace GreenCobra.ConsoleClient.ProxyStream;

public class ProxyStream : IDisposable
{
    public Guid Id = Guid.NewGuid();
    
    private readonly Socket _socket;
    private readonly IPEndPoint _endPoint;

    public ProxyStream(IPEndPoint endPoint)
    {
        _endPoint = endPoint;
        _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task CopyAsync(ProxyStream destination)
    {
        await ConnectIfNotConnected();

        int bufferSize = 64 * 1024;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

        try
        {
            int bytesRead;
            while ((bytesRead = await ReadAsync(buffer)) != 0)
            {
                //LogCopyToConsole(destination, buffer);
                
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

    private async Task ConnectIfNotConnected()
    {
        if (!_socket.Connected)
            await _socket.ConnectAsync(_endPoint);
    }

    public async Task<int> WriteAsync(byte[] buffer)
    {
        await ConnectIfNotConnected();

        var sendCount = await _socket.SendAsync(buffer, SocketFlags.None);

        return sendCount;
    }

    public async Task<int> ReadAsync(byte[] buffer)
    {
        await ConnectIfNotConnected();

        var receivedCount = await _socket.ReceiveAsync(buffer, SocketFlags.None);

        return receivedCount;
    }

    //public void LogCopyToConsole(ProxyStream destination, byte[] data)
    //{
    //    //$"{Type} --> {destination.Type}".WriteInfoLine();
    //    //Encoding.UTF8.GetString(data).WriteSuccessLine();
    //}

    public void Dispose()
    {
        //$"Proxy Stream '{Type}' Disposed".WriteInfoLine();

        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
        _socket.Dispose();
    }
}
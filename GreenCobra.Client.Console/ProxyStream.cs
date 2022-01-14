using System.Buffers;
using System.Net;
using System.Net.Sockets;

namespace GreenCobra.Client.Console;

public class ProxyStream : IDisposable
{
    public Socket _socket;
    //private IPEndPoint _endPoint;
    public Guid Id = Guid.NewGuid();

    public ProxyStreamType Type;

    public ProxyStream(IPEndPoint endPoint, ProxyStreamType type)
    {
        Type = type;
        //_endPoint = endPoint;

        _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect(endPoint); // todo: Move to Connect()
    }

    public async Task CopyAsync(ProxyStream destination)
    {
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
            LogException(e);
            throw;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public async Task<int> WriteAsync(byte[] buffer)
    {
        var sendCount = await _socket.SendAsync(buffer, SocketFlags.None);

        return sendCount;
    }

    public async Task<int> ReadAsync(byte[] buffer)
    {
        var receivedCount = await _socket.ReceiveAsync(buffer, SocketFlags.None);

        return receivedCount;
    }

    private void LogException(Exception e)
    {
        //$"{Type}:".WriteInfoLine();
        System.Console.WriteLine(e);
    }

    public void LogCopyToConsole(ProxyStream destination, byte[] data)
    {
        //$"{Type} --> {destination.Type}".WriteInfoLine();
        //Encoding.UTF8.GetString(data).WriteSuccessLine();
    }

    public void Dispose()
    {
        //$"Proxy Stream '{Type}' Disposed".WriteInfoLine();

        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
        _socket.Dispose();
    }
}
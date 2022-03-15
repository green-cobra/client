using System.Net;
using System.Net.Sockets;

namespace GreenCobra.Client.Helpers;

public static class DnsNameResolver
{
    public static IPAddress GetIpAddress(string hostNameOrAddress, CancellationToken cancellationToken) =>
        GetIpAddressAsync(hostNameOrAddress, cancellationToken).GetAwaiter().GetResult();
    
    public static async Task<IPAddress> GetIpAddressAsync(string hostNameOrAddress, CancellationToken cancellationToken)
    {
        var hostEntry = await Dns.GetHostEntryAsync(hostNameOrAddress, AddressFamily.InterNetwork, cancellationToken);

        return hostEntry.AddressList.First();
    }
}
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace GreenCobra.Client.Helpers;

public static class DnsNameResolver
{
    public static async Task<IPAddress> GetIpAddressAsync(string hostNameOrAddress, ILogger logger,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug(Resources.Traces.DnsAddress_Resolving, hostNameOrAddress);
        
            var address = await GetIpAddressAsync(hostNameOrAddress, cancellationToken);
        
            logger.LogDebug(Resources.Traces.DnsAddress_Resolved, hostNameOrAddress, address);
            return address;
        }
        catch (Exception e)
        {
            logger.LogCritical(Resources.Errors.DnsAddressResolutionFailed, e.Message, hostNameOrAddress);
            throw;
        }
    }

    public static async Task<IPAddress> GetIpAddressAsync(string hostNameOrAddress,
        CancellationToken cancellationToken = default)
    {
        var hostEntry = await Dns.GetHostEntryAsync(hostNameOrAddress, AddressFamily.InterNetwork, cancellationToken);
        return hostEntry.AddressList.First();
    }
}
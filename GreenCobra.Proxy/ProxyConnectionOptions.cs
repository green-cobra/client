using System.Net;

namespace GreenCobra.Proxy;

public record ProxyConnectionOptions(EndPoint ServerEndPoint, EndPoint LocalEndPoint, int ParallelDegree);
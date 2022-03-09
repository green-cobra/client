using System.Net;

namespace GreenCobra.Proxy;

public record ProxyConfiguration(EndPoint ServerEndPoint, EndPoint LocalEndPoint, int ParallelDegree);
namespace GreenCobra.Client;

public static class Resources
{
    public static readonly string ProxyCommand_Name = "proxy";
    public static readonly string ProxyCommand_Description = "Proxy requests from public server to your locally running application";
    
    public static class Traces
    {
        public static readonly string ProxyHandler_Enter = @"Entered ProxyHandler() ...";
        public static readonly string SetupProxyPoint_ServerResponse = 
@"Proxy Point setup :
Request Content : {request}
Response        : {response}";

        public static readonly string SetupProxyPoint_ParsedServerResponse = @"Response content: {content}";

        public static readonly string DnsAddress_Resolving = @"Resolving {hostName} ...";
        public static readonly string DnsAddress_Resolved = @"{hostName} Resolved to {address}";
    }
    
    public static class Logs
    {
        public static readonly string DefaultConfig_Loaded = @"Configuration loaded : {config}";
        public static readonly string DefaultConfig_FileCreation = @"Default configuration file is not found in the system. Creating new one, with default config : {config}";
        public static readonly string DefaultConfig_FileCreationDone = @"configuration file created. Path : {filePath}";
        
        public static readonly string ProxyCommandInputParsed = 
@"Input parameters  :
Local Host          : {localHost}
Local Port          : {localPort}
Proxy Server        : {serverUrl}
Desired Domain      : {desiredDomain}";
        // todo: rename
        public static readonly string ProxyPointSetupDone = 
@"Proxy Point setup done :
Distributed Domain       : {distributedDomain}
Parallel Connections     : {parallelConn}
Proxy Point Host         : {ppHost}
Proxy Point Port         : {ppPort}";

        public static readonly string ProxyEndpointsResolved = @"
Proxy Point : {ppEndPoint}
Local App   : {localEndPoint}";

        public static readonly string ProxyConnectionSetupDone = @"Proxy connection from {from} to {to}, created";

        public static readonly string ProxyParallelConnectionStarted = @"Proxy started, parallel connections {connCount}";

        public static readonly string ProxyTaskCompleted = @"Proxy task completed. Status: {status}";
        
//         public static readonly string ProxyTaskCompleted = @"
// Request Headers  : {reqHeaders}
// Response Headers : {resHeaders}";
    }
    
    public static class Errors
    {
        public static readonly string ProxyServerInvalidResponse = 
@"Proxy Point setup failed
Response message : {response}
Configuration    : {proxyConfig}";

        public static readonly string DnsAddressResolutionFailed = 
@"Address resolution failed
Message : {message} 
Host    : {host}";
    }

    public static class Option
    {
        public static readonly string LocalHostOrAddress_LongName = "--local-host";
        public static readonly string LocalHostOrAddress_ShortName = "-h";
        public static readonly string LocalHostOrAddress_Description = "Specifies local application host name or address";
        
        public static readonly string LocalPort_LongName = "--local-port";
        public static readonly string LocalPort_ShortName = "-p";
        public static readonly string LocalPort_Description = "Specifies local application port";
        
        public static readonly string ServerUrl_LongName = "--server-url";
        public static readonly string ServerUrl_ShortName = "-s";
        public static readonly string ServerUrl_Description = "Specifies server base url, that is responsible for proxy-url distribution";
        
        public static readonly string DesiredDomain_LongName = "--domain";
        public static readonly string DesiredDomain_ShortName = "-d";
        public static readonly string DesiredDomain_Description = "Specifies domain name which server will distribute to your application, if it will be available";
    }
}

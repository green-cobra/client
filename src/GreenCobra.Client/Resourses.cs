namespace GreenCobra.Client;

public static class Resources
{
    public static readonly string ProxyCommand_Name = "proxy";
    public static readonly string ProxyCommand_Description = "Proxy requests from public server to your locally running application";
    
    public static class Messages
    {
        public static readonly string ProxyCommandInputParsed = @"Local Host:     {localHost}
Local Port:     {localPort}
Proxy Server:   {serverUrl}
Desired Domain: {desiredDomain}";
        public static readonly string ProxyOptionsRetrieved = @"Client end point: {clientEndPoint}
Maximum connections: {maxConn}";
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

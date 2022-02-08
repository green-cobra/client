namespace GreenCobra.Client.Services.Configuration.Models;

public record ProxyCommandOptions(
    string LocalHost, 
    int LocalPort, 
    Uri ServerUrl, 
    string ServerDomainRequest);
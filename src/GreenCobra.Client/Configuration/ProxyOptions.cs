namespace GreenCobra.Client.Configuration;

public record ProxyOptions
{
    public string LocalHost { get; set; } = null!;
    public int LocalPort { get; set; }
    public Uri ServerUrl { get; set; } = null!;
    public string ServerDomainRequest { get; set; } = null!;
}
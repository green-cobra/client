using System.Text.Json;
using GreenCobra.Client.Services.Configuration.Models;

namespace GreenCobra.Client.Services.Configuration;

public class ConfigurationDefaultService
{
    private const string ConfigFolderName = "green-cobra";
    private const string ConfigFileName = "config.json";

    private static readonly ProxyCommandOptions ProxyDefaults = new("127.0.0.1", 80, new Uri("https://localtunnel.me/"), "?new");

    private readonly string _configurationFolderPath;
    private readonly string _configurationFilePath;

    public ConfigurationDefaultService()
    {
        var platformId = Environment.OSVersion.Platform;
        Environment.SpecialFolder configFolder;
        switch (platformId)
        {
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.Win32NT:
            case PlatformID.WinCE:
                configFolder = Environment.SpecialFolder.LocalApplicationData;
                break;
            case PlatformID.Unix:
            case PlatformID.MacOSX:
                configFolder = Environment.SpecialFolder.ApplicationData;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _configurationFolderPath = Path.Combine(Environment.GetFolderPath(configFolder), ConfigFolderName);
        _configurationFilePath = Path.Combine(_configurationFolderPath, ConfigFileName);
    }

    public ProxyCommandOptions GetProxyDefaults() => CreateDefaultConfig() ? ProxyDefaults : ReadFromConfigFile();

    private ProxyCommandOptions ReadFromConfigFile()
    {
        var configString = File.ReadAllText(_configurationFilePath);
        return JsonSerializer.Deserialize<ProxyCommandOptions>(configString)!;
    }

    private bool CreateDefaultConfig()
    {
        if (File.Exists(_configurationFilePath))
            return false;

        if (!Directory.Exists(_configurationFolderPath))
            Directory.CreateDirectory(_configurationFolderPath);

        var jsonConfigs = JsonSerializer.Serialize(ProxyDefaults, new JsonSerializerOptions { WriteIndented = true });
        using var sw = new StreamWriter(_configurationFilePath, new FileStreamOptions
        {
            Mode = FileMode.CreateNew,
            Access = FileAccess.Write,
        });
        sw.Write(jsonConfigs);
        
        return true;
    }
}
using System.Text.Json;
using GreenCobra.Client.Commands.Proxy.Models;
using GreenCobra.Client.Helpers;

namespace GreenCobra.Client.Services.Configuration;

public class ConfigurationDefaultService
{
    private const string ConfigFolderName = "green-cobra";
    private const string ConfigFileName = "config.json";

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

    public ProxyCommandInput ReadConfigurationFile() => CreateDefaultConfig() 
        ? ProxyCommandInput.Defaults : ReadFromConfigFile();

    private ProxyCommandInput ReadFromConfigFile()
    {
        var configString = File.ReadAllText(_configurationFilePath);
        var config = JsonSerializer.Deserialize<ProxyCommandInput>(configString)!;

        BuildTimeLogger.LogInformation(Resources.Logs.DefaultConfig_Loaded, config);
        
        return config;
    }

    private bool CreateDefaultConfig()
    {
        if (File.Exists(_configurationFilePath))
            return false;
        
        BuildTimeLogger.LogInformation(Resources.Logs.DefaultConfig_FileCreation, ProxyCommandInput.Defaults);

        if (!Directory.Exists(_configurationFolderPath))
            Directory.CreateDirectory(_configurationFolderPath);

        var jsonConfigs = JsonSerializer.Serialize(ProxyCommandInput.Defaults, new JsonSerializerOptions { WriteIndented = true });
        using var sw = new StreamWriter(_configurationFilePath, new FileStreamOptions
        {
            Mode = FileMode.CreateNew,
            Access = FileAccess.Write,
        });
        sw.Write(jsonConfigs);
        
        BuildTimeLogger.LogInformation(Resources.Logs.DefaultConfig_FileCreationDone, _configurationFilePath);
        
        return true;
    }
}
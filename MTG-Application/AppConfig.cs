using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Xaml;
using MTGApplication.General.Services.IOService;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MTGApplication;

/// <summary>
/// Class that contains application settings
/// </summary>
public static partial class AppConfig
{
  public static GlobalAppSettings GlobalSettings { get; set; } = new();
  public static LocalAppSettings LocalSettings { get; set; } = new();

  /// <summary>
  /// Initializes the App configurations using appsettings.json
  /// </summary>
  public static void Initialize()
  {
    GlobalSettings.Load();
    LocalSettings.Load();
  }

  /// <summary>
  /// Class that contains global application settings
  /// </summary>
  public class GlobalAppSettings
  {
    private static IConfigurationRoot configurationRoot;
    private static readonly string fileName = "appsettings.json";

    public string CompanyName { get; set; }

    /// <summary>
    /// Loads global settings from the application settings file.
    /// </summary>
    public void Load()
    {
      if (!File.Exists(fileName))
        throw new Exception("Error: App settings file not found. Look at 'appsettings - Template.json' file for more information");

      var builder = new ConfigurationBuilder().AddJsonFile(fileName, optional: false);
      configurationRoot = builder.Build();

      var config = configurationRoot.GetSection("AppInformation").GetChildren();
      CompanyName = config.First(x => x.Key == "Company").Value;
    }
  }

  /// <summary>
  /// Class that contains local application settings
  /// </summary>
  public partial class LocalAppSettings : ObservableObject
  {
    [ObservableProperty] private ElementTheme appTheme = ElementTheme.Default;

    private readonly static string fileName = "settings.json";
    private readonly static string filePath = Path.Join(IOService.GetAppDataPath(), fileName);

    public LocalAppSettings() => PropertyChanged += (s, e) => Save();

    /// <summary>
    /// Saves local settings to json file
    /// </summary>
    private void Save()
    {
      var json = JsonSerializer.Serialize(new
      {
        AppTheme,
      });

      IOService.WriteTextToFile(filePath, json);
    }

    /// <summary>
    /// Loads local settings from json file
    /// </summary>
    public void Load()
    {
      if (!File.Exists(filePath)) return;
      try
      {
        var json = JsonNode.Parse(IOService.ReadTextFromFile(filePath));
        var appTheme = json[nameof(AppTheme)]?.GetValue<int>() ?? (int)ElementTheme.Default;
        AppTheme = appTheme switch
        {
          1 => ElementTheme.Light,
          2 => ElementTheme.Dark,
          _ => ElementTheme.Default,
        };
      }
      catch (Exception e) { Debug.WriteLine(e.Message); }
    }
  }
}

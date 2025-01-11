using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Xaml;
using MTGApplication.General.Extensions;
using System;
using System.Collections.Generic;
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
    private static readonly string fileName = "appsettings.json";

    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Loads global settings from the application settings file.
    /// </summary>
    public void Load()
    {
      if (!File.Exists(fileName))
        throw new Exception("Error: App settings file not found. Look at 'appsettings - Template.json' file for more information");

      var builder = new ConfigurationBuilder().AddJsonFile(fileName, optional: false);
      var configurationRoot = builder.Build();

      if (configurationRoot.GetSection("AppInformation").GetChildren() is IEnumerable<IConfigurationSection> config)
        CompanyName = config.FirstOrDefault(x => x.Key == "Company")?.Value ?? CompanyName;
    }
  }

  /// <summary>
  /// Class that contains local application settings
  /// </summary>
  public partial class LocalAppSettings : ObservableObject
  {
    // Application theme can only set on start, so the themeing needs to be done with ElementThemes
    [ObservableProperty] public partial ElementTheme AppTheme { get; set; } = ElementTheme.Default;

    private readonly static string fileName = "settings.json";
    private readonly static string filePath = Path.Join(PathExtensions.GetAppDataPath(), fileName);

    public LocalAppSettings() => PropertyChanged += (s, e) => Save();

    /// <summary>
    /// Saves local settings to json file
    /// </summary>
    private void Save()
    {
      try
      {
        File.WriteAllText(filePath, JsonSerializer.Serialize(new { AppTheme = AppTheme }));
      }
      catch { }
    }

    /// <summary>
    /// Loads local settings from json file
    /// </summary>
    public void Load()
    {
      try
      {
        if (JsonNode.Parse(File.ReadAllText(filePath)) is JsonNode json)
        {
          var appTheme = json[nameof(AppTheme)]?.GetValue<int>() ?? (int)ElementTheme.Default;
          AppTheme = appTheme switch
          {
            1 => ElementTheme.Light,
            2 => ElementTheme.Dark,
            _ => ElementTheme.Default,
          };
        }
      }
      catch { }
    }
  }
}

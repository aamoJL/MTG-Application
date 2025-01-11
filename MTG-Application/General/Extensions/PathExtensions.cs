using System;
using System.IO;
using System.Reflection;

namespace MTGApplication.General.Extensions;

public static class PathExtensions
{
  /// <summary>
  /// Returns the application's AppData directory path. The path will be to the company's directory inside the AppData directory.
  /// If the debugger is attached, the path will be /Debug directory inside the company directory instead.
  /// </summary>
  public static string GetAppDataPath()
  {
    var path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppConfig.GlobalSettings.CompanyName, Assembly.GetCallingAssembly().GetName().Name);

    if (System.Diagnostics.Debugger.IsAttached)
      path = Path.Join(path, "Debug");

    try
    {
      Directory.CreateDirectory(path);
    }
    catch { }

    return path;
  }

  /// <summary>
  /// Retruns path to the application's Asset directory
  /// </summary>
  public static string GetAssetDirectoryPath()
    => Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Assets");
}

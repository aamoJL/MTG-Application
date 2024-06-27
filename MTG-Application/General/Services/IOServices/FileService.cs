using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.IOServices;

public static class FileService
{
  /// <summary>
  /// Returns the application's AppData directory path. The path will be to the company's directory inside the AppData directory.
  /// If the debugger is attached, the path will be /Debug directory inside the company directory instead.
  /// </summary>
  public static string GetAppDataPath()
  {
    var path = System.Diagnostics.Debugger.IsAttached ? Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppConfig.GlobalSettings.CompanyName,
        Assembly.GetCallingAssembly().GetName().Name, "Debug") : Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppConfig.GlobalSettings.CompanyName,
        Assembly.GetCallingAssembly().GetName().Name);

    Directory.CreateDirectory(path);
    return path;
  }

  /// <summary>
  /// Retruns path to the application's Asset directory
  /// </summary>
  public static string GetAssetDirectoryPath()
    => Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Assets");

  /// <summary>
  /// Returns array of json file names from a path
  /// </summary>
  public static string[] GetJsonFileNamesFromPath(string path)
    => Directory.GetFiles(path, "*.json").Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();

  /// <summary>
  /// Writes text to file
  /// </summary>
  public static bool TryWriteTextToFile(string path, string text)
  {
    try { File.WriteAllText(path, text); return true; }
    catch { return false; }
  }

  /// <summary>
  /// Reads text from a file
  /// </summary>
  /// <param name="path">File path</param>
  /// <returns></returns>
  public static bool TryReadTextFromFile(string path, out string output)
  {
    try { output = File.ReadAllText(path); }
    catch { output = null; }

    return output != null;
  }

  /// <summary>
  /// Returns text from the given file
  /// </summary>
  public static async Task<string> TryReadTextFromFileAsync(string path)
  {
    try { return await File.ReadAllTextAsync(path); }
    catch { return null; }
  }

  /// <summary>
  /// Deletes file from path
  /// </summary>
  /// <param name="path"></param>
  public static bool TryDeleteFile(string path)
  {
    try { File.Delete(path); return true; }
    catch { return false; }
  }
}

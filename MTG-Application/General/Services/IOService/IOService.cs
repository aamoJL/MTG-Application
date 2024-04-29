using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace MTGApplication.General.Services.IOService;

/// <summary>
/// Service that handles file systems and networking
/// </summary>
public static partial class IOService
{
  public static readonly HttpClient HttpClient = new();

  #region//-----------------File-------------------//
  /// <summary>
  /// Returns the application's AppData folder path. The path will be to the company's folder inside the AppData folder.
  /// If the debugger is attached, the path will be /Debug folder inside the company folder instead.
  /// </summary>
  public static string GetAppDataPath()
  {
    var path = System.Diagnostics.Debugger.IsAttached ? Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppConfig.GlobalSettings.CompanyName,
        Assembly.GetCallingAssembly().GetName().Name, "Debug") : Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppConfig.GlobalSettings.CompanyName,
        Assembly.GetCallingAssembly().GetName().Name);

    Directory.CreateDirectory(path);
    return path;
  }

  public static string GetAssetDirectoryPath() => Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Assets");

  /// <summary>
  /// Returns array of json file names from a path
  /// </summary>
  public static string[] GetJsonFileNames(string path)
  {
    var files = Directory.GetFiles(path, "*.json");
    var fileNames = new string[files.Length];

    for (var i = 0; i < files.Length; i++)
    {
      fileNames[i] = Path.GetFileNameWithoutExtension(files[i]);
    }

    return fileNames;
  }

  /// <summary>
  /// Writes text to file
  /// </summary>
  public static void WriteTextToFile(string path, string json)
  {
    try
    {
      File.WriteAllText(path, json);
    }
    catch (Exception) { }
  }

  /// <summary>
  /// Reads text from a file
  /// </summary>
  /// <param name="path">File path</param>
  /// <returns></returns>
  public static string ReadTextFromFile(string path)
  {
    try
    {
      return File.ReadAllText(path);
    }
    catch (Exception)
    {
      return "";
    }
  }

  /// <summary>
  /// Returns text from the given file
  /// </summary>
  public static async Task<string> ReadTextFromFileAsync(string path)
  {
    try
    {
      return await File.ReadAllTextAsync(path);
    }
    catch (Exception)
    {
      return "";
    }
  }

  /// <summary>
  /// Deletes file from path
  /// </summary>
  /// <param name="path"></param>
  public static void DeleteFile(string path)
  {
    try
    {
      File.Delete(path);
    }
    catch (Exception) { }
  }
  #endregion

  #region//---------------Networking------------------//
  /// <summary>
  /// Fetches text from the given <paramref name="url"/> using GET
  /// </summary>
  public static async Task<string> FetchStringFromURL(string url)
  {
    try
    {
      return await HttpClient.GetStringAsync(url);
    }
    catch (Exception) { return ""; }
  }

  /// <summary>
  /// Fetches text from the given <paramref name="url"/> using POST with the given <paramref name="content"/>
  /// </summary>
  public static async Task<string> FetchStringFromURLPost(string url, string content)
  {
    try
    {
      var result = await HttpClient.PostAsync(url, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
      return await result.Content.ReadAsStringAsync();
    }
    catch (Exception) { throw; }
  }

  /// <summary>
  /// Opens the given <paramref name="uri"/> with the default program (probably a web browser)
  /// </summary>
  public static async Task<bool> OpenUri(string uri) => await Launcher.LaunchUriAsync(new(uri));
  #endregion
}

// Clipboard service
public static partial class IOService
{
  /// <summary>
  /// Service that handles clipboard
  /// </summary>
  public class ClipboardService
  {
    /// <summary>
    /// Adds the <paramref name="text"/> to the clipboard
    /// </summary>
    public virtual void Copy(string text)
    {
      DataPackage dataPackage = new();
      dataPackage.SetText(text);
      SetClipboardContent(dataPackage);
    }

    /// <summary>
    /// Sets the clipboard content to the <paramref name="dataPackage"/>
    /// </summary>
    protected virtual void SetClipboardContent(DataPackage dataPackage) => Clipboard.SetContent(dataPackage);
  }
}

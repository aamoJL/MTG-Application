using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MTG_builder
{
  /// <summary>
  /// Functions that uses files and networking
  /// </summary>
  internal static class IO
  {
    public static readonly HttpClient HttpClient = new();
    public static readonly string CollectionsPath = "Resources/Collections/";

    /// <summary>
    /// Create directories if they do not exist
    /// </summary>
    public static void InitDirectories()
    {
      _ = Directory.CreateDirectory(CollectionsPath);
    }

    #region//-----------------File-------------------//
    /// <summary>
    /// Returns array of json file names from a path
    /// </summary>
    public static string[] GetJsonFileNames(string path)
    {
      string[] files = Directory.GetFiles(path, "*.json");
      string[] fileNames = new string[files.Length];

      for (int i = 0; i < files.Length; i++)
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
    /// Fetches JsonObject from given url
    /// </summary>
    /// <param name="url">Json file URL</param>
    /// <returns>JsonObject. Object will be null if the fetch is unsuccessful</returns>
    public static async Task<string> FetchStringFromURL(string url)
    {
      try
      {
        return await HttpClient.GetStringAsync(url);
      }
      catch (Exception) { return ""; }
    }
    public static async Task<string> FetchStringFromURLPost(string url, string content)
    {
      try
      {
        return await HttpClient.PostAsync(url, new StringContent(content, System.Text.Encoding.UTF8, "application/json")).Result.Content.ReadAsStringAsync();
      }
      catch (Exception) { return ""; }
    }
    #endregion
  }
}

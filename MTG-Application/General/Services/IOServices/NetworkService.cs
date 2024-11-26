using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Windows.System;

namespace MTGApplication.General.Services.IOServices;

public static class NetworkService
{
  public static readonly string JSON_MEDIA_TYPE = "application/json";

  /// <summary>
  /// Fetches Json string from the given url using GET
  /// </summary>
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  public static async Task<string> GetJsonFromUrl(string url)
  {
    try
    {
      var client = new HttpClient();
      var assemblyName = Assembly.GetExecutingAssembly().GetName() ?? throw new Exception("Assembly is null");

      client.DefaultRequestHeaders.Accept.Add(new(JSON_MEDIA_TYPE));
      client.DefaultRequestHeaders.UserAgent.Add(new(assemblyName!.Name ?? string.Empty, assemblyName!.Version?.ToString()));

      return await client.GetStringAsync(url);
    }
    catch { throw; }
  }

  /// <summary>
  /// Fetches Json string from the given url using POST
  /// </summary>
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  public static async Task<string> PostJsonFromUrl(string url, string content)
  {
    try
    {
      var client = new HttpClient();
      var assemblyName = Assembly.GetExecutingAssembly().GetName() ?? throw new Exception("Assembly is null");

      client.DefaultRequestHeaders.Accept.Add(new(JSON_MEDIA_TYPE));
      client.DefaultRequestHeaders.UserAgent.Add(new(assemblyName!.Name ?? string.Empty, assemblyName!.Version?.ToString()));

      return await (await client.PostAsync(url, new StringContent(content, System.Text.Encoding.UTF8, JSON_MEDIA_TYPE)))
        .Content.ReadAsStringAsync();
    }
    catch { throw; }
  }

  /// <summary>
  /// Opens the given <paramref name="uri"/> with the default program (probably a web browser)
  /// </summary>
  public static async Task<bool> OpenUri(string uri)
  {
    try { return await Launcher.LaunchUriAsync(new(uri)); }
    catch { return false; }
  }
}

using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Windows.System;

namespace MTGApplication.General.Services.IOServices;

public static class NetworkService
{
  public static readonly string JSON_MEDIA_TYPE = "application/json";

  private static HttpClient Client { get; } = new();

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
      var assemblyName = Assembly.GetExecutingAssembly().GetName() ?? throw new Exception("Assembly is null");

      using var request = new HttpRequestMessage
      {
        RequestUri = new(url),
        Method = HttpMethod.Get
      };

      request.Headers.Accept.Add(new(JSON_MEDIA_TYPE));
      request.Headers.UserAgent.Add(new(assemblyName!.Name ?? string.Empty, assemblyName!.Version?.ToString()));

      using var response = await Client.SendAsync(request);

      if (response.IsSuccessStatusCode)
        return await response.Content.ReadAsStringAsync();
      if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        return string.Empty;
      else
        throw new HttpRequestException(response.StatusCode.ToString());
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
      var assemblyName = Assembly.GetExecutingAssembly().GetName() ?? throw new Exception("Assembly is null");

      using var request = new HttpRequestMessage
      {
        RequestUri = new(url),
        Method = HttpMethod.Post
      };

      request.Headers.Accept.Add(new(JSON_MEDIA_TYPE));
      request.Headers.UserAgent.Add(new(assemblyName!.Name ?? string.Empty, assemblyName!.Version?.ToString()));
      request.Content = new StringContent(content, System.Text.Encoding.UTF8, JSON_MEDIA_TYPE);

      using var response = await Client.SendAsync(request);

      return await response.Content.ReadAsStringAsync();
    }
    catch { throw; }
  }

  /// <summary>
  /// Opens the given <paramref name="uri"/> with the default program (probably a web browser)
  /// </summary>
  public static async Task<bool> OpenUri(string uri)
  {
    try
    {
      return await Launcher.LaunchUriAsync(new(uri));
    }
    catch
    {
      return false;
    }
  }
}

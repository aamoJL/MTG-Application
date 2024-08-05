using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.System;

namespace MTGApplication.General.Services.IOServices;

// TODO: remove NetworkService and make the network requests on the origin method so the errors can be handled better.
public static class NetworkService
{
  private static readonly string JSON_MEDIA_TYPE = "application/json";

  /// <summary>
  /// Fetches text from the given <paramref name="url"/> using GET
  /// </summary>
  public static async Task<string> TryFetchStringFromUrlGetAsync(string url)
  {
    try
    {
      var client = new HttpClient();
      client.DefaultRequestHeaders.Accept.Add(new(JSON_MEDIA_TYPE));
      client.DefaultRequestHeaders.UserAgent.Add(new("MTGApplication", "1"));

      return await client.GetStringAsync(url);
    }
    catch { return null; }
  }

  /// <summary>
  /// Fetches text from the given <paramref name="url"/> using POST with the given <paramref name="content"/>
  /// </summary>
  public static async Task<string> TryFetchStringFromUrlPostAsync(string url, string content)
  {
    try
    {
      var client = new HttpClient();
      client.DefaultRequestHeaders.Accept.Add(new(JSON_MEDIA_TYPE));
      client.DefaultRequestHeaders.UserAgent.Add(new("MTGApplication", "1"));

      return await (await client.PostAsync(url, new StringContent(content, System.Text.Encoding.UTF8, JSON_MEDIA_TYPE)))
        .Content.ReadAsStringAsync();
    }
    catch { return null; }
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

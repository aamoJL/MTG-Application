using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.System;

namespace MTGApplication.General.Services.IOService;

public static class NetworkService
{
  public static readonly HttpClient HttpClient = new();

  /// <summary>
  /// Fetches text from the given <paramref name="url"/> using GET
  /// </summary>
  public static async Task<string> TryFetchStringFromUrlGetAsync(string url)
  {
    try { return await HttpClient.GetStringAsync(url); }
    catch { return null; }
  }

  /// <summary>
  /// Fetches text from the given <paramref name="url"/> using POST with the given <paramref name="content"/>
  /// </summary>
  public static async Task<string> TryFetchStringFromUrlPostAsync(string url, string content)
  {
    try
    {
      var result = await HttpClient.PostAsync(url, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
      return await result.Content.ReadAsStringAsync();
    }
    catch { return null; }
  }

  /// <summary>
  /// Opens the given <paramref name="uri"/> with the default program (probably a web browser)
  /// </summary>
  public static async Task<bool> OpenUri(string uri) => await Launcher.LaunchUriAsync(new(uri));
}

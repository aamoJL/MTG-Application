using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.IOServices;
using System;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter;

public partial class EdhrecImporter
{
  private static readonly string API_URI = "https://json.edhrec.com/pages/commanders";
  private static readonly string WEBSITE_BASE_URI = "https://edhrec.com/commanders";

  /// <summary>
  /// Tries to parse a card <paramref name="name"/> from the given <paramref name="data"/>
  /// The <paramref name="data"/> should be an EDHREC card uri.
  /// </summary>
  /// <param name="data">EDHREC card Uri</param>
  /// <param name="name">Parsed card name</param>
  /// <returns><see langword="true"/> if the <paramref name="data"/> was successfully parsed; otherwise, <see langword="false"/></returns>
  public static bool TryParseCardNameFromEdhrecUri(string data, out string? name)
  {
    if (Uri.TryCreate(data, UriKind.Absolute, out var uri) && uri.Host == "edhrec.com")
      name = uri.Segments[^1]; // Name is the last segment of the Uri
    else
      name = null;

    return name != null;
  }

  public static string GetCommanderWebsiteUri(DeckEditorMTGCard commander, DeckEditorMTGCard? partner, string themeSuffix = "")
  {
    if (commander == null)
      return string.Empty;

    var stringBuilder = new StringBuilder($"{WEBSITE_BASE_URI}/{commander.Info.Name.ToKebabCase().ToLower()}");

    if (partner != null)
      stringBuilder.Append($"-{partner.Info.Name.ToKebabCase().ToLower()}");

    if (!string.IsNullOrEmpty(themeSuffix))
      stringBuilder.Append($"/{themeSuffix.ToKebabCase().ToLower()}");

    return stringBuilder.ToString();
  }

  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  /// <exception cref="System.Text.Json.JsonException"></exception>
  public static async Task<CommanderTheme[]> GetThemes(string commander, string? partner = null)
  {
    var uri = GetApiUri(commander, partner);

    if (string.IsNullOrEmpty(uri))
      return [];

    try
    {
      var json = JsonNode.Parse(await NetworkService.GetJsonFromUrl(uri));
      var themeNodes = json?["panels"]?["tribelinks"]?.AsArray();

      var themes = themeNodes?.Select(x => (
        x?["value"]?.GetValue<string>(),
        GetApiUri(commander, partner, GetLeadingSlash().Replace(x?["href-suffix"]?.GetValue<string>() ?? string.Empty, string.Empty))))
        .Where(x => x.Item1 != null && x.Item2 != string.Empty)
        .Select(x => new CommanderTheme(x.Item1!, x.Item2))
        .ToArray() ?? [];

      return themes;
    }
    catch { throw; }
  }

  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  /// <exception cref="System.Text.Json.JsonException"></exception>
  public static async Task<string[]> FetchNewCardNames(string uri)
  {
    try
    {
      var jsonString = await NetworkService.GetJsonFromUrl(uri);
      var json = JsonNode.Parse(jsonString);

      var names = json?["container"]?["json_dict"]?["cardlists"]?.AsArray()
        .FirstOrDefault(x => x?["tag"]?.GetValue<string>() == "newcards")?["cardviews"]?.AsArray()
        .Select(x => x?["name"]?.GetValue<string>())
        .Where(x => !string.IsNullOrEmpty(x))
        .Select(x => x!).ToArray();

      return names ?? [];
    }
    catch { throw; }
  }

  /// <summary>
  /// Returns the api uri for the given commanders
  /// </summary>
  private static string GetApiUri(string commander, string? partner = null, string themeSuffix = "")
  {
    // Example fetch uri:
    // https://json.edhrec.com/pages/commanders/{commander-names-like-this}/{theme}.json
    // NOTE: Commander names needs to be in lower kebab case

    if (string.IsNullOrEmpty(commander))
      return string.Empty;

    var stringBuilder = new StringBuilder($"{API_URI}/{commander.ToKebabCase().ToLower()}");

    if (!string.IsNullOrEmpty(partner))
      stringBuilder.Append($"-{partner.ToKebabCase().ToLower()}");

    if (!string.IsNullOrEmpty(themeSuffix))
      stringBuilder.Append($"/{themeSuffix.ToKebabCase().ToLower()}");

    stringBuilder.Append(".json");

    return stringBuilder.ToString();
  }

  [GeneratedRegex(@"^\/")]
  private static partial Regex GetLeadingSlash();
}
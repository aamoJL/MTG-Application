using MTGApplication.General.Extensions;
using MTGApplication.Models.Structs;
using MTGApplication.Services.IOService;
using System;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MTGApplication.API.CardAPI;
/// <summary>
/// API to fetch MTG commander information, e.g. new relevant cards, from EDHREC.com
/// </summary>
public class EDHRECCommanderAPI : IMTGCommanderAPI
{
  private readonly string BASE_URI = "https://json.edhrec.com/pages/commanders";
  private readonly string WEBSITE_BASE_URI = "https://edhrec.com/commanders";

  /// <summary>
  /// Returns the api uri for the given commanders
  /// </summary>
  public string GetUri(Commanders commanders, string themeSuffix = "")
  {
    // Example fetch uri:
    // https://json.edhrec.com/pages/commanders/{commander-names-like-this}/{theme}.json
    // NOTE: Commander names needs to be in lower kebab case
    if (string.IsNullOrEmpty(commanders.Commander) && string.IsNullOrEmpty(commanders.Partner))
      return string.Empty;

    var stringBuilder = new StringBuilder();
    stringBuilder.Append($"{BASE_URI}/");
    stringBuilder.Append(commanders.AsKebabString());

    if (!string.IsNullOrEmpty(themeSuffix))
    {
      stringBuilder.Append($"/{themeSuffix.ToKebabCase().ToLower()}");
    }

    stringBuilder.Append(".json");

    return stringBuilder.ToString();
  }

  #region IMTGCommanderAPI implementation
  public async Task<CommanderTheme[]> GetThemes(Commanders commanders)
  {
    var uri = GetUri(commanders);

    if (string.IsNullOrEmpty(uri)) return Array.Empty<CommanderTheme>();

    var jsonString = await IOService.FetchStringFromURL(uri);

    if (string.IsNullOrEmpty(jsonString))
      return Array.Empty<CommanderTheme>();

    try
    {
      var json = JsonNode.Parse(jsonString) ?? throw new Exception();
      var themeNodes = json["panels"]?["tribelinks"]?["themes"]?.AsArray();

      return themeNodes?.Select(
        x => new CommanderTheme(
          x["value"].GetValue<string>(),
          GetUri(commanders, Regex.Replace(x["href-suffix"].GetValue<string>(), @"^\/", string.Empty))))
        .ToArray()
       ?? Array.Empty<CommanderTheme>();
    }
    catch (Exception) { return Array.Empty<CommanderTheme>(); }
  }

  public async Task<string[]> FetchNewCards(string uri)
  {
    var json = JsonNode.Parse(await IOService.FetchStringFromURL(uri));

    if (json == null) return Array.Empty<string>();

    return json["container"]?["json_dict"]?["cardlists"].AsArray()
      .FirstOrDefault(x => x["tag"]?.GetValue<string>() == "newcards")?["cardviews"]?.AsArray()
      .Select(x => x["name"]!.GetValue<string>()).ToArray()
      ?? Array.Empty<string>();
  }

  public string GetCommanderWebsiteUri(Commanders commanders, string themeSuffix = "")
  {
    var stringBuilder = new StringBuilder();
    stringBuilder.Append($"{WEBSITE_BASE_URI}/");
    stringBuilder.Append(commanders.AsKebabString());

    if (!string.IsNullOrEmpty(themeSuffix))
    {
      stringBuilder.Append($"/{themeSuffix.ToKebabCase().ToLower()}");
    }

    return stringBuilder.ToString();
  }
  #endregion
}

using MTGApplication.General.Extensions;
using MTGApplication.General.Models.Card;
using System;
using System.Text;

namespace MTGApplication.General.Services.API.CardAPI;

public class EdhrecAPI
{
  private static readonly string WEBSITE_BASE_URI = "https://edhrec.com/commanders";

  /// <summary>
  /// Tries to parse a card <paramref name="name"/> from the given <paramref name="data"/>
  /// The <paramref name="data"/> should be an EDHREC card uri.
  /// </summary>
  /// <param name="data">EDHREC card Uri</param>
  /// <param name="name">Parsed card name</param>
  /// <returns><see langword="true"/> if the <paramref name="data"/> was successfully parsed; otherwise, <see langword="false"/></returns>
  public static bool TryParseCardNameFromEdhrecUri(string data, out string name)
  {
    if (Uri.TryCreate(data, UriKind.Absolute, out var uri) && uri.Host == "edhrec.com")
      name = uri.Segments[^1]; // Name is the last segment of the Uri
    else
      name = default;

    return name != default;
  }

  public static string GetCommanderWebsiteUri(MTGCard commander, MTGCard partner, string themeSuffix = "")
  {
    if (commander == null) return string.Empty;

    var stringBuilder = new StringBuilder($"{WEBSITE_BASE_URI}/{commander.Info.Name.ToKebabCase().ToLower()}");

    if(partner != null)
      stringBuilder.Append($"-{partner.Info.Name.ToKebabCase().ToLower()}");

    if (!string.IsNullOrEmpty(themeSuffix))
      stringBuilder.Append($"/{themeSuffix.ToKebabCase().ToLower()}");

    return stringBuilder.ToString();
  }
}
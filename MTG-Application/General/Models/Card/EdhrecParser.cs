using System;

namespace MTGApplication.General.Models.Card;

public static class EdhrecParser
{
  private static readonly string HOST_NAME = "edhrec.com";

  /// <summary>
  /// Tries to parse a card <paramref name="name"/> from the given <paramref name="data"/>
  /// The <paramref name="data"/> should be an EDHREC card uri.
  /// </summary>
  /// <param name="data">EDHREC card Uri</param>
  /// <param name="name">Parsed card name</param>
  /// <returns><see langword="true"/> if the <paramref name="data"/> was successfully parsed; otherwise, <see langword="false"/></returns>
  public static bool TryParseNameFromUri(string data, out string name)
  {
    if (Uri.TryCreate(data, UriKind.Absolute, out var uri) && uri.Host == HOST_NAME)
      name = uri.Segments[^1]; // Name is the last segment of the Uri
    else
      name = default;

    return name != default;
  }
}
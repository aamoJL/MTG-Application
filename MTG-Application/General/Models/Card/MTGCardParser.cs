using System.Text.Json;

namespace MTGApplication.General.Models.Card;

public static class MTGCardParser
{
  public static bool TryParseJson(string json, out MTGCard card)
  {
    try { card = JsonSerializer.Deserialize<MTGCard>(json); }
    catch { card = null; }

    return card != default;
  }
}
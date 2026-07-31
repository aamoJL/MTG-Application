using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.General.Extensions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using static MTGApplication.General.Services.Cache.Caching;

namespace MTGApplication.General.Services.Cache.Caches;

public static class DeckSelectionCaching
{
  public static JsonNode? ToJson(object value)
  {
    if (JsonExtensions.TrySerializeObject(value, out var output))
      return JsonNode.Parse(output);
    else return null;
  }

  public static Dictionary<string, string>? FromJson(JsonNode json)
  {
    var decks = new Dictionary<string, string>();

    foreach (var item in json.AsObject())
    {
      if (item.Value != null && item.Value.GetValue<string>() is string result && !string.IsNullOrEmpty(result))
        decks.TryAdd(item.Key, result);
    }

    return decks.Count != 0 ? decks : null;
  }

  extension(IMemoryCache<CacheKey> cache)
  {
    public bool TryGetSelectionDeck(string name, [NotNullWhen(true)] out DeckSelectionDeck? selectionDeck)
    {
      var jsons = cache.Get<Dictionary<string, string>>(CacheKey.DeckSelection);

      if (jsons?[name] is string json && JsonExtensions.TryDeserializeJson(json, out selectionDeck)) { }
      else
        selectionDeck = null;

      return selectionDeck != null;
    }

    public void TryCacheSelectionDecks(IEnumerable<DeckSelectionDeck> decks)
    {
      try
      {
        if (cache.GetOrCreate<Dictionary<string, string>>(CacheKey.DeckSelection, (_) => []) is Dictionary<string, string> deckCache)
        {
          foreach (var item in decks)
          {
            if (JsonExtensions.TrySerializeObject(item, out var json))
              deckCache[item.Name] = json;
          }
        }
      }
      catch { }
    }

    public void UnCacheSelectionDeck(string name)
    {
      var jsons = cache.Get<Dictionary<string, string>>(CacheKey.DeckSelection);

      jsons?.Remove(name);
    }
  }
}

using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Extensions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using static MTGApplication.General.Services.Cache.Caching;

namespace MTGApplication.General.Services.Cache.Caches;

public static class DeckEditorCaching
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
    public bool TryGetDeck(string name, [NotNullWhen(true)] out DeckEditorMTGDeck? deck)
    {
      var json = cache.Get<Dictionary<string, string>>(CacheKey.Deck)?.GetValueOrDefault(name);

      if (json != null && JsonExtensions.TryDeserializeJson(json, out deck)) { }
      else
        deck = null;

      return deck != null;
    }

    public void CacheDeck(DeckEditorMTGDeck deck)
    {
      if (JsonExtensions.TrySerializeObject(deck, out var json))
      {
        var decks = cache.GetOrCreate(CacheKey.Deck, (_) => new Dictionary<string, string>());

        decks?[deck.Name] = json;

        // Uncache selection deck if commander changed
        if (cache.TryGetSelectionDeck(deck.Name, out var selectionDeck))
        {
          if (selectionDeck.ImageUri != deck.Commander?.Info.FrontFace.ArtCropUri)
            cache.UnCacheSelectionDeck(selectionDeck.Name);
        }
      }
    }

    public void UnCacheDeck(string name)
    {
      cache.Get<Dictionary<string, string>>(CacheKey.Deck)?.Remove(name);
      cache.UnCacheSelectionDeck(name); // Uncache also selection deck
    }
  }
}
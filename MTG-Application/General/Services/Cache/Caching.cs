using MTGApplication.General.Extensions;
using MTGApplication.General.Services.Cache.Caches;
using System;
using System.IO;
using System.Text.Json.Nodes;

namespace MTGApplication.General.Services.Cache;

public static class Caching
{
  private static readonly string _fileName = "cache.json";
  private static readonly string _filePath = Path.Join(PathExtensions.GetAppDataPath(), _fileName);
  private static readonly TimeSpan _expirationTime = new(7, 0, 0, 0);

  public enum CacheKey
  {
    DeckSelection, Deck, CardImport
  }

  extension(IMemoryCache<CacheKey> cache)
  {
    private JsonObject ToJson()
    {
      var root = new JsonObject();

      foreach (var key in cache.Keys)
      {
        if (cache.Get(key) is not object value)
          continue;

        var node = key switch
        {
          CacheKey.DeckSelection => DeckSelectionCaching.ToJson(value),
          CacheKey.Deck => DeckEditorCaching.ToJson(value),
          CacheKey.CardImport => CardImportCaching.ToJson(value),
          _ => null
        };

        if (node != null)
          root.TryAdd(key.ToString(), node);
      }

      return root;
    }

    public void LoadFromFile()
    {
      try
      {
        if (File.GetCreationTime(_filePath).Add(_expirationTime) < DateTime.Now)
          File.Delete(_filePath);

        if (JsonNode.Parse(File.ReadAllText(_filePath)) is JsonNode root)
        {
          foreach (var key in Enum.GetValues<CacheKey>())
          {
            if (root[key.ToString()] is not JsonNode value)
              continue;

            var result = key switch
            {
              CacheKey.DeckSelection => DeckSelectionCaching.FromJson(value),
              CacheKey.Deck => DeckEditorCaching.FromJson(value),
              CacheKey.CardImport => CardImportCaching.FromJson(value),
              _ => null
            };

            if (result != null)
              cache.Set(key, result);
          }
        }
      }
      catch { }
    }

    public void SaveToFile()
    {
      try
      {
        File.WriteAllText(_filePath, cache.ToJson().ToJsonString());
      }
      catch { }
    }

    public void Invalidate()
    {
      cache.Clear();

      try
      {
        File.Delete(_filePath);
      }
      catch { }
    }
  }
}
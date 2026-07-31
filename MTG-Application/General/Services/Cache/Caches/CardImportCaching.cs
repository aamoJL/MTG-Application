using MTGApplication.General.Extensions;
using MTGApplication.General.Services.Importers.CardImporter;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using static MTGApplication.General.Services.Cache.Caching;

namespace MTGApplication.General.Services.Cache.Caches;

public static class CardImportCaching
{
  public static JsonNode? ToJson(object value)
  {
    if (JsonExtensions.TrySerializeObject(value, out var output))
      return JsonNode.Parse(output);
    else return null;
  }

  public static Dictionary<string, string>? FromJson(JsonNode json)
  {
    var results = new Dictionary<string, string>();

    foreach (var item in json.AsObject())
    {
      if (item.Value != null && item.Value.GetValue<string>() is string result && !string.IsNullOrEmpty(result))
        results.TryAdd(item.Key, result);
    }

    return results.Count != 0 ? results : null;
  }

  extension(IMemoryCache<CacheKey> cache)
  {
    public bool TryGetImportResult(string importString, [NotNullWhen(true)] out CardImportResult? result)
    {
      var json = cache.Get<Dictionary<string, string>>(CacheKey.CardImport)?.GetValueOrDefault(importString);

      if (json != null && JsonExtensions.TryDeserializeJson(json, out result)) { }
      else
        result = null;

      return result != null;
    }

    public void CacheImportResult(string importString, CardImportResult result)
    {
      if (JsonExtensions.TrySerializeObject(result, out var json))
      {
        var results = cache.GetOrCreate(CacheKey.CardImport, (_) => new Dictionary<string, string>());

        results?[importString] = json;
      }
    }
  }
}

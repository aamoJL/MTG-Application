using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MTGApplication.General.Services.IOServices;

public static class JsonService
{
  // TODO: remove try methods for better exception handling

  /// <summary>
  /// Tries to parse data to json object
  /// </summary>
  public static bool TryParseJson(string data, out JsonNode? rootNode)
  {
    try { rootNode = JsonNode.Parse(data); }
    catch { rootNode = null; }

    return rootNode != null;
  }

  public static bool TryDeserializeJson<T>(string json, out T? output)
  {
    try { output = JsonSerializer.Deserialize<T>(json); }
    catch { output = default; }

    return !EqualityComparer<T>.Default.Equals(output, default);
  }

  public static bool TrySerializeObject<T>(T input, out string? output)
  {
    try { output = JsonSerializer.Serialize(input); }
    catch { output = null; }

    return output != null;
  }
}

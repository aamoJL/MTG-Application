using System.Text.Json.Nodes;

namespace MTGApplication.General.Services.IOService;

public static class JsonService
{
  /// <summary>
  /// Tries to parse data to json object
  /// </summary>
  public static bool TryParseJson(string data, out JsonNode rootNode)
  {
    try { rootNode = JsonNode.Parse(data); }
    catch { rootNode = null; }

    return rootNode != null;
  }
}

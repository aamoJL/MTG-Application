using System.Collections.Generic;
using System.Text.Json;

namespace MTGApplication.General.Extensions;

public static class JsonExtensions
{
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

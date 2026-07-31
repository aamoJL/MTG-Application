using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace MTGApplication.General.Extensions;

public static class JsonExtensions
{
  public static bool TryDeserializeJson<T>(string json, [NotNullWhen(true)] out T? output)
  {
    try { output = JsonSerializer.Deserialize<T>(json); }
    catch { output = default; }

    return !EqualityComparer<T>.Default.Equals(output, default);
  }

  public static bool TrySerializeObject<T>(T input, [NotNullWhen(true)] out string? output)
  {
    if (input == null)
    {
      output = null;
      return false;
    }

    try { output = JsonSerializer.Serialize(input); }
    catch { output = null; }

    return output != null;
  }
}

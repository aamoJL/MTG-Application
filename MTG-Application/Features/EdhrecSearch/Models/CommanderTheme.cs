namespace MTGApplication.Features.EdhrecSearch.Models;

/// <summary>
/// Struct for commander's theme on EDHREC card search
/// </summary>
public readonly struct CommanderTheme(string name, string uri)
{
  public string Name { get; } = name;
  public string Uri { get; } = uri;
}

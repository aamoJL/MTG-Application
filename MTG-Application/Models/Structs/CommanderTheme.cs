namespace MTGApplication.Models.Structs;

/// <summary>
/// Struct for EDHREC themed card search
/// </summary>
public readonly struct CommanderTheme
{
  public CommanderTheme(string name, string uri)
  {
    Name = name;
    Uri = uri;
  }

  public string Name { get; }
  public string Uri { get; }
}

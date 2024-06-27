namespace MTGApplication.General.Services.Importers.CardImporter;

public partial class EdhrecImporter
{
  /// <summary>
  /// Struct for commander's theme on EDHREC card search
  /// </summary>
  public readonly struct CommanderTheme(string name, string uri)
  {
    public string Name { get; } = name;
    public string Uri { get; } = uri;
  }
}
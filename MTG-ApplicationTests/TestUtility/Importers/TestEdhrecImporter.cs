using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Importers.CardImporter;

namespace MTGApplicationTests.TestUtility.Importers;

public class TestEdhrecImporter : IEdhrecImporter
{
  public string[] CardNames { get; init; } = null;
  public CommanderTheme[] CommanderThemes { get; init; } = null;
  public bool? ParseResult { get; init; } = null;
  public string CommanderWebsiteUri { get; init; } = null;

  public Task<string[]> FetchNewCardNames(string uri)
  {
    if (CardNames == null)
      throw new NotImplementedException(nameof(CardNames));

    return Task.FromResult(CardNames);
  }

  public string GetCommanderWebsiteUri(DeckEditorMTGCard commander, DeckEditorMTGCard partner, string themeSuffix = "")
    => CommanderWebsiteUri ?? throw new NotImplementedException(nameof(CommanderWebsiteUri));

  public Task<CommanderTheme[]> GetThemes(string commander, string partner = null)
  {
    if (CommanderThemes == null)
      throw new NotImplementedException(nameof(CommanderThemes));

    return Task.FromResult(CommanderThemes);
  }

  public bool TryParseCardNameFromEdhrecUri(string uri, out string name)
  {
    if (!ParseResult.HasValue) throw new NotImplementedException(nameof(ParseResult));

    name = ParseResult.Value ? "Name" : null;

    return ParseResult.Value;
  }
}
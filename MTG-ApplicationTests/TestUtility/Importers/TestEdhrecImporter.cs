using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Importers.CardImporter;

namespace MTGApplicationTests.TestUtility.Importers;

public class TestEdhrecImporter : IEdhrecImporter
{
  public string[]? CardNames { get; init; } = null;
  public CommanderTheme[]? CommanderThemes { get; init; } = null;
  public string? ParseResult { get; init; } = null;
  public string? CommanderWebsiteUri { get; init; } = null;

  public async Task<string[]> FetchNewCardNames(string _)
  {
    if (CardNames == null)
      throw new NotImplementedException($"FetchNewCardNames: {nameof(CardNames)}");

    return [.. CardNames];
  }

  public string GetCommanderWebsiteUri(DeckEditorMTGCard commander, DeckEditorMTGCard? partner, string themeSuffix = "")
    => CommanderWebsiteUri ?? throw new NotImplementedException(nameof(CommanderWebsiteUri));

  public async Task<CommanderTheme[]> GetThemes(string commander, string? partner = null)
  {
    if (CommanderThemes == null)
      throw new NotImplementedException($"GetThemes: {nameof(CommanderThemes)}");

    return await Task.FromResult(CommanderThemes);
  }

  public bool TryParseCardNameFromEdhrecUri(string uri, out string name)
  {
    if (ParseResult == null)
      throw new NotImplementedException($"TryParseCardNameFromEdhrecUri: {nameof(ParseResult)}");

    name = string.IsNullOrEmpty(ParseResult) ? "Name" : string.Empty;

    return !string.IsNullOrEmpty(ParseResult);
  }
}
using MTGApplication.General.Models;

namespace MTGApplication.Features.DeckSelection.Models;

public record DeckSelectionDeck
{
  public string Name { get; init; } = string.Empty;
  public string ImageUri { get; init; } = "";
  public ValueEqualityList<MTGCardInfo.ColorTypes> Colors { get; init; } = [];
};
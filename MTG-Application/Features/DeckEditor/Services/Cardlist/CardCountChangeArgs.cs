using MTGApplication.General.Models.Card;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModel
{
  public record CardCountChangeArgs(MTGCard Card, int NewValue);
}

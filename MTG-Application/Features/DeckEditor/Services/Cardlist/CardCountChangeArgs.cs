using MTGApplication.General.Models.Card;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModel
{
  public record CardCountChangeArgs(DeckEditorMTGCard Card, int NewValue);
}

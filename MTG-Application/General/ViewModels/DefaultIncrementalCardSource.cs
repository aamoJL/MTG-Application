using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;

namespace MTGApplication.General.ViewModels;

public class DefaultIncrementalCardSource(ICardAPI<DeckEditorMTGCard> cardAPI) : IncrementalCardSource<DeckEditorMTGCard>(cardAPI)
{
  protected override DeckEditorMTGCard ConvertToCardType(DeckEditorMTGCard card) => card;
}

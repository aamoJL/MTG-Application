using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.CardCollection.Services;

public class CardCollectionIncrementalCardSource(ICardAPI<DeckEditorMTGCard> cardAPI) : IncrementalCardSource<CardCollectionMTGCard>(cardAPI)
{
  protected override CardCollectionMTGCard ConvertToCardType(DeckEditorMTGCard card) => new(card.Info);
}

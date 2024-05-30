using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.CardCollection;

public class CardCollectionIncrementalCardSource(ICardAPI<MTGCard> cardAPI) : IncrementalCardSource<CardCollectionMTGCard>(cardAPI)
{
  protected override CardCollectionMTGCard ConvertToCardType(MTGCard card) => new(card.Info);
}

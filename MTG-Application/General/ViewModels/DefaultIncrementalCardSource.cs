using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;

namespace MTGApplication.General.ViewModels;

public class DefaultIncrementalCardSource(ICardAPI<MTGCard> cardAPI) : IncrementalCardSource<MTGCard>(cardAPI)
{
  protected override MTGCard ConvertToCardType(MTGCard card) => card;
}

using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.CardSearch.Models;

public class CardSearchIncrementalCardSource(MTGCardImporter importer) : IncrementalCardSource<CardSearchMTGCard>(importer)
{
  protected override CardSearchMTGCard ConvertToCardType(CardImportResult<MTGCardInfo>.Card card) 
    => new(card.Info);
}

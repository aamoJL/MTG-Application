using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.CardSearch.Models;

public class CardSearchIncrementalCardSource(MTGCardImporter importer) : IncrementalCardSource<MTGCard>(importer)
{
  protected override MTGCard ConvertToCardType(CardImportResult<MTGCardInfo>.Card card)
    => new(card.Info);
}

using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.CardSearch.Models;

public class CardSearchIncrementalCardSource(IMTGCardImporter importer) : IncrementalCardSource<MTGCard>(importer)
{
  protected override MTGCard ConvertToCardType(CardImportResult.Card card)
    => new(card.Info);
}

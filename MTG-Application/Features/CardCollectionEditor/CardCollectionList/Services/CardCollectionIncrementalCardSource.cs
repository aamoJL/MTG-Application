using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;

public class CardCollectionIncrementalCardSource(IMTGCardImporter importer) : IncrementalCardSource<CardCollectionMTGCard>(importer)
{
  protected override CardCollectionMTGCard ConvertToCardType(CardImportResult.Card card)
    => new(card.Info);
}

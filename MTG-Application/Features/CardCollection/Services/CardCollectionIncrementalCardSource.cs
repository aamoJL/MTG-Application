﻿using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.CardCollection.Services;

public class CardCollectionIncrementalCardSource(MTGCardImporter importer) : IncrementalCardSource<CardCollectionMTGCard>(importer)
{
  protected override CardCollectionMTGCard ConvertToCardType(CardImportResult<MTGCardInfo>.Card card)
    => new(card.Info);
}

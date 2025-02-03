using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter.UseCases;

public class FetchTokenCards(IMTGCardImporter importer) : UseCase<IEnumerable<MTGCard>, Task<CardImportResult>>
{
  /// /// <inheritdoc cref="FetchCardsWithImportString.Execute(string)" path="/exception"/>
  public override async Task<CardImportResult> Execute(IEnumerable<MTGCard> cards)
  {
    var ids = string.Join(Environment.NewLine,
      cards
      .Where(x => x.Info.Tokens.Length > 0)
      .DistinctBy(x => x.Info.OracleId)
      .SelectMany(c => c.Info.Tokens.Select(t => t.ScryfallId.ToString())));

    return await new FetchCardsWithImportString(importer).Execute(ids);
  }
}
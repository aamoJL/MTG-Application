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
    var ids = string.Join(Environment.NewLine, cards
      .SelectMany(c => c.Info.Tokens)
      .Select(t => t.ScryfallId)
      .Distinct());

    var result = await new FetchCardsWithImportString(importer).Execute(ids);

    return result with { Found = [.. result.Found.DistinctBy(x => x.Info.OracleId).OrderBy(x => x.Info.Name)] };
  }
}
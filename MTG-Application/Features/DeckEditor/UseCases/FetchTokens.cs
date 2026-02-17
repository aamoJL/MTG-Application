using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.UseCases;

public class FetchTokens(IMTGCardImporter importer) : UseCaseFunc<IEnumerable<MTGCard>, Task<CardImportResult>>
{
  public override async Task<CardImportResult> Execute(IEnumerable<MTGCard> cards)
    => await new FetchTokenCards(importer).Execute(cards);
}
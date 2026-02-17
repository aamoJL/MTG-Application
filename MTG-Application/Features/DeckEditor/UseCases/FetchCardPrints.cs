using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.UseCases;

public class FetchCardPrints(IMTGCardImporter importer) : UseCaseFunc<MTGCard, Task<CardImportResult>>
{
  public override async Task<CardImportResult> Execute(MTGCard card)
    => await importer.ImportWithUri(pageUri: card.Info.PrintSearchUri, paperOnly: true, fetchAll: true);
}
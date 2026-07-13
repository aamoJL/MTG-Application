using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.UseCases;

public class FetchCardPrints(IMTGCardImporter importer) : UseCaseFunc<MTGCard, Task<CardImportResult>>
{
  public override async Task<CardImportResult> Execute(MTGCard card)
    => await new FetchCardsWithUri(importer) { FetchAll = true }.Execute(card.Info.PrintSearchUri);
}
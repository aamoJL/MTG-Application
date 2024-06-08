using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.UseCases;

public class ImportCards(ICardAPI<DeckEditorMTGCard> cardAPI) : UseCase<string, Task<CardImportResult>>
{
  public override async Task<CardImportResult> Execute(string data)
    => await new CardImporter(cardAPI).Import(data);
}
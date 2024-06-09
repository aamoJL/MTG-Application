using MTGApplication.General.Models.Card;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.API.CardAPI.UseCases;
public class FetchCardsWithSearchQuery(ICardImporter<DeckEditorMTGCard> cardAPI) : UseCase<string, Task<CardImportResult>>
{
  public async override Task<CardImportResult> Execute(string query)
    => await cardAPI.ImportCardsWithSearchQuery(query);
}
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public class ImportCards : UseCase<string, Task<CardImportResult>>
{
  public ImportCards(ICardAPI<MTGCard> cardAPI) => CardAPI = cardAPI;

  public ICardAPI<MTGCard> CardAPI { get; }

  public override async Task<CardImportResult> Execute(string data)
    => await new CardImporter(CardAPI).Import(data);
}
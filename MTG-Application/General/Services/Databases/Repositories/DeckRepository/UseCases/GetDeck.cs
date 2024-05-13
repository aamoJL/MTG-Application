using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.DeckRepository;

public class GetDeck : UseCase<string, Task<MTGCardDeck>>
{
  public GetDeck(IRepository<MTGCardDeckDTO> repository, ICardAPI<MTGCard> cardAPI)
  {
    Repository = repository;
    CardAPI = cardAPI;
  }

  public IRepository<MTGCardDeckDTO> Repository { get; }
  public ICardAPI<MTGCard> CardAPI { get; }

  public async override Task<MTGCardDeck> Execute(string name)
  {
    if (string.IsNullOrEmpty(name)) return null;

    var deck = await Repository.Get(name);

    return deck != null ? await new DTOToDeckConversion(CardAPI).Execute(deck) : null;
  }
}
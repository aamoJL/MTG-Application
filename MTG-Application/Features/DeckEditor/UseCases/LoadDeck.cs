using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.DeckRepository;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public class LoadDeck : UseCase<string, Task<MTGCardDeck>>
{
  public LoadDeck(IRepository<MTGCardDeckDTO> repository, ICardAPI<MTGCard> cardAPI)
  {
    Repository = repository;
    CardAPI = cardAPI;
  }

  public IRepository<MTGCardDeckDTO> Repository { get; }
  public ICardAPI<MTGCard> CardAPI { get; }

  public override async Task<MTGCardDeck> Execute(string loadName)
  {
    return !string.IsNullOrEmpty(loadName) switch
    {
      true => await new GetDeck(Repository, CardAPI).Execute(loadName),
      _ => null,
    };
  }
}
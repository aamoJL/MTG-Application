using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.DeckRepository;

public class AddOrUpdateDeck : UseCase<(MTGCardDeck deck, string saveName), Task<bool>>
{
  public AddOrUpdateDeck(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; }

  public override async Task<bool> Execute((MTGCardDeck deck, string saveName) args)
  {
    var (deck, saveName) = args;

    return await Repository.AddOrUpdate(new MTGCardDeckDTO(deck) { Name = saveName });
  }
}

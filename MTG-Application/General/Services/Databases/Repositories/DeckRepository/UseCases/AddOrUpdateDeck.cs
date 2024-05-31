using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.DeckRepository;

public class AddOrUpdateDeck(IRepository<MTGCardDeckDTO> repository) : UseCase<(MTGCardDeck deck, string saveName), Task<bool>>
{
  public override async Task<bool> Execute((MTGCardDeck deck, string saveName) args)
  {
    var (deck, saveName) = args;

    return await repository.AddOrUpdate(new MTGCardDeckDTO(deck) { Name = saveName });
  }
}

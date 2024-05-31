using MTGApplication.General.Extensions;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.DeckRepository;
public class DeleteDeck(IRepository<MTGCardDeckDTO> repository) : UseCase<MTGCardDeck, Task<bool>>
{
  public override async Task<bool> Execute(MTGCardDeck deck) => await repository.Delete(new(deck));

  public async Task<bool> Execute(string deckName)
    => await repository.Delete(await repository.Get(deckName, []));
}
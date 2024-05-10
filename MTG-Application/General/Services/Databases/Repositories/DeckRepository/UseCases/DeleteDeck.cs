using MTGApplication.General.Extensions;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.DeckRepository;
public class DeleteDeck : UseCase<MTGCardDeck, Task<bool>>
{
  public DeleteDeck(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; }

  public override async Task<bool> Execute(MTGCardDeck deck) => await Repository.Delete(new(deck));

  public async Task<bool> Execute(string deckName)
    => await Repository.Delete(await Repository.Get(deckName, ExpressionExtensions.EmptyArray<MTGCardDeckDTO>()));
}
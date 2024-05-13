using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public class DeleteDeck : UseCase<MTGCardDeck, Task<bool>>
{
  public DeleteDeck(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; }

  public override async Task<bool> Execute(MTGCardDeck deck)
    => await new General.Databases.Repositories.DeckRepository.DeleteDeck(Repository).Execute(deck);
}
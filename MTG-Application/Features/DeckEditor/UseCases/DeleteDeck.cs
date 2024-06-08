using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.UseCases;

public class DeleteDeck(IRepository<MTGCardDeckDTO> repository) : UseCase<MTGCardDeck, Task<bool>>
{
  public IRepository<MTGCardDeckDTO> Repository { get; } = repository;

  public override async Task<bool> Execute(MTGCardDeck deck)
    => await new DeleteDeck(Repository).Execute(deck);
}
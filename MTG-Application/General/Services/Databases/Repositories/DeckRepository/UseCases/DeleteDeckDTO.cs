using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
public class DeleteDeckDTO(IRepository<MTGCardDeckDTO> repository) : UseCase<MTGCardDeckDTO, Task<bool>>
{
  public override async Task<bool> Execute(MTGCardDeckDTO deck)
    => await repository.Delete(deck);

  public async Task<bool> Execute(string deckName)
    => await repository.Delete(await repository.Get(deckName, (set) => { }));
}
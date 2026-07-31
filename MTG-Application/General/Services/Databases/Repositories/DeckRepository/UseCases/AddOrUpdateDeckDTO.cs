using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;

public class AddOrUpdateDeckDTO(IRepository<MTGCardDeckDTO> repository) : UseCaseFunc<MTGCardDeckDTO, Task<bool>>
{
  public override async Task<bool> Execute(MTGCardDeckDTO deck)
    => await repository.AddOrUpdate(deck);
}

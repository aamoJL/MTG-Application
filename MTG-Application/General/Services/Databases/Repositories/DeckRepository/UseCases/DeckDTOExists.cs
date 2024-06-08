using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;

public class DeckDTOExists(IRepository<MTGCardDeckDTO> repository) : UseCase<string, Task<bool>>
{
  public async override Task<bool> Execute(string name) => await repository.Exists(name);
}

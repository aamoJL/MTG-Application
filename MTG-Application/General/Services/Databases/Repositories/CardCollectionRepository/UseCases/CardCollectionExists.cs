using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.CardCollection;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;

public class CardCollectionExists(IRepository<MTGCardCollectionDTO> repository) : UseCase<string, Task<bool>>
{
  public async override Task<bool> Execute(string name) => await repository.Exists(name);
}

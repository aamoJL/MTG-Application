using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.CardCollection;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;

public class DeleteCardCollection(IRepository<MTGCardCollectionDTO> repository) : UseCase<MTGCardCollection, Task<bool>>
{
  public override async Task<bool> Execute(MTGCardCollection collection) => await repository.Delete(new(collection));

  public async Task<bool> Execute(string name) => await repository.Delete(await repository.Get(name, []));
}
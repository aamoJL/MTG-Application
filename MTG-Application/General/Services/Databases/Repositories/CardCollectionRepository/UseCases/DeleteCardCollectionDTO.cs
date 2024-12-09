using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;

public class DeleteCardCollectionDTO(IRepository<MTGCardCollectionDTO> repository) : UseCase<MTGCardCollectionDTO, Task<bool>>
{
  public override async Task<bool> Execute(MTGCardCollectionDTO collection) => await repository.Delete(collection);

  public async Task<bool> Execute(string name)
  {
    return await repository.Get(name, RepositoryUtilities<MTGCardCollectionDTO>.EmptyIncludes) is MTGCardCollectionDTO item
      && await repository.Delete(item);
  }
}
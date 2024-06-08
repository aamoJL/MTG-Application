using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;

public class AddOrUpdateCardCollectionDTO(IRepository<MTGCardCollectionDTO> repository) : UseCase<(MTGCardCollectionDTO collection, string saveName), Task<bool>>
{
  public override async Task<bool> Execute((MTGCardCollectionDTO collection, string saveName) args)
  {
    var (collection, saveName) = args;

    return await repository.AddOrUpdate(collection with { Name = saveName });
  }
}
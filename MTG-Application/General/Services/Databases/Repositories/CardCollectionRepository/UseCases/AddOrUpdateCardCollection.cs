using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.CardCollection;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;

public class AddOrUpdateCardCollection(IRepository<MTGCardCollectionDTO> repository) : UseCase<(MTGCardCollection collection, string saveName), Task<bool>>
{
  public override async Task<bool> Execute((MTGCardCollection collection, string saveName) args)
  {
    var (collection, saveName) = args;

    return await repository.AddOrUpdate(new MTGCardCollectionDTO(collection) { Name = saveName });
  }
}
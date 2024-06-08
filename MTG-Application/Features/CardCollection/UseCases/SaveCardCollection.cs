using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;

public class SaveCardCollection(IRepository<MTGCardCollectionDTO> repository) : UseCase<SaveCardCollection.SaveArgs, Task<bool>>
{
  public record SaveArgs(MTGCardCollection Collection, string SaveName, bool OverrideOld = false);

  public override async Task<bool> Execute(SaveArgs args)
  {
    var (collection, saveName, overrideOld) = args;

    var oldName = collection.Name;

    if (oldName != saveName && await new CardCollectionDTOExists(repository).Execute(saveName) && !overrideOld)
      return false; // Cancel because overriding is not enabled

    if (await new AddOrUpdateCardCollectionDTO(repository).Execute((collection, saveName)) is bool wasSaved && wasSaved is true)
    {
      collection.Name = saveName;

      if (!string.IsNullOrEmpty(oldName) && oldName != saveName && await new CardCollectionDTOExists(repository).Execute(oldName))
        await new DeleteCardCollectionDTO(repository).Execute(oldName);
    }
    return wasSaved;
  }
}

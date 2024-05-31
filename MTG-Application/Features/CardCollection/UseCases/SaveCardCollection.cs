using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.CardCollection;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection;

public class SaveCardCollection(IRepository<MTGCardCollectionDTO> repository) : UseCase<SaveCardCollection.SaveArgs, Task<bool>>
{
  public record SaveArgs(MTGCardCollection Collection, string SaveName, bool OverrideOld = false);

  public override async Task<bool> Execute(SaveArgs args)
  {
    var (collection, saveName, overrideOld) = args;

    var oldName = collection.Name;

    if (oldName != saveName && await new CardCollectionExists(repository).Execute(saveName) && !overrideOld)
      return false; // Cancel because overriding is not enabled

    if (await new AddOrUpdateCardCollection(repository).Execute((collection, saveName)) is bool wasSaved && wasSaved is true)
    {
      collection.Name = saveName;

      if (!string.IsNullOrEmpty(oldName) && oldName != saveName && await new CardCollectionExists(repository).Execute(oldName))
        await new DeleteCardCollection(repository).Execute(oldName);
    }
    return wasSaved;
  }
}

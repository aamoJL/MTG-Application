using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.Features.CardCollectionEditor.Models.Converters;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.UseCases;

public class SaveCardCollection(IRepository<MTGCardCollectionDTO> repository) : UseCaseFunc<MTGCardCollection, string, bool, Task<bool>>
{
  public IRepository<MTGCardCollectionDTO> Repository { get; } = repository;

  public override async Task<bool> Execute(MTGCardCollection collection, string name, bool overrideOld)
  {
    var dto = CardCollectionToDTOConverter.Convert(collection);
    var oldName = dto.Name;

    if (oldName != name && await new CardCollectionDTOExists(Repository).Execute(name) && !overrideOld)
      return false; // Cancel because overriding is not enabled

    if (!await new AddOrUpdateCardCollectionDTO(Repository).Execute((dto, name)))
      return false; // Cancel because was not saved

    if (!string.IsNullOrEmpty(oldName) && oldName != name && await new CardCollectionDTOExists(Repository).Execute(oldName))
      await new DeleteCardCollectionDTO(Repository).Execute(oldName); // Delete old collection if it was renamed

    return true;
  }
}
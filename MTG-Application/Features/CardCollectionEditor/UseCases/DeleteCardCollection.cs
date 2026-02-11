using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.Features.CardCollectionEditor.Models.Converters;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.UseCases;

public class DeleteCardCollection(IRepository<MTGCardCollectionDTO> repository) : UseCaseFunc<MTGCardCollection, Task<bool>>
{
  public IRepository<MTGCardCollectionDTO> Repository { get; } = repository;

  public override async Task<bool> Execute(MTGCardCollection collection)
  {
    var dto = CardCollectionToDTOConverter.Convert(collection);

    return await new DeleteCardCollectionDTO(Repository).Execute(dto);
  }
}
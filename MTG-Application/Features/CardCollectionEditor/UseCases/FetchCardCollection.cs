using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.Features.CardCollectionEditor.Models.Converters;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.UseCases;

public class FetchCardCollection(IRepository<MTGCardCollectionDTO> repository, IMTGCardImporter importer) : UseCaseFunc<string, Task<MTGCardCollection>>
{
  public IRepository<MTGCardCollectionDTO> Repository { get; } = repository;
  public IMTGCardImporter Importer { get; } = importer;

  public override async Task<MTGCardCollection> Execute(string name)
  {
    return await new GetCardCollectionDTO(Repository).Execute(name) is MTGCardCollectionDTO dto
          ? await new DTOToCardCollectionConverter(Importer).Convert(dto)
          : throw new KeyNotFoundException();
  }
}
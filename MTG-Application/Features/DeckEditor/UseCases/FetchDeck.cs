using Microsoft.Extensions.Caching.Memory;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.Models.Converters;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.UseCases;

public class FetchDeck(IRepository<MTGCardDeckDTO> repository, IMTGCardImporter importer) : UseCaseFunc<string, Task<DeckEditorMTGDeck>>
{
  public IMemoryCache? Cache { get; init; } = null;

  public override async Task<DeckEditorMTGDeck> Execute(string name)
  {
    return await new GetDeckDTO(repository).Execute(name) is MTGCardDeckDTO dto
      ? await new DTOToDeckEditorDeckConverter(importer) { Cache = Cache }.Convert(dto)
      : throw new KeyNotFoundException();
  }
}
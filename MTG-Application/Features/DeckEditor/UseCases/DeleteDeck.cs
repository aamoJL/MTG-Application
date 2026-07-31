using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.Models.Converters;
using MTGApplication.General.Services.Cache;
using MTGApplication.General.Services.Cache.Caches;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.UseCases;

public class DeleteDeck(IRepository<MTGCardDeckDTO> repository) : UseCaseFunc<DeckEditorMTGDeck, Task<bool>>
{
  public IMemoryCache<Caching.CacheKey>? Cache { get; init; } = null;

  public override async Task<bool> Execute(DeckEditorMTGDeck deck)
  {
    var result = await new DeleteDeckDTO(repository).Execute(DeckEditorMTGDeckToDTOConverter.Convert(deck));

    Cache?.UnCacheDeck(deck.Name);

    return result;
  }
}

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

public class SaveDeck(IRepository<MTGCardDeckDTO> repository) : UseCaseFunc<DeckEditorMTGDeck, string, bool, Task<bool>>
{
  public IMemoryCache<Caching.CacheKey>? Cache { get; init; } = null;

  public override async Task<bool> Execute(DeckEditorMTGDeck deck, string name, bool overrideOld)
  {
    var oldName = deck.Name;

    if (oldName != name && await new DeckDTOExists(repository).Execute(name) && !overrideOld)
      return false; // Cancel because overriding is not enabled

    if (!await new AddOrUpdateDeckDTO(repository).Execute(DeckEditorMTGDeckToDTOConverter.Convert(deck) with { Name = name }))
      return false; // Cancel because was not saved

    deck.Name = name;

    if (!string.IsNullOrEmpty(oldName) && oldName != name && await new DeckDTOExists(repository).Execute(oldName))
    {
      await new DeleteDeckDTO(repository).Execute(oldName); // Delete old deck if it was renamed
      Cache?.UnCacheDeck(oldName);
    }

    Cache?.CacheDeck(deck);

    return true;
  }
}
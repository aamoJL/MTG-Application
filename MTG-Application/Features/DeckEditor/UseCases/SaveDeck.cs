using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.DeckRepository;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.SaveDeck;

namespace MTGApplication.Features.DeckEditor;
public class SaveDeck(IRepository<MTGCardDeckDTO> repository) : UseCase<SaveArgs, Task<bool>>
{
  public record SaveArgs(MTGCardDeck Deck, string SaveName, bool OverrideOld = false);

  public override async Task<bool> Execute(SaveArgs args)
  {
    var (deck, saveName, overrideOld) = args;

    var oldName = deck.Name;

    if (oldName != saveName && await new DeckExists(repository).Execute(saveName) && !overrideOld)
      return false; // Cancel because overriding is not enabled

    if (await new AddOrUpdateDeck(repository).Execute((deck, saveName)) is bool wasSaved && wasSaved is true)
    {
      deck.Name = saveName;

      if (oldName != saveName && await new DeckExists(repository).Execute(oldName) && !string.IsNullOrEmpty(oldName))
        await new General.Databases.Repositories.DeckRepository.DeleteDeck(repository).Execute(oldName);
    }
    return wasSaved;
  }
}
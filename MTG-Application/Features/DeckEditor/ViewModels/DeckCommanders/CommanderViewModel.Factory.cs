using MTGApplication.Features.DeckEditor.Models;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCommanders;

public partial class CommanderViewModel
{
  public class Factory : Factory<CommanderViewModel>
  {
    public override CommanderViewModel Build(DeckEditorMTGDeck deck)
    {
      return new(deck)
      {
        Worker = Worker,
        UndoStack = UndoStack,
        Importer = Importer,
        EdhrecImporter = EdhrecImporter,
        ScryfallImporter = ScryfallImporter,
        Notifier = Notifier,
        NetworkService = NetworkService,
        Confirmers = Confirmers,
      };
    }
  }
}

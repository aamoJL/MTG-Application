using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;

namespace MTGApplication.Features.DeckEditor.Editor.Services.Factories;

public class DeckEditorCommanderFactory(DeckEditorViewModel viewmodel)
{
  public CommanderViewModel CreateCommanderViewModel(DeckEditorMTGCard? card)
  {
    return new(viewmodel.Importer)
    {
      Card = card,
      Confirmers = viewmodel.Confirmers.CommanderConfirmers,
      UndoStack = viewmodel.UndoStack,
      Notifier = viewmodel.Notifier,
      Worker = viewmodel
    };
  }
}

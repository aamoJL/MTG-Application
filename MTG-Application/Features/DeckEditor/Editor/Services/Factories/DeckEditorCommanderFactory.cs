using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using System;

namespace MTGApplication.Features.DeckEditor.Editor.Services.Factories;

public class DeckEditorCommanderFactory(DeckEditorViewModel viewmodel)
{
  public CommanderViewModel CreateCommanderViewModel(DeckEditorMTGCard? card, Action<DeckEditorMTGCard?>? onChange = null)
  {
    return new(viewmodel.Importer)
    {
      Card = card,
      Confirmers = viewmodel.Confirmers.CommanderConfirmers,
      UndoStack = viewmodel.UndoStack,
      Notifier = viewmodel.Notifier,
      Worker = viewmodel,
      OnChange = onChange
    };
  }
}

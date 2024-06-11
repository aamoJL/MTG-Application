﻿using MTGApplication.Features.DeckEditor.Services.Commanders;
using MTGApplication.General.Models.Card;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor;

public partial class CommanderViewModelCommands
{
  public class RemoveCommander(CommanderViewModel viewmodel) : ViewModelCommand<CommanderViewModel, DeckEditorMTGCard>(viewmodel)
  {
    protected override bool CanExecute(DeckEditorMTGCard param) => Viewmodel.Card != null;

    protected override void Execute(DeckEditorMTGCard param)
    {
      if (!CanExecute(param)) return;

      Viewmodel.UndoStack.PushAndExecute(
        new ReversibleCommanderChangeCommand(null, Viewmodel.Card, Viewmodel.CardCopier)
        {
          ReversibleAction = new CommanderViewModelReversibleActions.ReversibleChangeCommanderAction(Viewmodel)
        });
    }
  }
}
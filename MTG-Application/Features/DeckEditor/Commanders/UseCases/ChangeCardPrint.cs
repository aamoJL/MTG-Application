﻿using MTGApplication.Features.DeckEditor.Commanders.Services;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.Commanders.UseCases.ReversibleActions.CommanderViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.Commanders.UseCases;

public partial class CommanderViewModelCommands
{
  public class ChangeCardPrint(CommanderCommands viewmodel) : ViewModelAsyncCommand<CommanderCommands>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.GetCommander() != null;

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      var prints = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportFromUri(pageUri: Viewmodel.GetCommander().Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found.Select(x => x.Info);

      if (await Viewmodel.Confirmers.ChangeCardPrintConfirmer.Confirm(CommanderConfirmers.GetChangeCardPrintConfirmation(prints.Select(x => new MTGCard(x)))) is MTGCard selection)
      {
        if (selection.Info.ScryfallId == Viewmodel.GetCommander().Info.ScryfallId)
          return; // Same print

        Viewmodel.UndoStack.PushAndExecute(
          new ReversiblePropertyChangeCommand<DeckEditorMTGCard, MTGCardInfo>(Viewmodel.GetCommander(), Viewmodel.GetCommander().Info, selection.Info, Viewmodel.CardCopier)
          {
            ReversibleAction = new ReversibleCardPrintChangeAction(Viewmodel)
          });
      }
    }
  }
}
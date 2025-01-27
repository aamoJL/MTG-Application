using MTGApplication.Features.DeckEditor.Commanders.Services;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.Commanders.UseCases.ReversibleActions.CommanderViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.Commanders.UseCases;

public partial class CommanderViewModelCommands
{
  public class ChangeCardPrint(CommanderViewModel viewmodel) : ViewModelAsyncCommand<CommanderViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.Card != null;

    protected override async Task Execute()
    {
      if (!CanExecute())
        return;

      try
      {
        var commander = Viewmodel.Card!;
        var prints = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportWithUri(pageUri: commander.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found.Select(x => x.Info);

        if (await Viewmodel.Confirmers.ChangeCardPrintConfirmer.Confirm(CommanderConfirmers.GetChangeCardPrintConfirmation(prints.Select(x => new MTGCard(x)))) is MTGCard selection)
        {
          if (selection.Info.ScryfallId == commander.Info.ScryfallId)
            return; // Same print

          Viewmodel.UndoStack.PushAndExecute(
            new ReversiblePropertyChangeCommand<DeckEditorMTGCard, MTGCardInfo>(commander, commander.Info, selection.Info)
            {
              ReversibleAction = new ReversibleCardPrintChangeAction(Viewmodel)
            });
        }
      }
      catch (Exception e)
      {
        Viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, $"Error: {e.Message}"));
      }
    }
  }
}
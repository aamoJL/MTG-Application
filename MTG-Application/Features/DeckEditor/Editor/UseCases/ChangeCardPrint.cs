using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands
{
  public class ChangeCardPrint(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel, DeckEditorMTGCard>(viewmodel)
  {
    protected override bool CanExecute(DeckEditorMTGCard? card) => card != null;

    protected override async Task Execute(DeckEditorMTGCard? card)
    {
      if (!CanExecute(card))
        return;

      try
      {
        var prints = (await (Viewmodel as IWorker).DoWork(Viewmodel.Importer.ImportWithUri(pageUri: card!.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found.Select(x => x.Info);

        if (await Viewmodel.Confirmers.ChangeCardPrintConfirmer.Confirm(DeckEditorConfirmers.GetChangeCardPrintConfirmation(prints.Select(x => new MTGCard(x)))) is MTGCard selection)
        {
          if (selection.Info.ScryfallId == card!.Info.ScryfallId)
            return; // Same print

          Viewmodel.UndoStack.PushAndExecute(
            new ReversiblePropertyChangeCommand<DeckEditorMTGCard, MTGCardInfo>(card, card.Info, selection.Info)
            {
              ReversibleAction = new ReversibleCardPrintChangeAction()
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
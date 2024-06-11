using MTGApplication.Features.DeckEditor.Services.Commanders;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class CommanderViewModelCommands
{
  public class ChangeCardPrint(CommanderViewModel viewmodel) : ViewModelAsyncCommand<CommanderViewModel, DeckEditorMTGCard>(viewmodel)
  {
    protected override bool CanExecute(DeckEditorMTGCard card)
      => Viewmodel.Card != null && Viewmodel.Card.Info.Name != card.Info.Name;

    protected override async Task Execute(DeckEditorMTGCard card)
    {
      if (!CanExecute(card)) return;

      var prints = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportFromUri(pageUri: Viewmodel.Card.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found.Select(x => x.Info);

      if (await Viewmodel.Confirmers.ChangeCardPrintConfirmer.Confirm(CommanderConfirmers.GetChangeCardPrintConfirmation(prints)) is MTGCardInfo selection)
      {
        if (selection.ScryfallId == Viewmodel.Card.Info.ScryfallId)
          return; // Same print

        Viewmodel.UndoStack.PushAndExecute(
          new ReversiblePropertyChangeCommand<DeckEditorMTGCard, MTGCardInfo>(Viewmodel.Card, Viewmodel.Card.Info, selection, Viewmodel.CardCopier)
          {
            ReversibleAction = new CommanderViewModelReversibleActions.ReversibleCardPrintChangeAction(Viewmodel)
          });
      }
    }
  }
}
using MTGApplication.Features.DeckEditor.Commanders.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class CommanderViewModelCommands
{
  public class ChangeCardPrint(CommanderViewModel viewmodel) : ViewModelAsyncCommand<CommanderViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.GetModelAction?.Invoke() != null;

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      var prints = (await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportFromUri(pageUri: Viewmodel.GetModelAction?.Invoke().Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found.Select(x => x.Info);

      if (await Viewmodel.Confirmers.ChangeCardPrintConfirmer.Confirm(CommanderConfirmers.GetChangeCardPrintConfirmation(prints.Select(x => new MTGCard(x)))) is MTGCard selection)
      {
        if (selection.Info.ScryfallId == Viewmodel.GetModelAction?.Invoke().Info.ScryfallId)
          return; // Same print

        Viewmodel.UndoStack.PushAndExecute(
          new ReversiblePropertyChangeCommand<DeckEditorMTGCard, MTGCardInfo>(Viewmodel.GetModelAction?.Invoke(), Viewmodel.GetModelAction?.Invoke().Info, selection.Info, Viewmodel.CardCopier)
          {
            ReversibleAction = new CommanderViewModelReversibleActions.ReversibleCardPrintChangeAction(Viewmodel)
          });
      }
    }
  }
}
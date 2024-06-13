using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor;

public partial class CommanderViewModelReversibleActions
{
  public class ReversibleCardPrintChangeAction(CommanderViewModel viewmodel) : ViewModelReversibleAction<CommanderViewModel, (DeckEditorMTGCard Card, MTGCardInfo Print)>(viewmodel)
  {
    protected override void ActionMethod((DeckEditorMTGCard Card, MTGCardInfo Print) args)
      => ChangePrint(args.Print);

    protected override void ReverseActionMethod((DeckEditorMTGCard Card, MTGCardInfo Print) args)
      => ChangePrint(args.Print);

    private void ChangePrint(MTGCardInfo print)
    {
      if (Viewmodel.GetModelAction?.Invoke().Info.Name != print.Name) return;

      Viewmodel.OnChange?.Invoke(new DeckEditorMTGCard(print, Viewmodel.GetModelAction?.Invoke().Count ?? 1));
    }
  }
}
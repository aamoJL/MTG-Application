using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor.Commanders.UseCases.ReversibleActions;

public partial class CommanderViewModelReversibleActions
{
  public class ReversibleCardPrintChangeAction(CommanderCommands viewmodel) : ViewModelReversibleAction<CommanderCommands, (DeckEditorMTGCard Card, MTGCardInfo Print)>(viewmodel)
  {
    protected override void ActionMethod((DeckEditorMTGCard Card, MTGCardInfo Print) args)
      => ChangePrint(args.Print);

    protected override void ReverseActionMethod((DeckEditorMTGCard Card, MTGCardInfo Print) args)
      => ChangePrint(args.Print);

    private void ChangePrint(MTGCardInfo print)
    {
      if (Viewmodel.GetCommander() is not DeckEditorMTGCard card) return;

      if (card.Info.Name != print.Name)
        return;

      Viewmodel.OnChange?.Invoke(new DeckEditorMTGCard(print, card.Count));
    }
  }
}
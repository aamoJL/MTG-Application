using MTGApplication.General.Models.Card;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor;

public partial class CommanderViewModelReversibleActions
{
  public class ReversibleChangeCommanderAction(CommanderViewModel viewmodel) : ViewModelReversibleAction<CommanderViewModel, DeckEditorMTGCard>(viewmodel)
  {
    protected override void ActionMethod(DeckEditorMTGCard card)
      => ChangeCommander(card);

    protected override void ReverseActionMethod(DeckEditorMTGCard card)
      => ChangeCommander(card);

    private void ChangeCommander(DeckEditorMTGCard card)
    {
      Viewmodel.Card = card;
      Viewmodel.OnChange?.Invoke();
    }
  }
}
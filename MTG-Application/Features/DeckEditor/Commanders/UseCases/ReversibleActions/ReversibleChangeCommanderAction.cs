using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor.Commanders.UseCases.ReversibleActions;

public partial class CommanderViewModelReversibleActions
{
  public class ReversibleChangeCommanderAction(CommanderCommands viewmodel) : ViewModelReversibleAction<CommanderCommands, DeckEditorMTGCard>(viewmodel)
  {
    protected override void ActionMethod(DeckEditorMTGCard card)
      => ChangeCommander(card);

    protected override void ReverseActionMethod(DeckEditorMTGCard card)
      => ChangeCommander(card);

    private void ChangeCommander(DeckEditorMTGCard card)
      => Viewmodel.OnChange?.Invoke(card);
  }
}
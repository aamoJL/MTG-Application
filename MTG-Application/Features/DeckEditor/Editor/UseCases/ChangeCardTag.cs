using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands
{
  public record CardTagChangeArgs(DeckEditorMTGCard Card, CardTag? Value);

  public class ChangeCardTag(DeckEditorViewModel viewmodel) : SyncCommand<CardTagChangeArgs>
  {
    protected override void Execute(CardTagChangeArgs? args)
    {
      if (args == null) return;

      var (card, newValue) = args;

      viewmodel.UndoStack.PushAndExecute(
        new ReversiblePropertyChangeCommand<DeckEditorMTGCard, CardTag?>(card, card.CardTag, newValue)
        {
          ReversibleAction = new ReversibleCardTagChangeAction()
        });
    }
  }
}
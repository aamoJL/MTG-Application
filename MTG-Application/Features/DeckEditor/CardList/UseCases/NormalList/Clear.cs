using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Linq;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands
{
  public class Clear(IList<DeckEditorMTGCard> cards, ReversibleCommandStack undoStack) : SyncCommand
  {
    private IList<DeckEditorMTGCard> Cards { get; } = cards;
    private ReversibleCommandStack UndoStack { get; } = undoStack;

    protected override bool CanExecute() => Cards.Any();

    protected override void Execute()
    {
      if (!CanExecute())
        return;

      UndoStack.PushAndExecute(
        new ReversibleCollectionCommand<DeckEditorMTGCard>(Cards)
        {
          ReversibleAction = new ReversibleRemoveCardsAction(Cards)
        });
    }
  }
}
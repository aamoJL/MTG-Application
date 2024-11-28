using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardGroupViewModelCommands
{
  public IRelayCommand<DeckEditorMTGCard> BeginMoveFromCommand { get; } = new MoveGroupCard.BeginMoveFrom(listViewmodel).Command;
  public IAsyncRelayCommand<DeckEditorMTGCard> BeginMoveToCommand { get; } = new MoveGroupCard.BeginMoveTo(groupViewmodel, listViewmodel).Command;
  public IRelayCommand<DeckEditorMTGCard> ExecuteMoveCommand { get; } = new MoveGroupCard.ExecuteMove(listViewmodel).Command;

  public class MoveGroupCard
  {
    public class BeginMoveFrom(GroupedCardListViewModel listViewmodel) : CardListViewModelCommands.MoveCard.BeginMoveFrom(listViewmodel);

    public class BeginMoveTo(CardGroupViewModel viewmodel, GroupedCardListViewModel listViewmodel) : ViewModelAsyncCommand<CardGroupViewModel, DeckEditorMTGCard>(viewmodel)
    {
      protected override async Task Execute(DeckEditorMTGCard? card)
      {
        if (card == null)
          return;

        var combinedCommands = listViewmodel.UndoStack.ActiveCombinedCommand.Commands;

        if (listViewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
        {
          if (existingCard == card)
          {
            // Card is from the same list
            // Cancel move and change the group
            listViewmodel.UndoStack.ActiveCombinedCommand.Cancel();

            listViewmodel.UndoStack.PushAndExecute(
              new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(existingCard, existingCard.Group, Viewmodel.Key)
              {
                ReversibleAction = new ReversibleCardGroupChangeAction(listViewmodel)
              });
          }
          else if (await listViewmodel.Confirmers.AddSingleConflictConfirmer.Confirm(CardListConfirmers.GetAddSingleConflictConfirmation(card.Info.Name)) is ConfirmationResult.Yes)
          {
            combinedCommands.Add(new CombinedReversibleCommand()
            {
              Commands = [
                new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(existingCard, existingCard.Count, card.Count + existingCard.Count)
                {
                  ReversibleAction = new ReversibleCardCountChangeAction(listViewmodel)
                },
                new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(existingCard, existingCard.Group, Viewmodel.Key)
                {
                  ReversibleAction = new ReversibleCardGroupChangeAction(listViewmodel)
                }]
            });
          }
          else
            listViewmodel.UndoStack.ActiveCombinedCommand.Cancel();
        }
        else
        {
          // Card not in the list, add
          combinedCommands.Add(new CombinedReversibleCommand()
          {
            Commands = [
              new ReversibleCollectionCommand<DeckEditorMTGCard>(card)
              {
                ReversibleAction = new ReversibleAddCardAction(listViewmodel)
              },
              new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(card, card.Group, Viewmodel.Key)
              {
                ReversibleAction = new ReversibleCardGroupChangeAction(listViewmodel)
              }]
          });
        }
      }
    }

    public class ExecuteMove(GroupedCardListViewModel listViewmodel) : CardListViewModelCommands.MoveCard.ExecuteMove(listViewmodel);
  }
}
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
  public IRelayCommand<DeckEditorMTGCard> BeginMoveFromCommand { get; } = new MoveGroupCard.BeginMoveFrom(groupViewmodel, listViewmodel).Command;
  public IAsyncRelayCommand<DeckEditorMTGCard> BeginMoveToCommand { get; } = new MoveGroupCard.BeginMoveTo(groupViewmodel, listViewmodel).Command;
  public IRelayCommand<DeckEditorMTGCard> ExecuteMoveCommand { get; } = new MoveGroupCard.ExecuteMove(groupViewmodel, listViewmodel).Command;

  private class MoveGroupCard
  {
    public class BeginMoveFrom(CardGroupViewModel viewmodel, GroupedCardListViewModel listViewmodel) : ViewModelCommand<CardGroupViewModel, DeckEditorMTGCard>(viewmodel)
    {
      protected override void Execute(DeckEditorMTGCard card)
      {
        if (listViewmodel.BeginMoveFromCommand.CanExecute(card))
          listViewmodel.BeginMoveFromCommand.Execute(card);
      }
    }

    public class BeginMoveTo(CardGroupViewModel viewmodel, GroupedCardListViewModel listViewmodel) : ViewModelAsyncCommand<CardGroupViewModel, DeckEditorMTGCard>(viewmodel)
    {
      protected override async Task Execute(DeckEditorMTGCard card)
      {
        var combinedCommands = listViewmodel.UndoStack.ActiveCombinedCommand.Commands;

        if (listViewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existing)
        {
          // Card already in the list, confirm and/or add
          if (existing == card
            || await listViewmodel.Confirmers.AddSingleConflictConfirmer.Confirm(CardListConfirmers.GetAddSingleConflictConfirmation(card.Info.Name)) is ConfirmationResult.Yes)
          {
            combinedCommands.Add(
              new ReversibleCollectionCommand<DeckEditorMTGCard>(card, listViewmodel.CardCopier)
              {
                ReversibleAction = new ReversibleAddCardAction(listViewmodel)
              });

            // Change group if needed
            if (card.Group != Viewmodel.Key)
            {
              combinedCommands.Add(new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(card, card.Group, Viewmodel.Key, listViewmodel.CardCopier)
              {
                ReversibleAction = new ReversibleCardGroupChangeAction(listViewmodel)
              });
            }
          }
          else
            listViewmodel.UndoStack.ActiveCombinedCommand.Cancel();
        }
        else
        {
          // Card not in the list, add
          card.Group = Viewmodel.Key;

          combinedCommands.Add(
            new ReversibleCollectionCommand<DeckEditorMTGCard>(card, listViewmodel.CardCopier)
            {
              ReversibleAction = new ReversibleAddCardAction(listViewmodel)
            });
        }
      }
    }

    public class ExecuteMove(CardGroupViewModel viewmodel, GroupedCardListViewModel listViewmodel) : ViewModelCommand<CardGroupViewModel, DeckEditorMTGCard>(viewmodel)
    {
      protected override void Execute(DeckEditorMTGCard card)
      {
        if (listViewmodel.ExecuteMoveCommand.CanExecute(card))
          listViewmodel.ExecuteMoveCommand.Execute(card);
      }
    }
  }
}
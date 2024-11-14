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
  public IAsyncRelayCommand<DeckEditorMTGCard> AddCardToGroupCommand { get; } = new AddCardToGroup(groupViewmodel, listViewmodel).Command;

  private class AddCardToGroup(CardGroupViewModel viewmodel, GroupedCardListViewModel listViewmodel) : ViewModelAsyncCommand<CardGroupViewModel, DeckEditorMTGCard>(viewmodel)
  {
    protected override async Task Execute(DeckEditorMTGCard card)
    {
      if (listViewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existing)
      {
        if (await listViewmodel.Confirmers.AddSingleConflictConfirmer.Confirm(CardListConfirmers.GetAddSingleConflictConfirmation(card.Info.Name)) is ConfirmationResult.Yes)
        {
          var combinedCommand = new CombinedReversibleCommand()
          {
            Commands = [
              new ReversibleCollectionCommand<DeckEditorMTGCard>(card, listViewmodel.CardCopier)
              {
                ReversibleAction = new ReversibleAddCardAction(listViewmodel)
              }]
          };

          // Change group if needed
          if (existing.Group != Viewmodel.Key)
          {
            combinedCommand.Commands.Add(new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(card, existing.Group, Viewmodel.Key, listViewmodel.CardCopier)
            {
              ReversibleAction = new ReversibleCardGroupChangeAction(listViewmodel)
            });
          }

          listViewmodel.UndoStack.PushAndExecute(combinedCommand);
        }
      }
      else
      {
        card.Group = Viewmodel.Key;

        listViewmodel.UndoStack.PushAndExecute(new ReversibleCollectionCommand<DeckEditorMTGCard>(card, listViewmodel.CardCopier)
        {
          ReversibleAction = new ReversibleAddCardAction(listViewmodel)
        });
      }
    }
  }
}
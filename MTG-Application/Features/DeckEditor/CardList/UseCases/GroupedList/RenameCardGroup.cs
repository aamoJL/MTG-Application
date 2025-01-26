using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class GroupedCardListViewModelCommands
{
  public class RenameCardGroup(GroupedCardListViewModel viewmodel) : ViewModelAsyncCommand<GroupedCardListViewModel, CardGroupViewModel>(viewmodel)
  {
    protected override bool CanExecute(CardGroupViewModel? group) => !string.IsNullOrEmpty(group?.Key);

    protected override async Task Execute(CardGroupViewModel? group)
    {
      if (!CanExecute(group))
        return;

      if (await Viewmodel.Confirmers.RenameCardGroupConfirmer.Confirm(GroupedCardListConfirmers.GetRenameCardGroupConfirmation(group!.Key)) is string newKey
        && !string.IsNullOrEmpty(newKey)
        && newKey != group.Key)
      {
        if (Viewmodel.Groups.TryFindIndex(x => x.Key == newKey, out var index))
        {
          // Confirm merge
          if (await Viewmodel.Confirmers.MergeCardGroupsConfirmer.Confirm(GroupedCardListConfirmers.GetMergeCardGroupsConfirmation(group.Key))
            is General.Services.ConfirmationService.ConfirmationResult.Yes)
          {
            Viewmodel.UndoStack.PushAndExecute(new CombinedReversibleCommand()
            {
              Commands = [
                .. group.Cards.Select(x => new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(x, x.Group, newKey)
                {
                  ReversibleAction = new ReversibleCardGroupChangeAction()
                }),
                new ReversibleCommand<CardGroupViewModel>(group)
                {
                  ReversibleAction = new ReversibleRemoveGroupAction(Viewmodel)
                }
              ]
            });
          }
        }
        else
        {
          Viewmodel.UndoStack.PushAndExecute(
            new ReversiblePropertyChangeCommand<CardGroupViewModel, string>(group, group.Key, newKey)
            {
              ReversibleAction = new ReversibleRenameGroupAction()
            });
        }
      }
    }
  }
}
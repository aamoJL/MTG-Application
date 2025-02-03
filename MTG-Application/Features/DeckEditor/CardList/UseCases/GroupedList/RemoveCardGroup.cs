using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class GroupedCardListViewModelCommands
{
  public class RemoveCardGroup(GroupedCardListViewModel viewmodel) : ViewModelCommand<GroupedCardListViewModel, CardGroupViewModel>(viewmodel)
  {
    protected override bool CanExecute(CardGroupViewModel? group) => !string.IsNullOrEmpty(group?.Key);

    protected override void Execute(CardGroupViewModel? group)
    {
      if (!CanExecute(group))
        return;

      if (Viewmodel.Groups.Contains(group!))
      {
        Viewmodel.UndoStack.PushAndExecute(
          new ReversibleCommand<CardGroupViewModel>(group!)
          {
            ReversibleAction = new ReversibleRemoveGroupAction(Viewmodel)
          });

        new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Success, "Group removed successfully."));
      }
      else
        new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Error, "Group was not found."));
    }
  }
}
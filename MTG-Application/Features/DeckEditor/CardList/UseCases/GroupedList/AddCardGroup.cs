using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.CardList.Services.Factories;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class GroupedCardListViewModelCommands
{
  public class AddCardGroup(GroupedCardListViewModel viewmodel) : ViewModelAsyncCommand<GroupedCardListViewModel, string>(viewmodel)
  {
    protected override async Task Execute(string? key)
    {
      key ??= await Viewmodel.Confirmers.AddCardGroupConfirmer.Confirm(GroupedCardListConfirmers.GetAddCardGroupConfirmation());

      if (string.IsNullOrEmpty(key))
        return;

      if (Viewmodel.Groups.Any(x => x.Key == key))
      {
        new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Error, "Group already exists."));
        return;
      }

      Viewmodel.UndoStack.PushAndExecute(
        new ReversibleCommand<CardGroupViewModel>(new GroupedCardListCardGroupFactory(Viewmodel).CreateCardGroup(key))
        {
          ReversibleAction = new ReversibleAddGroupAction(Viewmodel)
        });

      new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Success, "Group added successfully."));
    }
  }
}
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Linq;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class GroupedCardListViewModelCommands
{
  public IRelayCommand RemoveGroupCommand { get; } = new RemoveCardGroup(viewmodel).Command;

  private class RemoveCardGroup(GroupedCardListViewModel viewmodel) : ViewModelCommand<GroupedCardListViewModel, string>(viewmodel)
  {
    protected override bool CanExecute(string key) => !string.IsNullOrEmpty(key);

    protected override void Execute(string key)
    {
      if (!CanExecute(key))
        return;

      var group = Viewmodel.Groups.FirstOrDefault(x => x.Key == key);

      if (group == null)
      {
        new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Error, "Group was not found."));
        return;
      }

      Viewmodel.UndoStack.PushAndExecute(
        new ReversibleCommand<string>(key)
        {
          ReversibleAction = new ReversibleRemoveGroupAction(Viewmodel)
        });

      new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Success, "Group removed successfully."));
    }
  }

  private class GroupCopier : IClassCopier<CardGroupViewModel>
  {
    public CardGroupViewModel Copy(CardGroupViewModel item)
    {
      var group = new CardGroupViewModel(item.Key);

      foreach (var card in item.Items)
        group.Items.Add(card);

      return group;
    }

    public IEnumerable<CardGroupViewModel> Copy(IEnumerable<CardGroupViewModel> items)
      => items.Select(Copy);
  }
}
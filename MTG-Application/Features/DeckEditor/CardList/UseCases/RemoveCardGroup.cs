using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System.Linq;
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

      Viewmodel.Groups.Remove(group);

      foreach (var card in group.Items)
      {
        card.Group = string.Empty;
        Viewmodel.OnCardChange(card, nameof(card.Group));
      }

      new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Success, "Group removed successfully."));
    }
  }
}
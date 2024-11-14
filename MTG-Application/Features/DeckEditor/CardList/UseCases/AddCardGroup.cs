using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class GroupedCardListViewModelCommands
{
  public IAsyncRelayCommand<string> AddGroupCommand { get; } = new AddCardGroup(viewmodel).Command;

  private class AddCardGroup(GroupedCardListViewModel viewmodel) : ViewModelAsyncCommand<GroupedCardListViewModel, string>(viewmodel)
  {
    protected override async Task Execute(string key)
    {
      key ??= await Viewmodel.Confirmers.AddCardGroupConfirmer.Confirm(GroupedCardListConfirmers.GetAddCardGroupConfirmation());

      if (string.IsNullOrEmpty(key))
        return;

      if (Viewmodel.Groups.Any(x => x.Key == key))
      {
        new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Error, "Group already exists."));
        return;
      }

      // Find the alphabetical index of the key
      var index = Viewmodel.Groups.IndexOf(Viewmodel.Groups.FirstOrDefault(x =>
      {
        // Empty key will always be the last item
        return x.Key == string.Empty || x.Key.CompareTo(key) >= 0;
      }));

      var group = new CardGroupViewModel(key);
      group.Commands = new(group, viewmodel);

      if (index >= 0)
        Viewmodel.Groups.Insert(index, group);
      else
        Viewmodel.Groups.Add(group);

      new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Success, "Group added successfully."));
    }
  }
}
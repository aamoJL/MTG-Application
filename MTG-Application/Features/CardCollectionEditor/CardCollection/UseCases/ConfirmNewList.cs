using MTGApplication.Features.CardCollection.Services;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;

public partial class CardCollectionPageViewModelCommands
{
  public class ConfirmNewList(CardCollectionViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionViewModel>(viewmodel)
  {
    protected override async Task Execute()
    {
      if (await Viewmodel.Confirmers.NewCollectionListConfirmer.Confirm(CardCollectionConfirmers.GetNewCollectionListConfirmation())
      is not (string name, string query))
        return;

      if (string.IsNullOrEmpty(name))
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.NewListNameError);
      else if (string.IsNullOrEmpty(query))
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.NewListQueryError);
      else if (Viewmodel.CollectionLists.FirstOrDefault(x => x.Name == name) is not null)
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.NewListExistsError);
      else
      {
        var newList = new MTGCardCollectionList() { Name = name, SearchQuery = query };
        Viewmodel.CollectionLists.Add(newList);
        Viewmodel.HasUnsavedChanges = true;

        Viewmodel.OnListAdded?.Invoke(newList);

        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.NewListSuccess);
      }
    }
  }
}
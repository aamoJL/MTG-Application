using MTGApplication.Features.CardCollection.Services;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;

public partial class CardCollectionViewModelCommands
{
  public class EditList(CardCollectionViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.SelectedList != null;

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      if (await Viewmodel.Confirmers.EditCollectionListConfirmer.Confirm(
        CardCollectionConfirmers.GetEditCollectionListConfirmation((Viewmodel.SelectedList.Name, Viewmodel.SelectedList.SearchQuery)))
        is not (string name, string query) args)
        return;

      if (string.IsNullOrEmpty(name))
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.EditListNameError);
      else if (string.IsNullOrEmpty(query))
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.EditListQueryError);
      else if (Viewmodel.SelectedList.Name != name && Viewmodel.Collection.CollectionLists.FirstOrDefault(x => x.Name == name) is not null)
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.EditListExistsError);
      else
      {
        Viewmodel.SelectedList.Name = name;
        Viewmodel.SelectedList.SearchQuery = query;
        Viewmodel.HasUnsavedChanges = true;

        await Viewmodel.Worker.DoWork(Viewmodel.QueryCardsViewModel.UpdateQueryCards(Viewmodel.SelectedList?.SearchQuery ?? string.Empty));

        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.EditListSuccess);
      }
    }
  }
}
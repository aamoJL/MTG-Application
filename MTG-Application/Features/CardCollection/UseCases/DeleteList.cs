using MTGApplication.Features.CardCollection.Services;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;

public partial class CardCollectionViewModelCommands
{
  public class DeleteList(CardCollectionViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.SelectedList != null;

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      if (await Viewmodel.Confirmers.DeleteCollectionListConfirmer.Confirm(
        CardCollectionConfirmers.GetDeleteCollectionListConfirmation(Viewmodel.SelectedList.Name))
        is not ConfirmationResult.Yes)
        return;

      if (Viewmodel.Collection.CollectionLists.Remove(Viewmodel.SelectedList))
      {
        Viewmodel.HasUnsavedChanges = true;

        await Viewmodel.SelectListCommand.ExecuteAsync(Viewmodel.Collection.CollectionLists.FirstOrDefault());

        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteListSuccess);
      }
      else
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteListError);
    }
  }
}
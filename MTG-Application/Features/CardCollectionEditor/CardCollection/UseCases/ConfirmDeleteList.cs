using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollection.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.CardCollection.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ConfirmDeleteList(CardCollectionViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionViewModel, MTGCardCollectionList>(viewmodel)
  {
    protected override bool CanExecute(MTGCardCollectionList list) => Viewmodel.CollectionLists.Contains(list);

    protected override async Task Execute(MTGCardCollectionList list)
    {
      if (!CanExecute(list))
      {
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteListNotFoundError);
        return;
      }

      if (await Viewmodel.Confirmers.DeleteCollectionListConfirmer.Confirm(
        CardCollectionConfirmers.GetDeleteCollectionListConfirmation(Viewmodel.Name))
        is not ConfirmationResult.Yes)
        return;

      if (Viewmodel.CollectionLists.Remove(list))
      {
        Viewmodel.HasUnsavedChanges = true;

        Viewmodel.OnListRemoved?.Invoke(list);

        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteListSuccess);
      }
      else new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteListError);
    }
  }
}
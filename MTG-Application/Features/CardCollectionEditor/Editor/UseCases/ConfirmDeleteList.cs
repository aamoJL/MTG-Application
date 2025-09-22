using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ConfirmDeleteList(CardCollectionEditorViewModel viewmodel) : AsyncCommand<MTGCardCollectionList>
  {
    public CardCollectionEditorViewModel Viewmodel { get; } = viewmodel;

    protected override bool CanExecute(MTGCardCollectionList? list) => list != null && Viewmodel.Collection.CollectionLists.Contains(list);

    protected override async Task Execute(MTGCardCollectionList? list)
    {
      if (!CanExecute(list))
      {
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteListNotFoundError);
        return;
      }

      if (await Viewmodel.Confirmers.CardCollectionConfirmers.DeleteCollectionListConfirmer.Confirm(
        CardCollectionConfirmers.GetDeleteCollectionListConfirmation(Viewmodel.CollectionName))
        is not ConfirmationResult.Yes)
        return;

      if (Viewmodel.Collection.CollectionLists.Remove(list!))
      {
        Viewmodel.HasUnsavedChanges = true;

        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteListSuccess);
      }
      else
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteListError);
    }
  }
}
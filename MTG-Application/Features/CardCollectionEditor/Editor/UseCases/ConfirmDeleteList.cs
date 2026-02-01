using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ConfirmDeleteList(CardCollectionEditorViewModel viewmodel) : AsyncCommand
  {
    protected override bool CanExecute() => viewmodel.SelectedCardCollectionListViewModel.Name != string.Empty;

    protected override async Task Execute()
    {
      if (!CanExecute())
      {
        new SendNotification(viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteListNotFoundError);
        return;
      }

      if (await viewmodel.Confirmers.CardCollectionConfirmers.DeleteCollectionListConfirmer.Confirm(
        CardCollectionConfirmers.GetDeleteCollectionListConfirmation(viewmodel.CollectionName))
        is not ConfirmationResult.Yes)
        return;

      try
      {
        await viewmodel.Worker.DoWork(DeleteList(viewmodel.SelectedCardCollectionListViewModel.CollectionList));
        new SendNotification(viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteListSuccess);
      }
      catch
      {
        new SendNotification(viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteListError);
      }
    }

    private async Task DeleteList(MTGCardCollectionList list)
    {
      if (viewmodel.CollectionLists.Remove(list))
      {
        await viewmodel.SelectedCardCollectionListViewModel.ChangeCollectionList(viewmodel.CollectionLists.FirstOrDefault() ?? new());
      }
      else
        throw new();
    }
  }
}
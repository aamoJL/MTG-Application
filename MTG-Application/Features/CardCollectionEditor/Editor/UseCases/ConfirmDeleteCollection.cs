using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services.Converters;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ConfirmDeleteCollection(CardCollectionEditorViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => !string.IsNullOrEmpty(Viewmodel.CollectionName);

    protected override async Task Execute()
    {
      if (!CanExecute())
        return;

      if ((await Viewmodel.Confirmers.CardCollectionConfirmers.DeleteCollectionConfirmer.Confirm(
        CardCollectionConfirmers.GetDeleteCollectionConfirmation(Viewmodel.CollectionName)))
        is not ConfirmationResult.Yes)
        return; // Cancel

      var dto = CardCollectionToDTOConverter.Convert(Viewmodel.Collection);

      if (await (Viewmodel as IWorker).DoWork(new DeleteCardCollectionDTO(Viewmodel.Repository).Execute(dto)))
      {
        Viewmodel.Collection = new();
        Viewmodel.HasUnsavedChanges = false;

        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteCollectionSuccess);
      }
      else
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.DeletecollectionError);
    }
  }
}
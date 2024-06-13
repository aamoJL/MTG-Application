using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class DeckEditorViewModelCommands
{
  public class DeleteDeck(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => !string.IsNullOrEmpty(Viewmodel.Name);

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      if (await Viewmodel.Confirmers.DeleteDeckConfirmer.Confirm(DeckEditorConfirmers.GetDeleteDeckConfirmation(Viewmodel.Name))
        is not ConfirmationResult.Yes)
        return; // Cancel

      if (await Viewmodel.Worker.DoWork(new DeleteDeckDTO(Viewmodel.Repository).Execute(Viewmodel.DTO)))
      {
        Viewmodel.SetDeck(new());

        new SendNotification(Viewmodel.Notifier).Execute(DeckEditorNotifications.DeleteSuccess);
      }
      else new SendNotification(Viewmodel.Notifier).Execute(DeckEditorNotifications.DeleteError);
    }
  }
}
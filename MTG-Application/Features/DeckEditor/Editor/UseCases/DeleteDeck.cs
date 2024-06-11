using MTGApplication.Features.DeckEditor.Services.DeckEditor;
using MTGApplication.General.Models.Card;
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
    protected override bool CanExecute() => !string.IsNullOrEmpty(Viewmodel.Deck.Name);

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      if (await Viewmodel.Confirmers.DeleteDeckConfirmer.Confirm(DeckEditorConfirmers.GetDeleteDeckConfirmation(Viewmodel.Deck.Name))
        is not ConfirmationResult.Yes)
        return; // Cancel

      if (await Viewmodel.Worker.DoWork(new DeleteDeckDTO(Viewmodel.Repository).Execute(DeckEditorMTGDeckToDTOConverter.Convert(Viewmodel.Deck))))
      {
        Viewmodel.Deck = new();

        new SendNotification(Viewmodel.Notifier).Execute(DeckEditorNotifications.DeleteSuccess);
      }
      else new SendNotification(Viewmodel.Notifier).Execute(DeckEditorNotifications.DeleteError);
    }
  }
}
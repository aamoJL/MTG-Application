using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.Editor.Services.Converters;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public partial class DeckEditorViewModelCommands
{
  public class DeleteDeck(DeckEditorViewModel viewmodel) : AsyncCommand
  {
    public DeckEditorViewModel Viewmodel { get; } = viewmodel;

    protected override bool CanExecute() => !string.IsNullOrEmpty(Viewmodel.Name);

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      if (await Viewmodel.Confirmers.DeleteDeckConfirmer.Confirm(DeckEditorConfirmers.GetDeleteDeckConfirmation(Viewmodel.Name))
        is not ConfirmationResult.Yes)
        return; // Cancel

      if (await Viewmodel.Worker.DoWork(new DeleteDeckDTO(Viewmodel.Repository).Execute(DeckEditorMTGDeckToDTOConverter.Convert(Viewmodel.Deck))))
      {
        Viewmodel.Deck = new();
        Viewmodel.UndoStack.Clear();
        Viewmodel.HasUnsavedChanges = false;

        new ShowNotification(Viewmodel.Notifier).Execute(DeckEditorNotifications.DeleteSuccess);
      }
      else new ShowNotification(Viewmodel.Notifier).Execute(DeckEditorNotifications.DeleteError);
    }
  }
}
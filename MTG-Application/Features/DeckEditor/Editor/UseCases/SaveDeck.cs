using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.Editor.Services.Converters;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class DeckEditorViewModelCommands
{
  public class SaveDeck(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override async Task Execute()
    {
      var oldName = Viewmodel.Name;
      var overrideOld = false;
      var saveName = await Viewmodel.Confirmers.SaveDeckConfirmer.Confirm(DeckEditorConfirmers.GetSaveDeckConfirmation(oldName));

      if (string.IsNullOrEmpty(saveName))
        return;

      // Override confirmation
      if (saveName != oldName && await new DeckDTOExists(Viewmodel.Repository).Execute(saveName))
      {
        switch (await Viewmodel.Confirmers.OverrideDeckConfirmer.Confirm(DeckEditorConfirmers.GetOverrideDeckConfirmation(saveName)))
        {
          case ConfirmationResult.Yes: overrideOld = true; break;
          case ConfirmationResult.Cancel:
          default: return; // Cancel
        }
      }

      if (await Viewmodel.Worker.DoWork(SaveDTO(Viewmodel.DTO, saveName, overrideOld)) is true)
      {
        Viewmodel.Name = saveName;
        Viewmodel.HasUnsavedChanges = false;

        new SendNotification(Viewmodel.Notifier).Execute(DeckEditorNotifications.SaveSuccess);
      }
      else
        new SendNotification(Viewmodel.Notifier).Execute(DeckEditorNotifications.SaveError);
    }

    private async Task<bool> SaveDTO(MTGCardDeckDTO dto, string saveName, bool overrideOld = false)
    {
      var oldName = dto.Name;

      if (oldName != saveName && await new DeckDTOExists(Viewmodel.Repository).Execute(saveName) && !overrideOld)
        return false; // Cancel because overriding is not enabled

      if (await new AddOrUpdateDeckDTO(Viewmodel.Repository).Execute((dto, saveName))
        is bool wasSaved && wasSaved is true)
      {
        if (oldName != saveName && await new DeckDTOExists(Viewmodel.Repository).Execute(oldName) && !string.IsNullOrEmpty(oldName))
          await new DeleteDeckDTO(Viewmodel.Repository).Execute(oldName);
      }
      return wasSaved;
    }
  }
}
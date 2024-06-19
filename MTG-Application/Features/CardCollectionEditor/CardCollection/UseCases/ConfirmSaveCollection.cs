using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollection.ViewModels;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.CardCollection.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ConfirmSaveCollection(CardCollectionViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionViewModel>(viewmodel)
  {
    protected override async Task Execute()
    {
      var oldName = Viewmodel.Name;
      var overrideOld = false;
      var saveName = await Viewmodel.Confirmers.SaveCollectionConfirmer.Confirm(
        CardCollectionConfirmers.GetSaveCollectionConfirmation(oldName));

      if (string.IsNullOrEmpty(saveName))
        return;

      // Override confirmation
      if (saveName != oldName && await new CardCollectionDTOExists(Viewmodel.Repository).Execute(saveName))
      {
        switch (await Viewmodel.Confirmers.OverrideCollectionConfirmer.Confirm(CardCollectionConfirmers.GetOverrideCollectionConfirmation(saveName)))
        {
          case ConfirmationResult.Yes: overrideOld = true; break;
          case ConfirmationResult.No:
          default: return; // Cancel
        }
      }

      if (await Viewmodel.Worker.DoWork(SaveCollectionDTO(Viewmodel.AsDTO(), saveName, overrideOld)))
      {
        Viewmodel.Name = saveName;
        Viewmodel.HasUnsavedChanges = false;

        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.SaveCollectionSuccess);
      }
      else
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.SaveCollectionError);
    }

    private async Task<bool> SaveCollectionDTO(MTGCardCollectionDTO dto, string saveName, bool overrideOld = false)
    {
      var oldName = dto.Name;

      if (oldName != saveName && await new CardCollectionDTOExists(Viewmodel.Repository).Execute(saveName) && !overrideOld)
        return false; // Cancel because overriding is not enabled

      if (await new AddOrUpdateCardCollectionDTO(Viewmodel.Repository).Execute((dto, saveName)) is bool wasSaved && wasSaved is true)
      {
        if (!string.IsNullOrEmpty(oldName) && oldName != saveName && await new CardCollectionDTOExists(Viewmodel.Repository).Execute(oldName))
          await new DeleteCardCollectionDTO(Viewmodel.Repository).Execute(oldName);
      }

      return wasSaved;
    }
  }
}
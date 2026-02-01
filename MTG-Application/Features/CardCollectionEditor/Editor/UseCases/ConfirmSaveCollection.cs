using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services.Converters;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ConfirmSaveCollection(CardCollectionEditorViewModel viewmodel) : AsyncCommand
  {
    protected override async Task Execute()
    {
      var oldName = viewmodel.CollectionName;
      var overrideOld = false;
      var saveName = await viewmodel.Confirmers.CardCollectionConfirmers.SaveCollectionConfirmer.Confirm(
        CardCollectionConfirmers.GetSaveCollectionConfirmation(oldName));

      if (string.IsNullOrEmpty(saveName))
        return;

      // Override confirmation
      if (saveName != oldName && await new CardCollectionDTOExists(viewmodel.Repository).Execute(saveName))
      {
        if ((await viewmodel.Confirmers.CardCollectionConfirmers.OverrideCollectionConfirmer
          .Confirm(CardCollectionConfirmers.GetOverrideCollectionConfirmation(saveName))
          is ConfirmationResult.Yes))
        {
          overrideOld = true;
        }
        else
          return; // Cancel
      }

      try
      {
        var dto = CardCollectionToDTOConverter.Convert(viewmodel.Collection);

        await viewmodel.Worker.DoWork(Save(dto, saveName, overrideOld));

        new SendNotification(viewmodel.Notifier).Execute(CardCollectionNotifications.SaveCollectionSuccess);
      }
      catch
      {
        new SendNotification(viewmodel.Notifier).Execute(CardCollectionNotifications.SaveCollectionError);
      }
    }

    private async Task Save(MTGCardCollectionDTO dto, string saveName, bool overrideOld = false)
    {
      if (await SaveCollectionDTO(dto, saveName, overrideOld))
      {
        viewmodel.CollectionName = saveName;
        viewmodel.HasUnsavedChanges = false;
      }
      else throw new();
    }

    private async Task<bool> SaveCollectionDTO(MTGCardCollectionDTO dto, string saveName, bool overrideOld = false)
    {
      var oldName = dto.Name;

      if (oldName != saveName && await new CardCollectionDTOExists(viewmodel.Repository).Execute(saveName) && !overrideOld)
        return false; // Cancel because overriding is not enabled

      if (await new AddOrUpdateCardCollectionDTO(viewmodel.Repository).Execute((dto, saveName)) is bool wasSaved && wasSaved is true)
      {
        if (!string.IsNullOrEmpty(oldName) && oldName != saveName && await new CardCollectionDTOExists(viewmodel.Repository).Execute(oldName))
          await new DeleteCardCollectionDTO(viewmodel.Repository).Execute(oldName);
      }

      return wasSaved;
    }
  }
}
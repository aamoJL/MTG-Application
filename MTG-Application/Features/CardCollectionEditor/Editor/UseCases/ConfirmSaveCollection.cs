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
    public CardCollectionEditorViewModel Viewmodel { get; } = viewmodel;

    protected override async Task Execute()
    {
      var oldName = Viewmodel.CollectionName;
      var overrideOld = false;
      var saveName = await Viewmodel.Confirmers.CardCollectionConfirmers.SaveCollectionConfirmer.Confirm(
        CardCollectionConfirmers.GetSaveCollectionConfirmation(oldName));

      if (string.IsNullOrEmpty(saveName))
        return;

      // Override confirmation
      if (saveName != oldName && await new CardCollectionDTOExists(Viewmodel.Repository).Execute(saveName))
      {
        if ((await Viewmodel.Confirmers.CardCollectionConfirmers.OverrideCollectionConfirmer
          .Confirm(CardCollectionConfirmers.GetOverrideCollectionConfirmation(saveName))
          is ConfirmationResult.Yes))
        {
          overrideOld = true;
        }
        else
          return; // Cancel
      }

      var dto = CardCollectionToDTOConverter.Convert(Viewmodel.Collection);

      if (await (Viewmodel as IWorker).DoWork(SaveCollectionDTO(dto, saveName, overrideOld)))
      {
        Viewmodel.CollectionName = saveName;
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
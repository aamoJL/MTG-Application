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
  public class ConfirmDeleteCollection(CardCollectionEditorViewModel viewmodel) : AsyncCommand
  {
    protected override bool CanExecute() => !string.IsNullOrEmpty(viewmodel.CollectionName);

    protected override async Task Execute()
    {
      if (!CanExecute())
        return;

      if ((await viewmodel.Confirmers.CardCollectionConfirmers.DeleteCollectionConfirmer.Confirm(
        CardCollectionConfirmers.GetDeleteCollectionConfirmation(viewmodel.CollectionName)))
        is not ConfirmationResult.Yes)
        return; // Cancel

      try
      {
        var dto = CardCollectionToDTOConverter.Convert(viewmodel.Collection);

        await viewmodel.Worker.DoWork(Delete(dto));

        new SendNotification(viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteCollectionSuccess);
      }
      catch
      {
        new SendNotification(viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteCollectionError);
      }
    }

    private async Task Delete(MTGCardCollectionDTO dto)
    {
      if (await new DeleteCardCollectionDTO(viewmodel.Repository).Execute(dto))
        await viewmodel.ChangeCollection(new());
      else
        throw new();
    }
  }
}
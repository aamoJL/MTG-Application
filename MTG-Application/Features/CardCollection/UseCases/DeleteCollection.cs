using MTGApplication.Features.CardCollection.Services;
using MTGApplication.Features.CardCollection.Services.Converters;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;

public partial class CardCollectionViewModelCommands
{
  public class DeleteCollection(CardCollectionViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionViewModel>(viewmodel)
  {
    protected override bool CanExecute() => !string.IsNullOrEmpty(Viewmodel.Collection.Name);

    protected override async Task Execute()
    {
      if (!CanExecute())
        return;

      var deleteConfirmationResult = await Viewmodel.Confirmers.DeleteCollectionConfirmer.Confirm(
        CardCollectionConfirmers.GetDeleteCollectionConfirmation(Viewmodel.Collection.Name));

      switch (deleteConfirmationResult)
      {
        case ConfirmationResult.Yes: break;
        default: return; // Cancel
      }

      switch (await Viewmodel.Worker.DoWork(new DeleteCardCollectionDTO(Viewmodel.Repository)
        .Execute(CardCollectionToDTOConverter.Convert(Viewmodel.Collection))))
      {
        case true:
          await Viewmodel.SetCollection(new());
          new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.DeleteCollectionSuccess);
          break;
        case false:
          new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.DeletecollectionError);
          break;
      }
    }
  }
}
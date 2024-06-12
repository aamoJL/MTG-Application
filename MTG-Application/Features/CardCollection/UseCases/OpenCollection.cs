using MTGApplication.Features.CardCollection.Services;
using MTGApplication.Features.CardCollection.Services.Converters;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;

public partial class CardCollectionViewModelCommands
{
  public class OpenCollection(CardCollectionViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionViewModel>(viewmodel)
  {
    protected override async Task Execute()
    {
      var unsavedArgs = new ISavable.ConfirmArgs();

      await Viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(unsavedArgs);

      if (unsavedArgs.Cancelled) return;
      if (await Viewmodel.Confirmers.LoadCollectionConfirmer.Confirm(
        CardCollectionConfirmers.GetLoadCollectionConfirmation((await Viewmodel.Repository.Get(setIncludes: (set) => { })).Select(x => x.Name).OrderBy(x => x)))
        is not string loadName)
        return;

      if (await Viewmodel.Worker.DoWork(LoadCollection(loadName)) is MTGCardCollection loadedCollection)
      {
        await Viewmodel.SetCollection(loadedCollection);
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.OpenCollectionSuccess);
      }
      else new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.OpenCollectionError);
    }

    private async Task<MTGCardCollection> LoadCollection(string loadName)
    {
      var dto = await new GetCardCollectionDTO(Viewmodel.Repository).Execute(loadName);

      if (dto == null)
        return null;

      return await new DTOToCardCollectionConverter(Viewmodel.Importer).Convert(dto);
    }
  }
}
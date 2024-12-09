using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Models;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.Features.CardCollectionEditor.Editor.Services.Converters;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ConfirmOpenCollection(CardCollectionEditorViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionEditorViewModel>(viewmodel)
  {
    protected override async Task Execute()
    {
      var unsavedArgs = new ISavable.ConfirmArgs();

      if (Viewmodel.ConfirmUnsavedChangesCommand != null)
        await Viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(unsavedArgs);

      if (unsavedArgs.Cancelled)
        return;

      if (await Viewmodel.Confirmers.LoadCollectionConfirmer.Confirm(
        CardCollectionEditorConfirmers.GetLoadCollectionConfirmation((await Viewmodel.Repository.Get(setIncludes: (set) => { })).Select(x => x.Name).OrderBy(x => x)))
        is not string loadName)
        return;

      try
      {
        if (await Viewmodel.Worker.DoWork(LoadCollection(loadName)) is MTGCardCollection loadedCollection)
        {
          await Viewmodel.ChangeCollection(loadedCollection);

          new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.OpenCollectionSuccess);
        }
        else
          throw new InvalidOperationException("Collection was not found");
      }
      catch
      {
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.OpenCollectionError);
      }
    }

    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="System.Net.Http.HttpRequestException"></exception>
    /// <exception cref="UriFormatException"></exception>
    private async Task<MTGCardCollection?> LoadCollection(string loadName)
    {
      try
      {
        if (await new GetCardCollectionDTO(Viewmodel.Repository).Execute(loadName) is MTGCardCollectionDTO dto)
          return await new DTOToCardCollectionConverter(Viewmodel.Importer).Convert(dto);
        else
          return null;
      }
      catch { throw; }
    }
  }
}
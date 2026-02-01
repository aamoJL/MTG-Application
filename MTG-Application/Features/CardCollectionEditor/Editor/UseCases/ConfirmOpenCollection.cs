using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
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
  public class ConfirmOpenCollection(CardCollectionEditorViewModel viewmodel) : AsyncCommand
  {
    protected override async Task Execute()
    {
      var unsavedArgs = new ISavable.ConfirmArgs();

      if (viewmodel.ConfirmUnsavedChangesCommand != null)
        await viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(unsavedArgs);

      if (unsavedArgs.Cancelled)
        return;

      if (await viewmodel.Confirmers.CardCollectionConfirmers.LoadCollectionConfirmer.Confirm(
        CardCollectionConfirmers.GetLoadCollectionConfirmation((await viewmodel.Repository.Get(setIncludes: (set) => { })).Select(x => x.Name).OrderBy(x => x)))
        is not string loadName)
        return;

      try
      {
        await viewmodel.Worker.DoWork(Open(loadName));
        new SendNotification(viewmodel.Notifier).Execute(CardCollectionNotifications.OpenCollectionSuccess);
      }
      catch
      {
        new SendNotification(viewmodel.Notifier).Execute(CardCollectionNotifications.OpenCollectionError);
      }
    }

    private async Task Open(string loadName)
    {
      if (await LoadCollection(loadName) is CardCollectionEditorCardCollection loadedCollection)
        await viewmodel.ChangeCollection(loadedCollection);
      else
        throw new();
    }

    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="System.Net.Http.HttpRequestException"></exception>
    /// <exception cref="UriFormatException"></exception>
    private async Task<CardCollectionEditorCardCollection?> LoadCollection(string loadName)
    {
      if (await new GetCardCollectionDTO(viewmodel.Repository).Execute(loadName) is MTGCardCollectionDTO dto)
        return await new DTOToCardCollectionConverter(viewmodel.Importer).Convert(dto);
      else
        return null;
    }
  }
}
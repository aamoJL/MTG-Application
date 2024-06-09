using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.Services.DeckEditor;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.UseCases;

public partial class ConfirmUnsavedChanges(
  Confirmer<ConfirmationResult> saveUnsavedChangesConfirmer,
  IAsyncRelayCommand saveCommand)
{
  public async Task<bool> Execute(bool hasUnsavedChanges, string currentName)
  {
    if (!hasUnsavedChanges || !saveCommand?.CanExecute(null) is true) return true;

    switch (await saveUnsavedChangesConfirmer
      .Confirm(DeckEditorConfirmers.GetSaveUnsavedChangesConfirmation(currentName)))
    {
      case ConfirmationResult.Yes: await saveCommand.ExecuteAsync(null); return !hasUnsavedChanges;
      case ConfirmationResult.No: return true;
      default: return false;
    };
  }
}

public class NewDeck(Func<Task<bool>> unsavedConfirmer, Action<DeckEditorMTGDeck> onDeckChanged)
{
  public async Task Execute()
  {
    if (await unsavedConfirmer?.Invoke())
      onDeckChanged?.Invoke(new());
  }
}

public class OpenDeck(
  IRepository<MTGCardDeckDTO> repository,
  MTGCardImporter importer,
  Notifier notifier,
  Func<Task<bool>> unsavedConfirmer,
  Confirmer<string, string[]> loadConfirmer,
  IWorker worker,
  Action<DeckEditorMTGDeck> onDeckChanged)
{
  public static bool CanExecute(string name) => name != string.Empty;

  public async Task Execute(string loadName)
  {
    if (!CanExecute(loadName)) return;

    if (!await unsavedConfirmer?.Invoke())
      return;

    loadName ??= await loadConfirmer
      .Confirm(DeckEditorConfirmers.GetLoadDeckConfirmation(
        (await worker.DoWork(repository.Get((set) => { }))).Select(x => x.Name).ToArray()));

    if (string.IsNullOrEmpty(loadName))
      return;

    if (await worker.DoWork(new DTOToDeckEditorDeckConverter(importer).Convert(
      dto: await new GetDeckDTO(repository).Execute(loadName)))
      is DeckEditorMTGDeck deck)
    {
      onDeckChanged?.Invoke(deck);

      new SendNotification(notifier).Execute(DeckEditorNotifications.LoadSuccessNotification);
    }
    else
      new SendNotification(notifier).Execute(DeckEditorNotifications.LoadErrorNotification);
  }
}

public class SaveDeck(
  IRepository<MTGCardDeckDTO> repository,
  Notifier notifier,
  Confirmer<string, string> saveConfirmer,
  Confirmer<ConfirmationResult> overrideConfirmer,
  IWorker worker,
  Action<string> onNameChanged)
{
  public static bool CanExecute(DeckEditorMTGDeck deck) => deck.DeckCards.Count > 0;

  public async Task Execute(DeckEditorMTGDeck deck)
  {
    if (!CanExecute(deck)) return;

    var oldName = deck.Name;
    var overrideOld = false;
    var saveName = await saveConfirmer.Confirm(DeckEditorConfirmers.GetSaveDeckConfirmation(oldName));

    if (string.IsNullOrEmpty(saveName))
      return;

    // Override confirmation
    if (saveName != oldName && await new DeckDTOExists(repository).Execute(saveName))
    {
      switch (await overrideConfirmer.Confirm(DeckEditorConfirmers.GetOverrideDeckConfirmation(saveName)))
      {
        case ConfirmationResult.Yes: overrideOld = true; break;
        case ConfirmationResult.Cancel:
        default: return; // Cancel
      }
    }

    if (await worker.DoWork(Save(DeckEditorMTGDeckToDTOConverter.Convert(deck), saveName, overrideOld)) is true)
    {
      onNameChanged?.Invoke(saveName);
      new SendNotification(notifier).Execute(DeckEditorNotifications.SaveSuccessNotification);
    }
    else
      new SendNotification(notifier).Execute(DeckEditorNotifications.SaveErrorNotification);
  }

  private async Task<bool> Save(MTGCardDeckDTO dto, string saveName, bool overrideOld = false)
  {
    var oldName = dto.Name;

    if (oldName != saveName && await new DeckDTOExists(repository).Execute(saveName) && !overrideOld)
      return false; // Cancel because overriding is not enabled

    if (await new AddOrUpdateDeckDTO(repository).Execute((dto, saveName))
      is bool wasSaved && wasSaved is true)
    {
      if (oldName != saveName && await new DeckDTOExists(repository).Execute(oldName) && !string.IsNullOrEmpty(oldName))
        await new DeleteDeckDTO(repository).Execute(oldName);
    }

    return wasSaved;
  }
}

public class DeleteDeck(
  IRepository<MTGCardDeckDTO> repository,
  Notifier notifier,
  Confirmer<ConfirmationResult> deleteConfirmer,
  IWorker worker,
  Action<DeckEditorMTGDeck> onDeckChanged)
{
  public static bool CanExecute(DeckEditorMTGDeck deck) => !string.IsNullOrEmpty(deck.Name);

  public async Task Execute(DeckEditorMTGDeck deck)
  {
    if (!CanExecute(deck)) return;

    if (await deleteConfirmer.Confirm(DeckEditorConfirmers.GetDeleteDeckConfirmation(deck.Name))
      is not ConfirmationResult.Yes)
      return; // Cancel

    if (await worker.DoWork(new DeleteDeckDTO(repository).Execute(DeckEditorMTGDeckToDTOConverter.Convert(deck))))
    {
      onDeckChanged?.Invoke(new());

      new SendNotification(notifier).Execute(DeckEditorNotifications.DeleteSuccessNotification);
    }
    else new SendNotification(notifier).Execute(DeckEditorNotifications.DeleteErrorNotification);
  }
}

public class ImportCards()
{
  // TODO: import cards use case
}

public class ExportCards()
{
  // TODO: export cards use case
}
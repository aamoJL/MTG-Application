﻿using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.Editor.Services.Converters;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class DeckEditorViewModelCommands
{
  public class OpenDeck(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel, string>(viewmodel)
  {
    protected override bool CanExecute(string name) => name != string.Empty;

    protected override async Task Execute(string loadName)
    {
      if (!CanExecute(loadName)) return;

      var unsavedArgs = new ISavable.ConfirmArgs();

      await new ConfirmUnsavedChanges(Viewmodel).Command.ExecuteAsync(unsavedArgs);

      if (unsavedArgs.Cancelled)
        return;

      loadName ??= await Viewmodel.Confirmers.LoadDeckConfirmer
        .Confirm(DeckEditorConfirmers.GetLoadDeckConfirmation(
          (await Viewmodel.Worker.DoWork(Viewmodel.Repository.Get((set) => { }))).Select(x => x.Name).ToArray()));

      if (string.IsNullOrEmpty(loadName))
        return;

      if (await Viewmodel.Worker.DoWork(new DTOToDeckEditorDeckConverter(Viewmodel.Importer).Convert(
        dto: await new GetDeckDTO(Viewmodel.Repository).Execute(loadName)))
        is DeckEditorMTGDeck deck)
      {
        Viewmodel.SetDeck(deck);

        new SendNotification(Viewmodel.Notifier).Execute(DeckEditorNotifications.LoadSuccess);
      }
      else
        new SendNotification(Viewmodel.Notifier).Execute(DeckEditorNotifications.LoadError);
    }
  }
}
﻿using MTGApplication.Features.DeckEditor.Services.DeckEditor;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class DeckEditorViewModel
{
  public class SaveDeck(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.Deck.DeckCards.Count > 0;

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      var oldName = Viewmodel.Deck.Name;
      var overrideOld = false;
      var saveName = await Viewmodel.Confirmers.SaveDeckConfirmer.Confirm(DeckEditorConfirmers.GetSaveDeckConfirmation(oldName));

      if (string.IsNullOrEmpty(saveName))
        return;

      // Override confirmation
      if (saveName != oldName && await new DeckDTOExists(Viewmodel.Repository).Execute(saveName))
      {
        switch (await Viewmodel.Confirmers.OverrideDeckConfirmer.Confirm(DeckEditorConfirmers.GetOverrideDeckConfirmation(saveName)))
        {
          case ConfirmationResult.Yes: overrideOld = true; break;
          case ConfirmationResult.Cancel:
          default: return; // Cancel
        }
      }

      if (await Viewmodel.Worker.DoWork(SaveDTO(DeckEditorMTGDeckToDTOConverter.Convert(Viewmodel.Deck), saveName, overrideOld)) is true)
      {
        Viewmodel.Deck.Name = saveName;
        Viewmodel.OnPropertyChanged(nameof(DeckName));
        Viewmodel.HasUnsavedChanges = false;

        new SendNotification(Viewmodel.Notifier).Execute(DeckEditorNotifications.SaveSuccessNotification);
      }
      else
        new SendNotification(Viewmodel.Notifier).Execute(DeckEditorNotifications.SaveErrorNotification);
    }

    private async Task<bool> SaveDTO(MTGCardDeckDTO dto, string saveName, bool overrideOld = false)
    {
      var oldName = dto.Name;

      if (oldName != saveName && await new DeckDTOExists(Viewmodel.Repository).Execute(saveName) && !overrideOld)
        return false; // Cancel because overriding is not enabled

      if (await new AddOrUpdateDeckDTO(Viewmodel.Repository).Execute((dto, saveName))
        is bool wasSaved && wasSaved is true)
      {
        if (oldName != saveName && await new DeckDTOExists(Viewmodel.Repository).Execute(oldName) && !string.IsNullOrEmpty(oldName))
          await new DeleteDeckDTO(Viewmodel.Repository).Execute(oldName);
      }
      return wasSaved;
    }
  }
}
using MTGApplication.Features.CardCollection.Services;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;

public partial class CardCollectionViewModelCommands
{
  public class ImportCards(CardCollectionViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.SelectedList != null;

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      if (await Viewmodel.Confirmers.ImportCardsConfirmer.Confirm(
        CardCollectionConfirmers.GetImportCardsConfirmation())
        is not string importText || string.IsNullOrEmpty(importText))
        return;

      var result = await Viewmodel.Worker.DoWork(new DeckEditorCardImporter(Viewmodel.Importer).Import(importText));
      var addedCards = result.Found.Where(found => !Viewmodel.SelectedList.Cards.Any(old => old.Info.ScryfallId == found.Info.ScryfallId))
        .DistinctBy(x => x.Info.ScryfallId).ToList();

      foreach (var card in addedCards)
        Viewmodel.SelectedList.Cards.Add(new(card.Info));

      Viewmodel.HasUnsavedChanges = true;

      if (result.Found.Length == 0)
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.ImportCardsError);
      else
        new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.ImportCardsSuccessOrWarning(
          added: addedCards.Count,
          skipped: result.Found.Length - addedCards.Count,
          notFound: result.NotFoundCount));
    }
  }
}
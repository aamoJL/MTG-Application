using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ImportCards(CardCollectionListViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionListViewModel>(viewmodel)
  {
    protected override bool CanExecute() => !string.IsNullOrEmpty(Viewmodel.Name);

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      if (await Viewmodel.Confirmers.ImportCardsConfirmer.Confirm(
        CardCollectionListConfirmers.GetImportCardsConfirmation())
        is not string importText || string.IsNullOrEmpty(importText))
        return;

      var result = await Viewmodel.Worker.DoWork(new DeckEditorCardImporter(Viewmodel.Importer).Import(importText));
      var addedCards = result.Found.Where(found => !Viewmodel.OwnedCards.Any(old => old.Info.ScryfallId == found.Info.ScryfallId))
        .DistinctBy(x => x.Info.ScryfallId).ToList();

      foreach (var card in addedCards)
        Viewmodel.OwnedCards.Add(new(card.Info));

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
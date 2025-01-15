using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
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

      try
      {
        // Fetch imported cards and add the cards that are included in the query but not in the owned cards
        var importResult = await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportWithString(importText));
        var queryResult = await Viewmodel.Worker.DoWork(Viewmodel.Importer.ImportCardsWithSearchQuery(Viewmodel.Query, pagination: false));

        var addedCards = importResult.Found.Select(f => new CardCollectionMTGCard(f.Info))
          .IntersectBy(queryResult.Found.Select(c => c.Info.ScryfallId), f => f.Info.ScryfallId)
          .ExceptBy(Viewmodel.OwnedCards.Select(o => o.Info.ScryfallId), f => f.Info.ScryfallId)
          .DistinctBy(x => x.Info.ScryfallId)
          .ToList();

        foreach (var card in addedCards)
          Viewmodel.OwnedCards.Add(new(card.Info));

        Viewmodel.HasUnsavedChanges = true;

        if (importResult.Found.Length == 0)
          new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.ImportCardsError);
        else
          new SendNotification(Viewmodel.Notifier).Execute(CardCollectionNotifications.ImportCardsSuccessOrWarning(
            added: addedCards.Count,
            skipped: importResult.Found.Length - addedCards.Count,
            notFound: importResult.NotFoundCount));
      }
      catch (System.Exception e)
      {
        Viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, $"Error: {e.Message}"));
      }
    }
  }
}
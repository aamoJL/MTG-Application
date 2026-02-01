using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ImportCards(CardCollectionListViewModel viewmodel) : AsyncCommand
  {
    protected record ImportResult(CardImportResult Result, int AddedCount);

    protected override bool CanExecute() => !string.IsNullOrEmpty(viewmodel.Name);

    protected override async Task Execute()
    {
      if (!CanExecute()) return;

      if (await viewmodel.Confirmers.ImportCardsConfirmer.Confirm(
        CardCollectionListConfirmers.GetImportCardsConfirmation())
        is not string importText || string.IsNullOrEmpty(importText))
        return;

      try
      {
        var importResult = await viewmodel.Worker.DoWork(Import(importText));

        if (importResult.Result.Found.Length == 0)
          new SendNotification(viewmodel.Notifier).Execute(CardCollectionNotifications.ImportCardsError);
        else
          new SendNotification(viewmodel.Notifier).Execute(CardCollectionNotifications.ImportCardsSuccessOrWarning(
            added: importResult.AddedCount,
            skipped: importResult.Result.Found.Length - importResult.AddedCount,
            notFound: importResult.Result.NotFoundCount));
      }
      catch (System.Exception e)
      {
        viewmodel.Notifier.Notify(new(General.Services.NotificationService.NotificationService.NotificationType.Error, $"Error: {e.Message}"));
      }
    }

    private async Task<ImportResult> Import(string importText)
    {
      // Fetch imported cards and add the cards that are included in the query but not in the owned cards
      var importTask = Task.Run(() => viewmodel.Importer.ImportWithString(importText));
      var queryTask = Task.Run(() => viewmodel.Importer.ImportCardsWithSearchQuery(viewmodel.Query, pagination: false));

      await Task.WhenAll(importTask, queryTask);

      if (importTask.IsFaulted) throw importTask.Exception;
      if (queryTask.IsFaulted) throw queryTask.Exception;

      var addedCards = importTask.Result.Found.Select(f => new CardCollectionMTGCard(f.Info))
        .IntersectBy(queryTask.Result.Found.Select(c => c.Info.ScryfallId), f => f.Info.ScryfallId)
        .ExceptBy(viewmodel.Cards.Select(o => o.Info.ScryfallId), f => f.Info.ScryfallId)
        .DistinctBy(x => x.Info.ScryfallId)
        .ToList();

      foreach (var card in addedCards)
        viewmodel.Cards.Add(new(card.Info));

      return new(importTask.Result, addedCards.Count);
    }
  }
}
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor;

public partial class CardListViewModelCommands
{
  public class ImportCards(CardListViewModel viewmodel) : ViewModelAsyncCommand<CardListViewModel, string>(viewmodel)
  {
    protected override async Task Execute(string data)
    {
      data ??= await Viewmodel.Confirmers.ImportConfirmer.Confirm(CardListConfirmers.GetImportConfirmation(string.Empty));

      var result = await Viewmodel.Worker.DoWork(new DeckEditorCardImporter(Viewmodel.Importer).Import(data));

      var addedCards = new List<CardImportResult<MTGCardInfo>.Card>();
      var skipConflictConfirmation = false;
      var addConflictConfirmationResult = ConfirmationResult.Yes;

      // Confirm imported cards, if already exists
      foreach (var card in result.Found)
      {
        if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is not null)
        {
          // Card exist in the list; confirm the import, unless the user skips confirmations
          if (!skipConflictConfirmation)
            (addConflictConfirmationResult, skipConflictConfirmation) = await Viewmodel.Confirmers.AddMultipleConflictConfirmer.Confirm(
              CardListConfirmers.GetAddMultipleConflictConfirmation(card.Info.Name));

          if (addConflictConfirmationResult == ConfirmationResult.Yes)
            addedCards.Add(card);
        }
        else if (addedCards.FindIndex(x => x.Info.Name == card.Info.Name) is int index && index >= 0)
          addedCards[index] = addedCards[index] with { Count = addedCards[index].Count + card.Count };
        else
          addedCards.Add(card);
      }

      // Add cards
      if (addedCards.Count != 0)
      {
        Viewmodel.UndoStack.PushAndExecute(new ReversibleCollectionCommand<DeckEditorMTGCard>(addedCards.Select(x => new DeckEditorMTGCard(x.Info, x.Count)), Viewmodel.CardCopier)
        {
          ReversibleAction = new CardListViewModelReversibleActions.ReversibleAddCardAction(Viewmodel),
        });
      }

      var skippedCount = result.Found.Length - addedCards.Count;

      // Notifications
      if (result.Source == CardImportResult.ImportSource.External)
      {
        if (result.Found.Length != 0 && result.NotFoundCount == 0) // All found
          new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Success,
            $"{addedCards.Count} cards imported successfully." + (skippedCount > 0 ? $" ({skippedCount} cards skipped) " : "")));
        else if (result.Found.Length != 0 && result.NotFoundCount > 0) // Some found
          new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Warning,
            $"{addedCards.Count} / {result.NotFoundCount + addedCards.Count} cards imported successfully.{Environment.NewLine}{result.NotFoundCount} cards were not found." + (skippedCount > 0 ? $" ({skippedCount} cards skipped) " : "")));
        else if (result.NotFoundCount > 0) // None found
          new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Error, $"Error. No cards were imported."));
      }
    }
  }
}
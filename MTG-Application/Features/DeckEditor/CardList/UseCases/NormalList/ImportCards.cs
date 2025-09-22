using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions.CardListViewModelReversibleActions;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands
{
  public class ImportCards(IList<DeckEditorMTGCard> cards, ReversibleCommandStack undoStack, CardListConfirmers confirmers, IWorker worker, IMTGCardImporter importer, Notifier notifier) : AsyncCommand<string>
  {
    private IList<DeckEditorMTGCard> Cards { get; } = cards;
    private ReversibleCommandStack UndoStack { get; } = undoStack;
    private CardListConfirmers Confirmers { get; } = confirmers;
    private IWorker Worker { get; } = worker;
    private IMTGCardImporter Importer { get; } = importer;
    private Notifier Notifier { get; } = notifier;

    protected override async Task Execute(string? data)
    {
      data ??= await Confirmers.ImportConfirmer.Confirm(CardListConfirmers.GetImportConfirmation(string.Empty));

      try
      {
        ArgumentNullException.ThrowIfNull(data);

        if (data == string.Empty)
          return;

        var result = await Worker.DoWork(new DeckEditorCardImporter(Importer).Import(data));

        var newCards = new List<DeckEditorMTGCard>();
        var existingCards = new List<(DeckEditorMTGCard Card, int NewCount)>();
        var skipConflictConfirmation = false;
        var addConflictConfirmationResult = ConfirmationResult.Yes;

        // Confirm imported cards, if already exists
        foreach (var card in result.Found)
        {
          if (Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
          {
            // Card exist in the list; confirm the import, unless the user skips confirmations
            if (!skipConflictConfirmation)
            {
              (addConflictConfirmationResult, skipConflictConfirmation) = await Confirmers.AddMultipleConflictConfirmer
                .Confirm(CardListConfirmers.GetAddMultipleConflictConfirmation(card.Info.Name));
            }

            if (addConflictConfirmationResult == ConfirmationResult.Yes)
            {
              // Change existing card
              if (existingCards.FirstOrDefault(x => x.Card == existingCard) is (DeckEditorMTGCard, int) listItem)
                listItem.NewCount += card.Count;
              else
                existingCards.Add((existingCard, existingCard.Count + card.Count));
            }
          }
          else if (newCards.FindIndex(x => x.Info.Name == card.Info.Name) is int index && index >= 0)
            newCards[index].Count += card.Count; // Already added new card; change count
          else
            newCards.Add(new(card.Info, card.Count)); // add new card
        }

        var combinedCommand = new CombinedReversibleCommand();

        // Add new cards
        if (newCards.Count != 0)
        {
          combinedCommand.Commands.Add(
            new ReversibleCollectionCommand<DeckEditorMTGCard>(newCards)
            {
              ReversibleAction = new ReversibleAddCardsAction(Cards),
            });
        }

        // Change existing cards
        if (existingCards.Count != 0)
        {
          combinedCommand.Commands.AddRange(
            existingCards.Select(x => new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(x.Card, x.Card.Count, x.NewCount)
            {
              ReversibleAction = new ReversibleCardCountChangeAction()
            }));
        }

        // Execute
        if (combinedCommand.Commands.Count != 0)
          UndoStack.PushAndExecute(combinedCommand);

        var importCount = newCards.Count + existingCards.Count;
        var skippedCount = result.Found.Length - importCount;

        // Notifications
        if (result.Source == CardImportResult.ImportSource.External)
        {
          if (result.Found.Length != 0 && result.NotFoundCount == 0) // All found
            new SendNotification(Notifier).Execute(new(NotificationType.Success,
              $"{importCount} cards imported successfully." + (skippedCount > 0 ? $" ({skippedCount} cards skipped) " : "")));
          else if (result.Found.Length != 0 && result.NotFoundCount > 0) // Some found
            new SendNotification(Notifier).Execute(new(NotificationType.Warning,
              $"{importCount} / {result.NotFoundCount + result.Found.Length} cards imported successfully.{Environment.NewLine}{result.NotFoundCount} cards were not found." + (skippedCount > 0 ? $" ({skippedCount} cards skipped) " : "")));
          else if (result.NotFoundCount > 0) // None found
            new SendNotification(Notifier).Execute(new(NotificationType.Error, $"Error. No cards were imported."));
        }
      }
      catch (Exception e)
      {
        Notifier.Notify(new(NotificationType.Error, $"Error: {e.Message}"));
      }
    }
  }
}
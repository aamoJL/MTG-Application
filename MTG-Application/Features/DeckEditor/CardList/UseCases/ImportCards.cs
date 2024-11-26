using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.ViewModels;
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
  public IAsyncRelayCommand<string> ImportCardsCommand { get; } = new ImportCards(viewmodel).Command;

  private class ImportCards(CardListViewModel viewmodel) : ViewModelAsyncCommand<CardListViewModel, string>(viewmodel)
  {
    protected override async Task Execute(string data)
    {
      data ??= await Viewmodel.Confirmers.ImportConfirmer.Confirm(CardListConfirmers.GetImportConfirmation(string.Empty));

      try
      {
        var result = await Viewmodel.Worker.DoWork(new DeckEditorCardImporter(Viewmodel.Importer).Import(data));

        var newCards = new List<DeckEditorMTGCard>();
        var existingCards = new List<(DeckEditorMTGCard Card, int NewCount)>();
        var skipConflictConfirmation = false;
        var addConflictConfirmationResult = ConfirmationResult.Yes;

        // Confirm imported cards, if already exists
        foreach (var card in result.Found)
        {
          if (Viewmodel.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
          {
            // Card exist in the list; confirm the import, unless the user skips confirmations
            if (!skipConflictConfirmation)
            {
              (addConflictConfirmationResult, skipConflictConfirmation) = await Viewmodel.Confirmers.AddMultipleConflictConfirmer
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
                ReversibleAction = new ReversibleAddCardAction(Viewmodel),
              });
        }

        // Change existing cards
        if (existingCards.Count != 0)
        {
          combinedCommand.Commands.AddRange(
              existingCards.Select(x => new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(x.Card, x.Card.Count, x.NewCount)
              {
                ReversibleAction = new ReversibleCardCountChangeAction(Viewmodel)
              }));
        }

        // Execute
        if (combinedCommand.Commands.Count != 0)
          Viewmodel.UndoStack.PushAndExecute(combinedCommand);

        var importCount = newCards.Count + existingCards.Count;
        var skippedCount = result.Found.Length - importCount;

        // Notifications
        if (result.Source == CardImportResult.ImportSource.External)
        {
          if (result.Found.Length != 0 && result.NotFoundCount == 0) // All found
            new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Success,
              $"{importCount} cards imported successfully." + (skippedCount > 0 ? $" ({skippedCount} cards skipped) " : "")));
          else if (result.Found.Length != 0 && result.NotFoundCount > 0) // Some found
            new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Warning,
              $"{importCount} / {result.NotFoundCount + result.Found.Length} cards imported successfully.{Environment.NewLine}{result.NotFoundCount} cards were not found." + (skippedCount > 0 ? $" ({skippedCount} cards skipped) " : "")));
          else if (result.NotFoundCount > 0) // None found
            new SendNotification(Viewmodel.Notifier).Execute(new(NotificationType.Error, $"Error. No cards were imported."));
        }
      }
      catch (Exception e)
      {
        Viewmodel.Notifier.Notify(new(NotificationType.Error, $"Error: {e.Message}"));
      }
    }
  }
}
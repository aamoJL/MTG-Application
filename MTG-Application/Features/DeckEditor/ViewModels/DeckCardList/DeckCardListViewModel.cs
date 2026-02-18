using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Exporters;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.ScryfallAPI;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardList;

public partial class DeckCardListViewModel
{
  public DeckCardListViewModel(ObservableCollection<DeckEditorMTGCard> list)
  {
    Model = list;

    Model.CollectionChanged += Model_CollectionChanged;
  }

  public Worker Worker { get; init; } = new();
  public IMTGCardImporter Importer { protected get; init; } = App.MTGCardImporter;
  public IEdhrecImporter EdhrecImporter { protected get; init; } = new EdhrecImporter();
  public IScryfallImporter ScryfallImporter { protected get; init; } = new ScryfallAPI();
  public IExporter<string> Exporter { protected get; init; } = new ClipboardExporter();
  public ReversibleCommandStack UndoStack { protected get; init; } = new();
  public Notifier Notifier { protected get; init; } = new();
  public INetworkService NetworkService { protected get; init; } = new NetworkService();
  public CardListConfirmers Confirmers { protected get; init; } = new();

  public ObservableCollection<DeckCardViewModel> CardViewModels => field ??= new(Model.Select(CardViewModelFactory.Build));

  protected ObservableCollection<DeckEditorMTGCard> Model { get; }
  protected DeckCardViewModel.Factory CardViewModelFactory => field ??= new()
  {
    Worker = Worker,
    Importer = Importer,
    UndoStack = UndoStack,
    Notifier = Notifier,
    NetworkService = NetworkService,
    Confirmers = Confirmers.CardConfirmers,
    OnCardDelete = OnCardDelete,
  };

  [RelayCommand]
  protected async Task AddCard(DeckEditorMTGCard? card)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(card);

      if (Model.FirstOrDefault(x => x.Info.Name == card.Info.Name) != null)
      {
        // Conflict confirmation
        switch (await Confirmers.ConfirmAddSingleConflict(Confirmations.GetAddSingleConflictConfirmation(card.Info.Name)))
        {
          case ConfirmationResult.Yes: break;
          default: return; // Cancel
        }
      }

      UndoStack.PushAndExecute(
        new ReversibleCollectionCommand<DeckEditorMTGCard>(card)
        {
          ReversibleAction = new ReversibleAddCardsAction(Model)
        });
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  protected void RemoveCard(DeckEditorMTGCard? card)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(card);

      UndoStack.PushAndExecute(
        new ReversibleCollectionCommand<DeckEditorMTGCard>(card)
        {
          ReversibleAction = new ReversibleRemoveCardsAction(Model)
        });
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  protected void BeginMoveFrom(DeckEditorMTGCard? card)
  {
    ArgumentNullException.ThrowIfNull(card);

    UndoStack.ActiveCombinedCommand.Commands.Add(
      new ReversibleCollectionCommand<DeckEditorMTGCard>(card)
      {
        ReversibleAction = new ReversibleRemoveCardsAction(Model)
      });
  }

  [RelayCommand]
  protected async Task BeginMoveTo(DeckEditorMTGCard? card)
  {
    ArgumentNullException.ThrowIfNull(card);

    if (Model.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
    {
      // Conflict confirmation
      switch (await Confirmers.ConfirmAddSingleConflict(Confirmations.GetAddSingleConflictConfirmation(card.Info.Name)))
      {
        case ConfirmationResult.Yes:
          UndoStack.ActiveCombinedCommand.Commands.Add(
            new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(existingCard, existingCard.Count, card.Count + existingCard.Count)
            {
              ReversibleAction = new ReversibleCardCountChangeAction()
            });
          break;
        default: UndoStack.ActiveCombinedCommand.Cancel(); return; // Cancel
      }
    }
    else
      UndoStack.ActiveCombinedCommand.Commands.Add(
        new ReversibleCollectionCommand<DeckEditorMTGCard>(card)
        {
          ReversibleAction = new ReversibleAddCardsAction(Model)
        });
  }

  [RelayCommand]
  protected void ExecuteMove() => UndoStack.PushAndExecuteActiveCombinedCommand();

  [RelayCommand(CanExecute = nameof(CanClear))]
  protected void Clear()
  {
    if (!CanClear()) return;

    UndoStack.PushAndExecute(
      new ReversibleCollectionCommand<DeckEditorMTGCard>(Model)
      {
        ReversibleAction = new ReversibleRemoveCardsAction(Model)
      });
  }

  [RelayCommand]
  protected async Task ImportCards(string? data)
  {
    try
    {
      data ??= await Confirmers.ConfirmImport(Confirmations.GetImportConfirmation());

      if (string.IsNullOrEmpty(data))
        return; // Cancel

      var result = await Worker.DoWork(new ImportCards(Importer, EdhrecImporter, ScryfallImporter).Execute(data));

      var newCards = new List<DeckEditorMTGCard>();
      var existingCards = new List<(DeckEditorMTGCard Card, int NewCount)>();
      var skipConflictConfirmation = false;
      var addConflictConfirmationResult = ConfirmationResult.Yes;

      // Confirm imported cards, if already exists
      foreach (var card in result.Found)
      {
        if (Model.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
        {
          // Card exist in the list; confirm the import, unless the user skips confirmations
          if (!skipConflictConfirmation)
          {
            (addConflictConfirmationResult, skipConflictConfirmation)
              = await Confirmers.ConfirmAddMultipleConflict(Confirmations.GetAddMultipleConflictConfirmation(card.Info.Name));
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
          newCards.Add(new(card.Info) { Count = card.Count }); // add new card
      }

      var combinedCommand = new CombinedReversibleCommand();

      // Add new cards
      if (newCards.Count != 0)
      {
        combinedCommand.Commands.Add(
          new ReversibleCollectionCommand<DeckEditorMTGCard>(newCards)
          {
            ReversibleAction = new ReversibleAddCardsAction(Model),
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
          new ShowNotification(Notifier).Execute(new(NotificationType.Success,
            $"{importCount} cards imported successfully." + (skippedCount > 0 ? $" ({skippedCount} cards skipped) " : "")));
        else if (result.Found.Length != 0 && result.NotFoundCount > 0) // Some found
          new ShowNotification(Notifier).Execute(new(NotificationType.Warning,
            $"{importCount} / {result.NotFoundCount + result.Found.Length} cards imported successfully.{Environment.NewLine}{result.NotFoundCount} cards were not found." + (skippedCount > 0 ? $" ({skippedCount} cards skipped) " : "")));
        else if (result.NotFoundCount > 0) // None found
          new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error. No cards were imported."));
      }
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  protected async Task ExportCards(string? exportProperty)
  {
    try
    {
      var exportText = exportProperty switch
      {
        "Id" => string.Join(Environment.NewLine, Model.Select(x => x.Info.ScryfallId)),
        "Name" => string.Join(Environment.NewLine, Model.Select(x => x.Info.Name)),
        _ => throw new ArgumentException("Invalid export property"),
      };

      var response = await Confirmers.ConfirmExport(Confirmations.GetExportConfirmation(exportText));

      if (string.IsNullOrEmpty(response))
        return; // Cancel

      new ExportText(Exporter).Execute(response);

      new ShowNotification(Notifier).Execute(Exporter.SuccessNotification);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  protected bool CanClear() => Model.Any();

  protected void OnCardDelete(DeckEditorMTGCard card) => Model.Remove(card);

  protected virtual void Model_CollectionChanged(object? _, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    foreach (var item in e.AddedItems<DeckEditorMTGCard>())
      CardViewModels.Add(CardViewModelFactory.Build(item));
    foreach (var item in e.RemovedItems<DeckEditorMTGCard>())
    {
      if (CardViewModels.TryFindIndex(x => x.Info == item.Info, out var index))
        CardViewModels.RemoveAt(index);
    }
  }
}

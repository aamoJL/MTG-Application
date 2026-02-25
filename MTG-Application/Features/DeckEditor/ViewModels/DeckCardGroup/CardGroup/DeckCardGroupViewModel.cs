using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.Services;
using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.ScryfallAPI;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using MTGApplication.General.Views.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList.GroupedDeckCardListViewModel;
using static MTGApplication.Features.DeckEditor.ViewModels.DeckCardList.DeckCardListViewModel;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;

public partial class DeckCardGroupViewModel : ViewModelBase
{
  public DeckCardGroupViewModel(DeckEditorCardGroup group)
  {
    Model = group;

    Model.PropertyChanged += Model_PropertyChanged;

    (Model.Cards as INotifyCollectionChanged).CollectionChanged += ModelCards_CollectionChanged;

    foreach (var item in Model.Cards)
      item.PropertyChanged += ModelCard_PropertyChanged;
  }

  public Worker Worker { private get; init; } = new();
  public IMTGCardImporter Importer { protected get; init; } = App.MTGCardImporter;
  public IEdhrecImporter EdhrecImporter { protected get; init; } = new EdhrecImporter();
  public IScryfallImporter ScryfallImporter { protected get; init; } = new ScryfallAPI();
  public ReversibleCommandStack UndoStack { private get; init; } = new();
  public Notifier Notifier { private get; init; } = new();
  public CardFilters CardFilter { private get; init; } = new();
  public CardSorter CardSorter { private get; init; } = new();
  public GroupConfirmers Confirmers { private get; init; } = new();
  public CardListConfirmers ListConfirmers { private get; init; } = new();
  public required DeckCardViewModel.Factory CardViewModelFactory { private get; init; }
  public string GroupKey => Model.GroupKey;
  public int Size => Model.Cards.Sum(x => x.Count);
  public ObservableCollection<DeckCardViewModel> CardViewModels => field ??= [.. Model.Cards.Select(CardViewModelFactory.Build)];
  public FilterableAndSortableCollectionView<DeckCardViewModel> SortedAndFilteredView => field ??= new(CardViewModels, CardFilter, CardSorter);

  public Action<DeckEditorCardGroup> OnDelete { private get => field ?? throw new NotImplementedException(); init; }
  public Action<DeckEditorCardGroup, string> OnRename { private get => field ?? throw new NotImplementedException(); init; }

  private DeckEditorCardGroup Model { get; }

  [RelayCommand]
  private async Task AddCard(DeckEditorMTGCard? card)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(card);

      if (Model.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard existingCard)
      {
        // Confirm change on existing card
        switch (await ListConfirmers.ConfirmAddSingleConflict(Confirmations.GetAddSingleConflictConfirmation(card.Info.Name)))
        {
          case ConfirmationResult.Yes: break;
          default: return; // Cancel
        }

        UndoStack.PushAndExecute(new CombinedReversibleCommand()
        {
          Commands = [
            new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(existingCard, existingCard.Count, card.Count + existingCard.Count)
              {
                ReversibleAction = new ReversibleCardCountChangeAction()
              },
              new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(existingCard, existingCard.Group, GroupKey)
              {
                ReversibleAction = new ReversibleCardGroupChangeAction()
              }]
        });
      }
      else
      {
        // Add nonexisting card
        UndoStack.PushAndExecute(new ReversibleCollectionCommand<DeckEditorMTGCard>(TransformCardModel(card))
        {
          ReversibleAction = new ReversibleAddCardsToGroupSourceAction(Model)
        });
      }
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, e.Message));
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
          ReversibleAction = new ReversibleRemoveCardsFromGroupSourceAction(Model)
        });
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private async Task ImportCards(string? data)
  {
    try
    {
      data ??= await ListConfirmers.ConfirmImport(Confirmations.GetImportConfirmation());

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
        if (Model.GetFromSource(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard sourceCard)
        {
          // Card exist in the source; confirm the import, unless the user skips confirmations
          if (!skipConflictConfirmation)
          {
            (addConflictConfirmationResult, skipConflictConfirmation)
              = await ListConfirmers.ConfirmAddMultipleConflict(Confirmations.GetAddMultipleConflictConfirmation(card.Info.Name));
          }

          if (addConflictConfirmationResult == ConfirmationResult.Yes)
          {
            // Change existing card
            if (existingCards.FirstOrDefault(x => x.Card == sourceCard) is (DeckEditorMTGCard, int) listItem)
              listItem.NewCount += card.Count;
            else
              existingCards.Add((sourceCard, sourceCard.Count + card.Count));
          }
        }
        else if (newCards.FindIndex(x => x.Info.Name == card.Info.Name) is int index && index >= 0)
          newCards[index].Count += card.Count; // Already added new card; change count
        else
          newCards.Add(new(card.Info) { Count = card.Count, Group = GroupKey }); // add new card
      }

      var combinedCommand = new CombinedReversibleCommand();

      // Add new cards
      if (newCards.Count != 0)
      {
        combinedCommand.Commands.AddRange([
          new ReversibleCollectionCommand<DeckEditorMTGCard>(newCards.Select(TransformCardModel))
            {
              ReversibleAction = new ReversibleAddCardsToGroupSourceAction(Model),
            },
            .. newCards.Select(x => new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(x, x.Group, GroupKey)
            {
              ReversibleAction = new ReversibleCardGroupChangeAction()
            })
        ]);
      }

      // Change existing cards
      if (existingCards.Count != 0)
      {
        combinedCommand.Commands.AddRange(
          existingCards.Select(x => new CombinedReversibleCommand()
          {
            Commands = [
                  new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(x.Card, x.Card.Count, x.NewCount)
                  {
                    ReversibleAction = new ReversibleCardCountChangeAction()
                  },
                  new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(x.Card, x.Card.Group, GroupKey)
                  {
                    ReversibleAction = new ReversibleCardGroupChangeAction()
                  }]
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
  private void BeginMoveFrom(DeckEditorMTGCard? card)
  {
    ArgumentNullException.ThrowIfNull(card);

    UndoStack.ActiveCombinedCommand.Commands.Add(
      new ReversibleCollectionCommand<DeckEditorMTGCard>(card)
      {
        ReversibleAction = new ReversibleRemoveCardsFromGroupSourceAction(Model)
      });
  }

  [RelayCommand]
  private async Task BeginMoveTo(DeckEditorMTGCard? card)
  {
    ArgumentNullException.ThrowIfNull(card);

    var combinedCommands = UndoStack.ActiveCombinedCommand.Commands;

    if (Model.Cards.Contains(card))
    {
      // The exact card is already in the group
      UndoStack.ActiveCombinedCommand.Cancel();
    }
    else if (Model.SourceContains(card))
    {
      // The exact card is in the same source bu in a different group
      //  Operation does not need combined command so it can be cancelled
      UndoStack.ActiveCombinedCommand.Cancel();

      UndoStack.PushAndExecute(new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(card, card.Group, GroupKey)
      {
        ReversibleAction = new ReversibleCardGroupChangeAction()
      });
    }
    else if (Model.Cards.FirstOrDefault(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard groupCard)
    {
      // Similar card is in the group
      switch (await ListConfirmers.ConfirmAddSingleConflict(Confirmations.GetAddSingleConflictConfirmation(card.Info.Name)))
      {
        case ConfirmationResult.Yes:
          combinedCommands.Add(
            new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(groupCard, groupCard.Count, card.Count + groupCard.Count)
            {
              ReversibleAction = new ReversibleCardCountChangeAction()
            });
          break;
        default: UndoStack.ActiveCombinedCommand.Cancel(); break;
      }
    }
    else if (Model.GetFromSource(x => x.Info.Name == card.Info.Name) is DeckEditorMTGCard sourceCard)
    {
      // Similar card is in the same source, but not in the group
      switch (await ListConfirmers.ConfirmAddSingleConflict(Confirmations.GetAddSingleConflictConfirmation(card.Info.Name)))
      {
        case ConfirmationResult.Yes:
          combinedCommands.Add(new CombinedReversibleCommand()
          {
            Commands = [
                new ReversiblePropertyChangeCommand<DeckEditorMTGCard, int>(sourceCard, sourceCard.Count, card.Count + sourceCard.Count)
                    {
                      ReversibleAction = new ReversibleCardCountChangeAction()
                    },
                    new ReversiblePropertyChangeCommand<DeckEditorMTGCard, string>(sourceCard, sourceCard.Group, GroupKey)
                    {
                      ReversibleAction = new ReversibleCardGroupChangeAction()
                    }]
          });
          break;
        default: UndoStack.ActiveCombinedCommand.Cancel(); break;
      }
    }
    else
    {
      // New card
      combinedCommands.Add(new ReversibleCollectionCommand<DeckEditorMTGCard>(TransformCardModel(card))
      {
        ReversibleAction = new ReversibleAddCardsToGroupSourceAction(Model)
      });
    }
  }

  [RelayCommand]
  private void ExecuteMove() => UndoStack.PushAndExecuteActiveCombinedCommand();

  [RelayCommand(CanExecute = nameof(CanDeleteGroup))]
  private void DeleteGroup()
  {
    try
    {
      if (!CanDeleteGroup())
        throw new InvalidOperationException("Can't delete the default group");

      OnDelete.Invoke(Model);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, e.Message));
    }
  }

  [RelayCommand(CanExecute = nameof(CanRenameGroup))]
  private async Task RenameGroup()
  {
    try
    {
      if (!CanRenameGroup())
        throw new InvalidOperationException("Can't rename the default group");

      var newKey = await Confirmers.ConfirmRenameGroup(GroupConfirmations.GetRenameCardGroupConfirmation(GroupKey));

      if (string.IsNullOrEmpty(newKey)) return; // Cancel
      if (newKey == GroupKey) return; // Cancel, same key

      OnRename(Model, newKey);
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, e.Message));
    }
  }

  public bool SourceContains(DeckEditorMTGCard card) => Model.SourceContains(card);

  private bool CanDeleteGroup() => !string.IsNullOrEmpty(GroupKey);

  private bool CanRenameGroup() => !string.IsNullOrEmpty(GroupKey);

  private void Model_PropertyChanged(object? _, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(DeckEditorCardGroup.GroupKey): OnPropertyChanged(nameof(GroupKey)); break;
    }
  }

  private void ModelCards_CollectionChanged(object? _, NotifyCollectionChangedEventArgs e)
  {
    foreach (var item in e.AddedItems<DeckEditorMTGCard>())
    {
      item.PropertyChanged += ModelCard_PropertyChanged;

      if (!CardViewModels.Any(vm => vm.Info == item.Info))
        CardViewModels.Add(CardViewModelFactory.Build(item));
    }
    foreach (var item in e.RemovedItems<DeckEditorMTGCard>())
    {
      item.PropertyChanged -= ModelCard_PropertyChanged;

      if (CardViewModels.TryFindIndex(vm => vm.Info == item.Info, out var i))
        CardViewModels.RemoveAt(i);
    }

    OnPropertyChanged(nameof(Size));
  }

  private void ModelCard_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(DeckEditorMTGCard.Count): OnPropertyChanged(nameof(Size)); break;
    }
  }

  protected virtual DeckEditorMTGCard TransformCardModel(DeckEditorMTGCard card) => card.Copy();
}
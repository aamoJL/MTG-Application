using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.Services;
using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCardList;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCommanders;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.Deck;

public partial class DeckViewModel : ViewModelBase
{
  public DeckViewModel(DeckEditorMTGDeck deck)
  {
    Model = deck;
    UndoStack = new();

    Model.PropertyChanged += Model_PropertyChanged;
  }

  public required DeckEditorDependencies EditorDependencies { get; init; }
  public SaveStatus SaveStatus { get; init; } = new();
  public ReversibleCommandStack UndoStack
  {
    get;
    init
    {
      field?.CollectionChanged -= UndoStack_CollectionChanged;
      SetProperty(ref field, value);
      field.CollectionChanged += UndoStack_CollectionChanged;
    }
  }
  public CardFilters CardFilter { get; } = new();
  public CardSorter CardSorter { get; } = new();

  public string DeckName => Model.Name;
  public int DeckSize => Model.DeckSize;
  public double DeckPrice => Model.DeckPrice;
  public ReadOnlyObservableCollection<DeckEditorMTGCard> ReadOnlyDeckCards => field ??= new(Model.DeckCards);

  public DeckCommandersViewModel CommandersViewModel => field ??= CreateDeckCommandersViewModel(Model);
  public GroupedDeckCardListViewModel DeckCardListViewModel => field ??= CreateGroupedDeckCardListViewModel(Model.DeckCards);
  public SideCardListViewModel MaybelistViewModel => field ??= CreateSideCardListViewModel(Model.Maybelist);
  public SideCardListViewModel WishlistViewModel => field ??= CreateSideCardListViewModel(Model.Wishlist);
  public SideCardListViewModel RemovelistViewModel => field ??= CreateSideCardListViewModel(Model.Removelist);

  public required Func<Task> OnDeleted { get; set; }

  private DeckEditorMTGDeck Model { get; }

  [RelayCommand]
  private async Task SaveUnsavedChanges(SaveStatus.ConfirmArgs? args)
  {
    if (args == null || args.Cancelled || !SaveStatus.HasUnsavedChanges) return;

    try
    {
      switch (await EditorDependencies.DeckConfirmers.ConfirmUnsavedChanges(Confirmations.GetSaveUnsavedChangesConfirmation(DeckName)))
      {
        case ConfirmationResult.Yes:
          await SaveDeck();
          args.Cancelled = SaveStatus.HasUnsavedChanges;
          break;
        case ConfirmationResult.Cancel:
          args.Cancelled = true;
          break;
      }
    }
    catch (Exception e)
    {
      args.Cancelled = true;

      new ShowNotification(EditorDependencies.Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private async Task SaveDeck()
  {
    try
    {
      var oldName = DeckName;
      var overrideOld = false;

      var saveName = await EditorDependencies.DeckConfirmers.ConfirmDeckSave(Confirmations.GetSaveDeckConfirmation(oldName));

      if (saveName == null)
        return; // Cancel

      if (saveName == string.Empty)
        throw new InvalidOperationException("Invalid name");

      // Override confirmation
      if (saveName != oldName && await new DeckDTOExists(EditorDependencies.Repository).Execute(saveName))
      {
        switch (await EditorDependencies.DeckConfirmers.ConfirmDeckSaveOverride(Confirmations.GetOverrideDeckConfirmation(saveName)))
        {
          case ConfirmationResult.Yes: overrideOld = true; break;
          default: return; // Cancel
        }
      }

      if (await EditorDependencies.Worker.DoWork(new SaveDeck(EditorDependencies.Repository).Execute(Model, saveName, overrideOld)))
      {
        Model.Name = saveName;
        SaveStatus.HasUnsavedChanges = false;

        new ShowNotification(EditorDependencies.Notifier).Execute(Notifications.SaveSuccess);
      }
      else
        new ShowNotification(EditorDependencies.Notifier).Execute(Notifications.SaveError);
    }
    catch (Exception e)
    {
      new ShowNotification(EditorDependencies.Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand(CanExecute = nameof(CanDelete))]
  private async Task DeleteDeck()
  {
    try
    {
      if (!CanDelete())
        throw new ArgumentException("Name should not be empty", nameof(DeckName));

      switch (await EditorDependencies.DeckConfirmers.ConfirmDeckDelete(Confirmations.GetDeleteDeckConfirmation(DeckName)))
      {
        case ConfirmationResult.Yes: break;
        default: return; // Cancel
      }

      if (await EditorDependencies.Worker.DoWork(new DeleteDeck(EditorDependencies.Repository).Execute(Model)))
      {
        await OnDeleted();

        new ShowNotification(EditorDependencies.Notifier).Execute(Notifications.DeleteSuccess);
      }
      else
        new ShowNotification(EditorDependencies.Notifier).Execute(Notifications.DeleteError);
    }
    catch (Exception e)
    {
      new ShowNotification(EditorDependencies.Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand]
  private async Task ShowDeckTokens()
  {
    try
    {
      var cards = new List<MTGCard?>([.. Model.DeckCards, Model.Commander, Model.CommanderPartner]).OfType<MTGCard>();
      var tokens = (await EditorDependencies.Worker.DoWork(new FetchTokens(EditorDependencies.Importer).Execute(cards))).Found.Select(x => new MTGCard(x.Info));

      await EditorDependencies.DeckConfirmers.ConfirmDeckTokens(Confirmations.GetShowTokensConfirmation(tokens));
    }
    catch (Exception e)
    {
      new ShowNotification(EditorDependencies.Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand(CanExecute = nameof(CanOpenDeckTestingWindow))]
  private void OpenDeckTestingWindow()
  {
    try
    {
      if (!CanOpenDeckTestingWindow())
        throw new InvalidOperationException("Can't open testing window. Deck does not have any cards");

      var testingDeck = new DeckTestingDeck(
          DeckCards: [.. Model.DeckCards.SelectMany(c => Enumerable.Range(1, c.Count).Select(_ => c as MTGCard))],
          Commander: Model.Commander,
          Partner: Model.CommanderPartner);

      new AppWindows.DeckTestingWindow.DeckTestingWindow(testingDeck).Activate();
    }
    catch (Exception e)
    {
      new ShowNotification(EditorDependencies.Notifier).Execute(new(NotificationType.Error, $"Error: {e.Message}"));
    }
  }

  [RelayCommand(CanExecute = nameof(CanOpenEdhrecSearchWindow))]
  private async Task OpenEdhrecSearchWindow()
  {
    try
    {
      if (!CanOpenEdhrecSearchWindow() || Model.Commander == null)
        throw new InvalidOperationException("Can't open EDHREC search. The deck does not have a commander");

      var themeCountLimit = 5;
      var themes = (await new EdhrecImporter().GetThemes(
        commander: Model.Commander.Info.Name,
        partner: Model.CommanderPartner?.Info.Name)).Take(themeCountLimit).ToArray();

      new AppWindows.EdhrecSearchWindow.EdhrecSearchWindow(themes).Activate();
    }
    catch (Exception e)
    {
      new ShowNotification(EditorDependencies.Notifier).Execute((new(NotificationType.Error, $"Error: {e.Message}")));
    }
  }

  private bool CanDelete() => !string.IsNullOrEmpty(DeckName);

  private bool CanOpenDeckTestingWindow() => Model.DeckCards.Count != 0;

  private bool CanOpenEdhrecSearchWindow() => Model.Commander != null;

  private void Model_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(DeckEditorMTGDeck.Name): OnPropertyChanged(nameof(DeckName)); break;
      case nameof(DeckEditorMTGDeck.DeckSize): OnPropertyChanged(nameof(DeckSize)); break;
      case nameof(DeckEditorMTGDeck.DeckPrice): OnPropertyChanged(nameof(DeckPrice)); break;
    }
  }

  private void UndoStack_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    => SaveStatus.HasUnsavedChanges = true;

  private SideCardListViewModel CreateSideCardListViewModel(ObservableCollection<DeckEditorMTGCard> source) => new(source)
  {
    EditorDependencies = EditorDependencies,
    UndoStack = UndoStack,
    CardFilter = CardFilter,
    CardSorter = CardSorter,
  };

  private GroupedDeckCardListViewModel CreateGroupedDeckCardListViewModel(ObservableCollection<DeckEditorMTGCard> source) => new(source)
  {
    EditorDependencies = EditorDependencies,
    UndoStack = UndoStack,
    CardFilter = CardFilter,
    CardSorter = CardSorter,
  };

  private DeckCommandersViewModel CreateDeckCommandersViewModel(DeckEditorMTGDeck deck) => new(deck)
  {
    EditorDependencies = EditorDependencies,
    UndoStack = UndoStack,
  };
}
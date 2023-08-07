using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Services;
using MTGApplication.ViewModels.Charts;
using MTGApplication.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MTGApplication.Enums;
using static MTGApplication.Services.CommandService;
using static MTGApplication.Services.DialogService;
using static MTGApplication.Services.MTGService;

namespace MTGApplication.ViewModels;

/// <summary>
/// Deck Builder tab view model
/// </summary>
public partial class DeckBuilderViewModel : ViewModelBase, ISavable
{
  /// <summary>
  /// Deck Builder tab dialogs
  /// </summary>
  public class DeckBuilderViewDialogs
  {
    public virtual ConfirmationDialog GetCardAlreadyInCardlistDialog(string cardName, string listName)
      => new("Card already in the deck") { Message = $"Card '{cardName}' is already in the {listName}. Do you still want to add it?", SecondaryButtonText = string.Empty, CloseButtonText = "No" };

    public virtual ConfirmationDialog GetOverrideDialog(string name)
      => new("Override existing deck?") { Message = $"Deck '{name}' already exist. Would you like to override the deck?", SecondaryButtonText = string.Empty };

    public virtual ConfirmationDialog GetDeleteDialog(string name)
      => new("Delete deck?") { Message = $"Are you sure you want to delete '{name}'?", SecondaryButtonText = string.Empty };

    public virtual ConfirmationDialog GetSaveUnsavedDialog()
      => new("Save unsaved changes?") { Message = "Deck has unsaved changes. Would you like to save the deck?", PrimaryButtonText = "Save" };

    public virtual CheckBoxDialog GetMultipleCardsAlreadyInDeckDialog(string name)
      => new("Card already in the deck") { Message = $"'{name}' is already in the deck. Do you still want to add it?", InputText = "Same for all cards.", SecondaryButtonText = string.Empty, CloseButtonText = "No" };

    public virtual GridViewDialog<MTGCardViewModel> GetCardPrintDialog(MTGCardViewModel[] printViewModels)
      => new("Change card print", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle") { Items = printViewModels, SecondaryButtonText = string.Empty };

    public virtual GridViewDialog<MTGCardViewModel> GetTokenPrintDialog(MTGCardViewModel[] printViewModels)
      => new("Tokens", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle") { Items = printViewModels, SecondaryButtonText = string.Empty, PrimaryButtonText = string.Empty };

    public virtual ComboBoxDialog GetLoadDialog(string[] names)
      => new("Open deck") { InputHeader = "Name", Items = names, PrimaryButtonText = "Open", SecondaryButtonText = string.Empty };

    public virtual TextAreaDialog GetExportDialog(string text)
      => new("Export deck") { TextInputText = text, PrimaryButtonText = "Copy to Clipboard", SecondaryButtonText = string.Empty };

    public virtual TextAreaDialog GetImportDialog()
      => new("Import cards") { InputPlaceholderText = "Example:\n2 Black Lotus\nMox Ruby\nbd8fa327-dd41-4737-8f19-2cf5eb1f7cdd", SecondaryButtonText = string.Empty, PrimaryButtonText = "Add to Collection" };

    public virtual TextBoxDialog GetSaveDialog(string name)
      => new("Save your deck?") { InvalidInputCharacters = Path.GetInvalidFileNameChars(), TextInputText = name, PrimaryButtonText = "Save", SecondaryButtonText = string.Empty };
  }

  public partial class DeckCardlistViewModel : ObservableObject
  {
    public DeckCardlistViewModel(ObservableCollection<MTGCard> cardlist, DeckBuilderViewDialogs dialogs, ICardAPI<MTGCard> cardAPI, MTGCardFilters cardFilters = null, MTGCardSortProperties sortProperties = null)
    {
      Cardlist = cardlist;
      Dialogs = dialogs;
      CardAPI = cardAPI;
      CardFilters = cardFilters ?? new();
      SortProperties = sortProperties ?? new();

      FilteredAndSortedCardViewModels = new(CardViewModels, true);
      FilteredAndSortedCardViewModels.SortDescriptions.Add(new SortDescription(MTGCardViewModel.GetPropertyName(SortProperties.PrimarySortProperty), SortProperties.SortDirection));
      FilteredAndSortedCardViewModels.SortDescriptions.Add(new SortDescription(MTGCardViewModel.GetPropertyName(SortProperties.SecondarySortProperty), SortProperties.SortDirection));

      PropertyChanged += MTGCardlistViewModel_PropertyChanged;
      CardFilters.PropertyChanged += Filters_PropertyChanged;
      CardViewModels.CollectionChanged += CardViewModels_CollectionChanged;
      SortProperties.PropertyChanged += SortProperties_PropertyChanged;

      OnPropertyChanged(nameof(Cardlist));
    }

    private void SortProperties_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      FilteredAndSortedCardViewModels.SortDescriptions[0] = new SortDescription(MTGCardViewModel.GetPropertyName(SortProperties.PrimarySortProperty), SortProperties.SortDirection);
      FilteredAndSortedCardViewModels.SortDescriptions[1] = new SortDescription(MTGCardViewModel.GetPropertyName(SortProperties.SecondarySortProperty), SortProperties.SortDirection);
    }

    private void CardViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnPropertyChanged(nameof(CardlistSize));

    private void Filters_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (CardFilters.FiltersApplied)
      {
        FilteredAndSortedCardViewModels.Filter = x =>
        {
          return CardFilters.CardValidation((MTGCardViewModel)x);
        };
        FilteredAndSortedCardViewModels.RefreshFilter();
      }
      else { FilteredAndSortedCardViewModels.Filter = null; }
    }

    private void CardViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case nameof(MTGCardViewModel.Count): OnPropertyChanged(nameof(CardlistSize)); break;
        case nameof(MTGCardViewModel.Price): OnPropertyChanged(nameof(EuroPrice)); break;
      }
    }

    private void MTGCardlistViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case nameof(Cardlist):
          Cardlist.CollectionChanged += CardCollection_CollectionChanged;
          CardViewModels.Clear();
          foreach (var card in Cardlist.ToArray())
          {
            var vm = new MTGCardViewModel(card) { DeleteCardCommand = new RelayCommand<MTGCard>(Remove), ShowPrintsDialogCommand = new AsyncRelayCommand<MTGCard>(ChangePrintDialog) };
            vm.PropertyChanged += CardViewModel_PropertyChanged;
            CardViewModels.Add(vm);
          }
          break;
        case nameof(CardlistSize):
          OnPropertyChanged(nameof(EuroPrice));
          break;
        case nameof(EuroPrice):
          SavableChangesOccured?.Invoke();
          break;
      }
    }

    private void CardCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      // Sync models and viewmodels
      MTGCardViewModel vm;
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          // Add CardViewModels
          vm = new MTGCardViewModel(e.NewItems[0] as MTGCard) { DeleteCardCommand = new RelayCommand<MTGCard>(Remove), ShowPrintsDialogCommand = new AsyncRelayCommand<MTGCard>(ChangePrintDialog) };
          vm.PropertyChanged += CardViewModel_PropertyChanged;
          CardViewModels.Add(vm);
          break;
        case NotifyCollectionChangedAction.Remove:
          vm = CardViewModels.FirstOrDefault(x => x.Model == e.OldItems[0] as MTGCard);
          if (vm != null)
          {
            vm.PropertyChanged -= CardViewModel_PropertyChanged;
            CardViewModels.Remove(vm);
          }
          break;
        case NotifyCollectionChangedAction.Reset:
          CardViewModels.Clear();
          break;
      }
      SavableChangesOccured?.Invoke();
    }

    private ObservableCollection<MTGCardViewModel> CardViewModels { get; } = new();
    private DeckBuilderViewDialogs Dialogs { get; }
    private ICardAPI<MTGCard> CardAPI { get; }

    [ObservableProperty] private ObservableCollection<MTGCard> cardlist;
    [ObservableProperty] private bool isBusy;

    public IOService.ClipboardService ClipboardService { get; init; } = new();
    public AdvancedCollectionView FilteredAndSortedCardViewModels { get; }
    public MTGCardSortProperties SortProperties { get; }
    public CommandService CommandService { get; init; } = new();
    public MTGCardFilters CardFilters { get; }
    public Action SavableChangesOccured { get; set; }
    public string Name { get; init; }

    /// <summary>
    /// Returns total card count of the cardlist cards
    /// </summary>
    public int CardlistSize => CardViewModels.Sum(x => x.Model.Count);
    /// <summary>
    /// Returns total euro price of the cardlist cards
    /// </summary>
    public float EuroPrice => (float)Math.Round(CardViewModels.Sum(x => x.Model.Info.Price * x.Model.Count), 0);

    #region Relay Commands
    /// <summary>
    /// Asks cards to import and imports them to the deck
    /// </summary>
    [RelayCommand]
    public async Task ImportToCardlistDialog()
    {
      if (await Dialogs.GetImportDialog().ShowAsync() is string importText)
      {
        await Import(importText);
      }
    }

    /// <summary>
    /// Shows dialog with formatted text of the cardlist cards
    /// </summary>
    /// <param name="exportProperty">'Name' or 'Id'</param>
    [RelayCommand]
    public async Task ExportDeckCardsDialog(string exportProperty = "Name")
    {
      if (await Dialogs.GetExportDialog(MTGService.GetExportString(Cardlist.ToArray(), exportProperty)).ShowAsync() is var response && !string.IsNullOrEmpty(response))
      {
        ClipboardService.Copy(response);
      }
    }

    /// <summary>
    /// Removes all items in the card list
    /// </summary>
    [RelayCommand]
    public void Clear() => CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.RemoveCardsFromCardlistCommand(Cardlist, Cardlist.ToArray()));
    #endregion

    /// <summary>
    /// Shows a dialog with cards prints and changes the cards print to the selected print
    /// </summary>
    public async Task ChangePrintDialog(MTGCard card)
    {
      if (card == null) { return; }

      // Get prints
      IsBusy = true;
      var printVMs = new List<MTGCardViewModel>();
      var pageUri = card.Info.PrintSearchUri;
      while (!string.IsNullOrEmpty(pageUri))
      {
        var result = await CardAPI.FetchFromUri(pageUri, paperOnly: true);
        printVMs.AddRange(result.Found.Select(x => new MTGCardViewModel(x)));
        pageUri = result.NextPageUri;
      }
      IsBusy = false;

      if (await Dialogs.GetCardPrintDialog(printVMs.ToArray()).ShowAsync() is MTGCardViewModel newPrint)
      {
        // Replace card
        card.Info = newPrint.Model.Info;
      }
    }

    /// <summary>
    /// Imports cards from the card API using the <paramref name="importText"/>
    /// Shows a dialog that asks the user if they want to skip already existing cards.
    /// </summary>
    public async Task Import(string importText)
    {
      var notFoundCount = 0;
      var found = Array.Empty<MTGCard>();
      var importCards = new List<MTGCard>();

      if (!string.IsNullOrEmpty(importText))
      {
        IsBusy = true;
        bool? import = true;
        bool? skipDialog = false;
        var result = await CardAPI.FetchFromString(importText);
        found = result.Found;
        notFoundCount = result.NotFoundCount;
        foreach (var card in found)
        {
          if (Cardlist.FirstOrDefault(x => x.Info.Name == card.Info.Name) is not null)
          {
            // Card exist in the deck, ask if want to import, unless user has checked the dialog skip checkbox in the dialog
            if (skipDialog is not true)
            {
              (import, skipDialog) = await Dialogs.GetMultipleCardsAlreadyInDeckDialog(card.Info.Name).ShowAsync();
            }

            if (import is true)
            { importCards.Add(card); }
          }
          else
          { importCards.Add(card); }
        }
      }

      if (importCards.Count != 0)
      {
        CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.AddCardsToCardlistCommand(Cardlist, importCards.ToArray()));
      }

      if (notFoundCount == 0 && found.Length > 0)
        NotificationService.RaiseNotification(NotificationService.NotificationType.Success, $"{importCards.Count} cards imported successfully." + ((found.Length - importCards.Count) > 0 ? $" ({(found.Length - importCards.Count)} cards skipped) " : ""));
      else if (found.Length > 0 && notFoundCount > 0)
        NotificationService.RaiseNotification(NotificationService.NotificationType.Warning,
          $"{found.Length} / {notFoundCount + found.Length} cards imported successfully.{Environment.NewLine}{notFoundCount} cards were not found." + ((found.Length - importCards.Count) > 0 ? $" ({(found.Length - importCards.Count)} cards skipped) " : ""));
      else if (found.Length == 0)
        NotificationService.RaiseNotification(NotificationService.NotificationType.Error, $"Error. No cards were imported.");

      IsBusy = false;
    }

    /// <summary>
    /// Adds given card to the deck cardlist
    /// </summary>
    public async Task Add(MTGCard card)
    {
      if (Cardlist.FirstOrDefault(x => x.Info.Name == card.Info.Name) is not null)
      {
        if ((await Dialogs.GetCardAlreadyInCardlistDialog(card.Info.Name, Name).ShowAsync()) is true)
        {
          CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.AddCardsToCardlistCommand(cardlist, new[] { card }));
        }
      }
      else { CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.AddCardsToCardlistCommand(cardlist, new[] { card })); }
    }

    /// <summary>
    /// Moves the given card from the origin list to this list
    /// </summary>
    public async Task Move(MTGCard card, DeckCardlistViewModel origin)
    {
      if (origin == null)
      {
        await Add(card); return;
      }
      else if (Cardlist.FirstOrDefault(x => x.Info.Name == card.Info.Name) is not null)
      {
        if ((await Dialogs.GetCardAlreadyInCardlistDialog(card.Info.Name, Name).ShowAsync()) is true)
        {
          CommandService.Execute(new CombinedCommand(new ICommand[]
          {
            new MTGCardDeck.MTGCardDeckCommands.AddCardsToCardlistCommand(Cardlist, new[]{ new MTGCard(card.Info, card.Count) }),
            new MTGCardDeck.MTGCardDeckCommands.RemoveCardsFromCardlistCommand(origin.Cardlist, new[]{card})
          }));
        }
      }
      else
      {
        CommandService.Execute(new CombinedCommand(new ICommand[]
          {
            new MTGCardDeck.MTGCardDeckCommands.AddCardsToCardlistCommand(Cardlist, new[]{new MTGCard(card.Info, card.Count) }),
            new MTGCardDeck.MTGCardDeckCommands.RemoveCardsFromCardlistCommand(origin.Cardlist, new[]{card})
          }));
      }
    }

    /// <summary>
    /// Removes given card from the deck cardlist
    /// </summary>
    public void Remove(MTGCard card) => CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.RemoveCardsFromCardlistCommand(Cardlist, new[] { card }));
  }

  public DeckBuilderViewModel(ICardAPI<MTGCard> cardAPI, IRepository<MTGCardDeck> deckRepository, IOService.ClipboardService clipboardService = null, DeckBuilderViewDialogs dialogs = null)
  {
    DeckRepository = deckRepository;
    CardAPI = cardAPI;
    Dialogs = dialogs ?? new();
    clipboardService ??= new();

    DeckCards = new DeckCardlistViewModel(CardDeck.DeckCards, Dialogs, CardAPI, CardFilters, SortProperties)
    {
      ClipboardService = clipboardService,
      CommandService = CommandService,
      Name = "Deck",
    };
    WishlistCards = new DeckCardlistViewModel(CardDeck.Wishlist, Dialogs, CardAPI, CardFilters, SortProperties)
    {
      ClipboardService = clipboardService,
      CommandService = CommandService,
      Name = "Wishlist",
    };
    MaybelistCards = new DeckCardlistViewModel(CardDeck.Maybelist, Dialogs, CardAPI, CardFilters, SortProperties)
    {
      ClipboardService = clipboardService,
      CommandService = CommandService,
      Name = "Maybelist",
    };
    RemovelistCards = new DeckCardlistViewModel(CardDeck.Removelist, Dialogs, CardAPI, CardFilters, SortProperties)
    {
      ClipboardService = clipboardService,
      CommandService = CommandService,
      Name = "Removelist",
    };

    DeckCards.SavableChangesOccured += () => { HasUnsavedChanges = true; };
    WishlistCards.SavableChangesOccured += () => { HasUnsavedChanges = true; };
    MaybelistCards.SavableChangesOccured += () => { HasUnsavedChanges = true; };
    RemovelistCards.SavableChangesOccured += () => { HasUnsavedChanges = true; };

    PropertyChanged += DeckBuilderViewModel_PropertyChanged;
    CardDeck.PropertyChanged += CardDeck_PropertyChanged;
    DeckCards.PropertyChanged += DeckCards_PropertyChanged;
    DeckCards.PropertyChanged += DeckCardlistViewModel_PropertyChanged;
    WishlistCards.PropertyChanged += DeckCardlistViewModel_PropertyChanged;
    MaybelistCards.PropertyChanged += DeckCardlistViewModel_PropertyChanged;
    RemovelistCards.PropertyChanged += DeckCardlistViewModel_PropertyChanged;

    UpdateCharts();
  }

  private void DeckCards_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(DeckCardlistViewModel.CardlistSize))
    {
      OnPropertyChanged(nameof(DeckSize));
    }
  }

  private void CardDeck_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(CardDeck.Commander):
        Commander = CardDeck?.Commander != null ? new(CardDeck.Commander) { DeleteCardCommand = SetCommanderCommand, ShowPrintsDialogCommand = ChangePrintDialogCommand } : null;
        HasUnsavedChanges = true;
        OnPropertyChanged(nameof(DeckSize));
        break;
      case nameof(CardDeck.CommanderPartner):
        CommanderPartner = CardDeck?.CommanderPartner != null ? new(CardDeck.CommanderPartner) { DeleteCardCommand = SetCommanderPartnerCommand, ShowPrintsDialogCommand = ChangePrintDialogCommand } : null;
        HasUnsavedChanges = true;
        OnPropertyChanged(nameof(DeckSize));
        break;
    }
  }

  private void DeckCardlistViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(DeckCardlistViewModel.IsBusy):
        IsBusy = DeckCards.IsBusy || WishlistCards.IsBusy || MaybelistCards.IsBusy || RemovelistCards.IsBusy; break;
    }
  }

  private void DeckBuilderViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(CardDeck):
        CardDeck.PropertyChanged += CardDeck_PropertyChanged;
        DeckCards.Cardlist = CardDeck.DeckCards;
        WishlistCards.Cardlist = CardDeck.Wishlist;
        MaybelistCards.Cardlist = CardDeck.Maybelist;
        RemovelistCards.Cardlist = CardDeck.Removelist;
        Commander = CardDeck?.Commander != null ? new(CardDeck.Commander) : null;
        CommanderPartner = CardDeck?.CommanderPartner != null ? new(CardDeck.CommanderPartner) : null;
        CommandService.Clear();
        UpdateCharts();
        HasUnsavedChanges = false;
        break;
      case nameof(DeckSize):
        OpenPlaytestWindowCommand.NotifyCanExecuteChanged(); break;
    }
  }

  private IRepository<MTGCardDeck> DeckRepository { get; }
  private DeckBuilderViewDialogs Dialogs { get; }
  private ICardAPI<MTGCard> CardAPI { get; }

  [ObservableProperty] private MTGCardDeck cardDeck = new();
  [ObservableProperty] private MTGManaProductionPieChart manaProductionChart;
  [ObservableProperty] private MTGSpellTypePieChart spellTypeChart;
  [ObservableProperty] private MTGCMCStackedColumnChart cMCChart;
  [ObservableProperty] private MTGColorPieChart colorChart;
  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private MTGCardViewModel commander;
  [ObservableProperty] private MTGCardViewModel commanderPartner;

  public DeckCardlistViewModel DeckCards { get; }
  public DeckCardlistViewModel WishlistCards { get; }
  public DeckCardlistViewModel MaybelistCards { get; }
  public DeckCardlistViewModel RemovelistCards { get; }
  public MTGCardFilters CardFilters { get; } = new();
  public CommandService CommandService { get; } = new();
  public MTGCardSortProperties SortProperties { get; } = new() { SortDirection = SortDirection.Ascending, PrimarySortProperty = MTGSortProperty.CMC, SecondarySortProperty = MTGSortProperty.Name };
  public int DeckSize => DeckCards.CardlistSize + (CardDeck.Commander != null ? 1 : 0) + (CardDeck.CommanderPartner != null ? 1 : 0);

  #region ISavable implementation
  private bool hasUnsavedChanges = false;
  public bool HasUnsavedChanges
  {
    get => hasUnsavedChanges;
    set => SetProperty(ref hasUnsavedChanges, value);
  }

  /// <summary>
  /// Shows dialog that asks the user if they want to save unsaved changes
  /// </summary>
  public async Task<bool> SaveUnsavedChanges()
  {
    if (HasUnsavedChanges)
    {
      // Deck has unsaved changes
      var wantSaveConfirmed = await Dialogs.GetSaveUnsavedDialog().ShowAsync(force: true);
      if (wantSaveConfirmed == null) { return false; }
      else if (wantSaveConfirmed is true)
      {
        // User wants to save the unsaved changes
        var saveName = await Dialogs.GetSaveDialog(CardDeck.Name).ShowAsync();
        if (string.IsNullOrEmpty(saveName)) { return false; }
        else
        {
          if (saveName != CardDeck.Name && await DeckRepository.Exists(saveName))
          {
            // Deck exists already
            var overrideConfirmed = await Dialogs.GetOverrideDialog(saveName).ShowAsync();
            if (overrideConfirmed == null) { return false; }
            else if (overrideConfirmed is true)
            {
              // User wants to override the deck
              await SaveDeck(saveName);
            }
          }
          else
          { await SaveDeck(saveName); }
        }
      }
    }
    return true;
  }
  #endregion

  #region Relay Commands
  /// <summary>
  /// Shows UnsavedChanges dialog and clear current card deck
  /// </summary>
  [RelayCommand]
  public async Task NewDeckDialog()
  {
    if (await SaveUnsavedChanges())
    {
      IsBusy = true;
      CardDeck = await Task.Run(() => new MTGCardDeck());
      IsBusy = false;
    }
  }

  /// <summary>
  /// Shows dialog with names of the saved decks and changes current deck to the selected deck from the database
  /// </summary>
  [RelayCommand]
  public async Task LoadDeckDialog()
  {
    if (await SaveUnsavedChanges())
    {
      var loadName = await Dialogs.GetLoadDialog((await DeckRepository.Get()).Select(x => x.Name).OrderBy(x => x).ToArray()).ShowAsync();
      if (loadName != null)
      {
        IsBusy = true;
        if (await Task.Run(() => DeckRepository.Get(loadName)) is MTGCardDeck loadedDeck)
        {
          CardDeck = loadedDeck;
          NotificationService.RaiseNotification(NotificationService.NotificationType.Success, "The deck was loaded successfully.");
        }
        else { NotificationService.RaiseNotification(NotificationService.NotificationType.Error, "Error. Could not load the deck."); }
        IsBusy = false;
      }
    }
  }

  /// <summary>
  /// Shows dialog that asks name for the deck and saves current deck to the database with the given name
  /// </summary>
  [RelayCommand]
  public async Task SaveDeckDialog()
  {
    var saveName = await Dialogs.GetSaveDialog(CardDeck.Name).ShowAsync();
    if (string.IsNullOrEmpty(saveName)) { return; }
    else
    {
      if (saveName != CardDeck.Name && await DeckRepository.Exists(saveName))
      {
        // Deck with the given name exists already
        if (await Dialogs.GetOverrideDialog(saveName).ShowAsync() == null) { return; }
      }
    }

    await SaveDeck(saveName);
  }

  /// <summary>
  /// Deletes current deck from the database
  /// </summary>
  [RelayCommand(CanExecute = nameof(DeckIsLoaded))]
  public async Task DeleteDeckDialog()
  {
    if (!await DeckRepository.Exists(CardDeck.Name)) { return; }
    if (await Dialogs.GetDeleteDialog(CardDeck.Name).ShowAsync() is true)
    {
      IsBusy = true;
      if (await Task.Run(() => DeckRepository.Remove(CardDeck)))
      {
        CardDeck = new();
        NotificationService.RaiseNotification(NotificationService.NotificationType.Success, "The deck was deleted successfully.");
      }
      else { NotificationService.RaiseNotification(NotificationService.NotificationType.Error, "Error. Could not delete the deck."); }
      IsBusy = false;
    }
  }

  /// <summary>
  /// Sorts selected card deck by selected property using given direction
  /// </summary>
  /// <param name="dir">"Ascending" or "Descending"</param>
  [RelayCommand]
  public void SortByDirection(string dir)
  {
    if (Enum.TryParse(dir, true, out SortDirection sortDirection))
    {
      SortProperties.SortDirection = sortDirection;
    }
  }

  /// <summary>
  /// Sets <see cref="SelectedPrimarySortProperty"/> to the given property
  /// </summary>
  [RelayCommand]
  public void SetPrimarySortProperty(string prop)
  {
    if (Enum.TryParse(prop, true, out MTGSortProperty sortProperty))
    {
      SortProperties.PrimarySortProperty = sortProperty;
    }
  }

  /// <summary>
  /// Sets <see cref="SelectedSecondarySortProperty"/> to the given property
  /// </summary>
  [RelayCommand]
  public void SetSecondarySortProperty(string prop)
  {
    if (Enum.TryParse(prop, true, out MTGSortProperty sortProperty))
    {
      SortProperties.SecondarySortProperty = sortProperty;
    }
  }

  /// <summary>
  /// Shows deck cards' tokens in a dialog
  /// </summary>
  [RelayCommand]
  public async Task TokensDialog()
  {
    var stringBuilder = new StringBuilder();

    foreach (var card in CardDeck.DeckCards)
    {
      foreach (var token in card.Info.Tokens)
      {
        stringBuilder.AppendLine(token.ScryfallId.ToString());
      }
    }

    var tokens = (await CardAPI.FetchFromString(stringBuilder.ToString())).Found.Select(x => new MTGCardViewModel(x)).ToArray();
    await Dialogs.GetTokenPrintDialog(tokens).ShowAsync();
  }

  /// <summary>
  /// Opens Deck testing page on a new window for the given deck
  /// </summary>
  [RelayCommand(CanExecute = nameof(DeckHasCards))]
  public void OpenPlaytestWindow(MTGCardDeck deck)
  {
    var testingWindow = new DeckTestingWindow(deck);
    testingWindow.Activate();
  }

  [RelayCommand]
  /// <summary>
  /// Sets deck's commander to the given card
  /// </summary>
  public void SetCommander(MTGCard card) => CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.SetCommanderCommand(CardDeck, card));

  [RelayCommand]
  /// <summary>
  /// Sets deck's commander partner to the given card
  /// </summary>
  /// <param name="card"></param>
  public void SetCommanderPartner(MTGCard card) => CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.SetCommanderPartnerCommand(CardDeck, card));

  /// <summary>
  /// Shows a dialog with cards prints and changes the cards print to the selected print
  /// </summary>
  [RelayCommand]
  public async Task ChangePrintDialog(MTGCard card)
  {
    if (card == null) { return; }

    // Get prints
    IsBusy = true;
    var printVMs = new List<MTGCardViewModel>();
    var pageUri = card.Info.PrintSearchUri;
    while (!string.IsNullOrEmpty(pageUri))
    {
      var result = await CardAPI.FetchFromUri(pageUri, paperOnly: true);
      printVMs.AddRange(result.Found.Select(x => new MTGCardViewModel(x)));
      pageUri = result.NextPageUri;
    }
    IsBusy = false;

    if (await Dialogs.GetCardPrintDialog(printVMs.ToArray()).ShowAsync() is MTGCardViewModel newPrint)
    {
      // Replace card
      card.Info = newPrint.Model.Info;
    }
  }
  #endregion

  /// <summary>
  /// Sets deck's commander to the given card and removes the card from the origin
  /// </summary>
  public void MoveToCommander(MTGCard card, DeckCardlistViewModel origin)
  {
    if (origin == null)
    {
      SetCommander(card);
    }
    else
    {
      CommandService.Execute(new CombinedCommand(new ICommand[]
      {
        new MTGCardDeck.MTGCardDeckCommands.SetCommanderCommand(CardDeck, new(card.Info)),
        new MTGCardDeck.MTGCardDeckCommands.RemoveCardsFromCardlistCommand(origin.Cardlist, new[] { card }),
      }));
    }
  }

  /// <summary>
  /// Sets deck's commander partner to the given card and removes the card from the origin
  /// </summary>
  public void MoveToCommanderPartner(MTGCard card, DeckCardlistViewModel origin)
  {
    if (origin == null)
    {
      SetCommanderPartner(card);
    }
    else
    {
      CommandService.Execute(new CombinedCommand(new ICommand[]
      {
        new MTGCardDeck.MTGCardDeckCommands.SetCommanderPartnerCommand(CardDeck, new(card.Info)),
        new MTGCardDeck.MTGCardDeckCommands.RemoveCardsFromCardlistCommand(origin.Cardlist, new[] { card }),
      }));
    }
  }

  /// <summary>
  /// Imports commander card from the given text
  /// </summary>
  public async Task ImportCommander(string importText)
  {
    if (await CardAPI.FetchFromString(importText) is var result && result.Found.Length > 0)
    {
      SetCommander(result.Found[0]);
      NotificationService.RaiseNotification(NotificationService.NotificationType.Success, $"Commander was imported successfully.");
    }
    else
    {
      NotificationService.RaiseNotification(NotificationService.NotificationType.Error, $"Could not import the commander.");
    }
  }

  /// <summary>
  /// Imports commander partner card from the given text
  /// </summary>
  public async Task ImportCommanderPartner(string importText)
  {
    if (await CardAPI.FetchFromString(importText) is var result && result.Found.Length > 0)
    {
      SetCommanderPartner(result.Found[0]);
      NotificationService.RaiseNotification(NotificationService.NotificationType.Success, $"Partner was imported successfully.");
    }
    else
    {
      NotificationService.RaiseNotification(NotificationService.NotificationType.Error, $"Could not import the partner.");
    }
  }

  /// <summary>
  /// Saves current deck with the given <paramref name="name"/>
  /// </summary>
  private async Task SaveDeck(string name)
  {
    IsBusy = true;
    var tempDeck = CardDeck.GetCopy();
    tempDeck.Name = name;
    if (await Task.Run(() => DeckRepository.AddOrUpdate(tempDeck)))
    {
      // TODO: can the temp deck be moved to the AddOrUpdate method?
      // Maybe add Remove(string name) method?
      if (!string.IsNullOrEmpty(CardDeck?.Name) && name != CardDeck.Name)
      {
        await DeckRepository.Remove(CardDeck); // Delete old deck if the name was changed
      }
      CardDeck.Name = name;
      HasUnsavedChanges = false;
      NotificationService.RaiseNotification(NotificationService.NotificationType.Success, "The deck was saved successfully.");
    }
    else
    { NotificationService.RaiseNotification(NotificationService.NotificationType.Error, "Error. Could not save the deck."); }
    IsBusy = false;
  }

  /// <summary>
  /// Updates card charts
  /// </summary>
  private void UpdateCharts()
  {
    CMCChart = new MTGCMCStackedColumnChart() { Models = CardDeck.DeckCards };
    SpellTypeChart = new MTGSpellTypePieChart() { Models = CardDeck.DeckCards };
    ManaProductionChart = new MTGManaProductionPieChart() { Models = CardDeck.DeckCards };
    ColorChart = new MTGColorPieChart(innerRadius: 60) { Models = CardDeck.DeckCards };
  }

  #region RelayCommand CanExecute Methods
  /// <summary>
  /// Returns true if deck has been loaded from a database
  /// </summary>
  private bool DeckIsLoaded() => !string.IsNullOrEmpty(CardDeck.Name);

  /// <summary>
  /// Returns true if the deck has cards in it
  /// </summary>
  private bool DeckHasCards() => DeckCards.CardlistSize > 0;
  #endregion
}

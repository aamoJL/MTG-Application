using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using MTGApplication.API;
using MTGApplication.API.CardAPI;
using MTGApplication.Database.Repositories;
using MTGApplication.General;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Services;
using MTGApplication.ViewModels.Charts;
using MTGApplication.Views.Pages;
using MTGApplication.Views.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MTGApplication.Enums;
using static MTGApplication.Services.CommandService;
using static MTGApplication.Services.MTGService;

namespace MTGApplication.ViewModels;

/// <summary>
/// Deck Builder tab view model
/// </summary>
public partial class DeckBuilderViewModel : ViewModelBase, ISavable, IInAppNotifier, IDialogNotifier
{
  public DeckBuilderViewModel(ICardAPI<MTGCard> cardAPI, IRepository<MTGCardDeck> deckRepository, IOService.ClipboardService clipboardService = default, IMTGCommanderAPI commanderAPI = default)
  {
    DeckRepository = deckRepository;
    CardAPI = cardAPI;
    clipboardService ??= new();
    CommanderAPI = commanderAPI ?? new EDHRECCommanderAPI();

    DeckCards = new DeckCardlistViewModel(CardDeck.DeckCards, CardAPI, CardFilters, SortProperties)
    {
      ClipboardService = clipboardService,
      CommandService = CommandService,
      Name = "Deck",
    };
    WishlistCards = new DeckCardlistViewModel(CardDeck.Wishlist, CardAPI, CardFilters, SortProperties)
    {
      ClipboardService = clipboardService,
      CommandService = CommandService,
      Name = "Wishlist",
    };
    MaybelistCards = new DeckCardlistViewModel(CardDeck.Maybelist, CardAPI, CardFilters, SortProperties)
    {
      ClipboardService = clipboardService,
      CommandService = CommandService,
      Name = "Maybelist",
    };
    RemovelistCards = new DeckCardlistViewModel(CardDeck.Removelist, CardAPI, CardFilters, SortProperties)
    {
      ClipboardService = clipboardService,
      CommandService = CommandService,
      Name = "Removelist",
    };

    DeckCards.SavableChangesOccurred += () => { HasUnsavedChanges = true; };
    WishlistCards.SavableChangesOccurred += () => { HasUnsavedChanges = true; };
    MaybelistCards.SavableChangesOccurred += () => { HasUnsavedChanges = true; };
    RemovelistCards.SavableChangesOccurred += () => { HasUnsavedChanges = true; };

    DeckCards.OnNotification += (s, args) => OnNotification?.Invoke(s, args);
    WishlistCards.OnNotification += (s, args) => OnNotification?.Invoke(s, args);
    MaybelistCards.OnNotification += (s, args) => OnNotification?.Invoke(s, args);
    RemovelistCards.OnNotification += (s, args) => OnNotification?.Invoke(s, args);

    DeckCards.OnGetDialogWrapper += (s, args) => OnGetDialogWrapper?.Invoke(s, args);
    WishlistCards.OnGetDialogWrapper += (s, args) => OnGetDialogWrapper?.Invoke(s, args);
    MaybelistCards.OnGetDialogWrapper += (s, args) => OnGetDialogWrapper?.Invoke(s, args);
    RemovelistCards.OnGetDialogWrapper += (s, args) => OnGetDialogWrapper?.Invoke(s, args);

    PropertyChanged += DeckBuilderViewModel_PropertyChanged;
    CardDeck.PropertyChanged += CardDeck_PropertyChanged;
    DeckCards.PropertyChanged += DeckCards_PropertyChanged;
    DeckCards.PropertyChanged += DeckCardlistViewModel_PropertyChanged;
    WishlistCards.PropertyChanged += DeckCardlistViewModel_PropertyChanged;
    MaybelistCards.PropertyChanged += DeckCardlistViewModel_PropertyChanged;
    RemovelistCards.PropertyChanged += DeckCardlistViewModel_PropertyChanged;

    UpdateCharts();
  }

  private MTGCardDeck cardDeck = new();
  private MTGCardViewModel commander;
  private MTGCardViewModel commanderPartner;

  #region Properties
  [ObservableProperty] private MTGManaProductionPieChart manaProductionChart;
  [ObservableProperty] private MTGSpellTypePieChart spellTypeChart;
  [ObservableProperty] private MTGCMCStackedColumnChart cMCChart;
  [ObservableProperty] private MTGColorPieChart colorChart;
  [ObservableProperty] private MTGColorAndManaPolarChart colorDevotionChart;
  [ObservableProperty] private bool isBusy;

  private IRepository<MTGCardDeck> DeckRepository { get; }
  private ICardAPI<MTGCard> CardAPI { get; }
  private IMTGCommanderAPI CommanderAPI { get; }

  public MTGCardDeck CardDeck
  {
    get => cardDeck;
    set
    {
      if (cardDeck != null)
      {
        cardDeck.PropertyChanged -= CardDeck_PropertyChanged;
      }

      cardDeck = value;

      DeckCards.Cardlist = cardDeck.DeckCards;
      WishlistCards.Cardlist = cardDeck.Wishlist;
      MaybelistCards.Cardlist = cardDeck.Maybelist;
      RemovelistCards.Cardlist = cardDeck.Removelist;
      Commander = cardDeck?.Commander != null ? new(cardDeck.Commander) { DeleteCardCommand = SetCommanderCommand, ShowPrintsDialogCommand = ChangePrintDialogCommand } : null;
      CommanderPartner = cardDeck?.CommanderPartner != null ? new(cardDeck.CommanderPartner) { DeleteCardCommand = SetCommanderPartnerCommand, ShowPrintsDialogCommand = ChangePrintDialogCommand } : null;
      CommandService.Clear();
      UpdateCharts();
      HasUnsavedChanges = false;
      cardDeck.PropertyChanged += CardDeck_PropertyChanged;
      OnPropertyChanged(nameof(CardDeck));
      OnPropertyChanged(nameof(DeckName));
    }
  }
  public MTGCardViewModel Commander
  {
    get => commander;
    set
    {
      if (commander != null)
        commander.PropertyChanged -= Commanders_PropertyChanged;

      commander = value;

      if (commander != null)
        commander.PropertyChanged += Commanders_PropertyChanged;

      OnPropertyChanged(nameof(Commander));
      OnPropertyChanged(nameof(DeckSize));
      OnPropertyChanged(nameof(DeckPrice));
      HasUnsavedChanges = true;
    }
  }
  public MTGCardViewModel CommanderPartner
  {
    get => commanderPartner;
    set
    {
      if (commanderPartner != null)
        commanderPartner.PropertyChanged -= Commanders_PropertyChanged;

      commanderPartner = value;

      if (commanderPartner != null)
        commanderPartner.PropertyChanged += Commanders_PropertyChanged;

      OnPropertyChanged(nameof(CommanderPartner));
      OnPropertyChanged(nameof(DeckSize));
      OnPropertyChanged(nameof(DeckPrice));
      HasUnsavedChanges = true;
    }
  }
  public DeckBuilderViewDialogs Dialogs { get; set; } = new();
  public DeckCardlistViewModel DeckCards { get; }
  public DeckCardlistViewModel WishlistCards { get; }
  public DeckCardlistViewModel MaybelistCards { get; }
  public DeckCardlistViewModel RemovelistCards { get; }
  public MTGCardFilters CardFilters { get; } = new();
  public CommandService CommandService { get; } = new();
  public MTGCardSortProperties SortProperties { get; }
    = new() { SortDirection = SortDirection.Ascending, PrimarySortProperty = MTGSortProperty.CMC, SecondarySortProperty = MTGSortProperty.Name };
  public string DeckName => CardDeck.Name;
  public int DeckSize
    => DeckCards.CardlistSize + (CardDeck.Commander != null ? 1 : 0) + (CardDeck.CommanderPartner != null ? 1 : 0);
  public float DeckPrice
    => DeckCards.EuroPrice + (CardDeck.Commander != null ? CardDeck.Commander.Info.Price : 0) + (CardDeck.CommanderPartner != null ? CardDeck.CommanderPartner.Info.Price : 0);
  #endregion

  #region ISavable implementation
  [ObservableProperty] private bool hasUnsavedChanges = false;

  /// <summary>
  /// Shows dialog that asks the user if they want to save unsaved changes
  /// </summary>
  /// <returns>True, if the user does not cancel the operation</returns>
  public async Task<bool> SaveUnsavedChanges()
  {
    if (HasUnsavedChanges)
    {
      // Deck has unsaved changes
      var wantSaveConfirmed = await Dialogs.GetSaveUnsavedDialog(CardDeck.Name).ShowAsync(GetDialogWrapper(), force: true);
      if (wantSaveConfirmed == null) { return false; }
      else if (wantSaveConfirmed is true)
      {
        // User wants to save the unsaved changes
        var saveName = await Dialogs.GetSaveDialog(CardDeck.Name).ShowAsync(GetDialogWrapper());
        if (string.IsNullOrEmpty(saveName)) { return false; }
        else
        {
          if (saveName != CardDeck.Name && await DeckRepository.Exists(saveName))
          {
            // Deck exists already
            var overrideConfirmed = await Dialogs.GetOverrideDialog(saveName).ShowAsync(GetDialogWrapper());
            if (overrideConfirmed == null) { return false; }
            else if (overrideConfirmed is true)
            {
              // User wants to override the deck
              await SaveDeck(saveName);
            }
          }
          else { await SaveDeck(saveName); }
        }
      }
    }
    return true;
  }
  #endregion

  #region IINAppNotifier implementation
  public event EventHandler<NotificationService.NotificationEventArgs> OnNotification;

  public void RaiseInAppNotification(NotificationService.NotificationType type, string text)
    => OnNotification?.Invoke(this, new(type, text));
  #endregion

  #region IDialogNotifier implementation
  public event EventHandler<DialogService.DialogEventArgs> OnGetDialogWrapper;

  public DialogService.DialogWrapper GetDialogWrapper()
  {
    var args = new DialogService.DialogEventArgs();
    OnGetDialogWrapper?.Invoke(this, args);
    return args.DialogWrapper;
  }
  #endregion

  #region OnPropertyChanged events
  private void DeckCards_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(DeckCardlistViewModel.CardlistSize):
        OnPropertyChanged(nameof(DeckSize)); break;
      case nameof(DeckCardlistViewModel.EuroPrice):
        OnPropertyChanged(nameof(DeckPrice)); break;
    }
  }

  private void CardDeck_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(CardDeck.Commander):
        Commander = CardDeck?.Commander != null ? new(CardDeck.Commander)
        {
          DeleteCardCommand = SetCommanderCommand,
          ShowPrintsDialogCommand = ChangePrintDialogCommand
        } : null;
        break;
      case nameof(CardDeck.CommanderPartner):
        CommanderPartner = CardDeck?.CommanderPartner != null ? new(CardDeck.CommanderPartner)
        {
          DeleteCardCommand = SetCommanderPartnerCommand,
          ShowPrintsDialogCommand = ChangePrintDialogCommand
        } : null;
        break;
      case nameof(CardDeck.Name):
        OnPropertyChanged(nameof(DeckName));
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
      case nameof(DeckSize):
        OnPropertyChanged(nameof(DeckPrice));
        OpenPlaytestWindowCommand.NotifyCanExecuteChanged();
        break;
    }
  }

  private void Commanders_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(Commander.Price))
    {
      OnPropertyChanged(nameof(DeckPrice));
      HasUnsavedChanges = true;
    }
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
      var loadName = await Dialogs.GetLoadDialog((await DeckRepository.Get()).Select(x => x.Name).OrderBy(x => x).ToArray())
        .ShowAsync(GetDialogWrapper());
      await LoadDeck(loadName);
    }
  }

  public async Task LoadDeck(string name)
  {
    if (name != null)
    {
      IsBusy = true;
      try
      {
        if (await Task.Run(() => DeckRepository.Get(name)) is MTGCardDeck loadedDeck)
        {
          CardDeck = loadedDeck;
          RaiseInAppNotification(NotificationService.NotificationType.Success, "The deck was loaded successfully.");
        }
        else { throw new Exception(); }
      }
      catch (Exception)
      {
        RaiseInAppNotification(NotificationService.NotificationType.Error, "Error. Could not load the deck.");
      }
      IsBusy = false;
    }
  }

  /// <summary>
  /// Shows dialog that asks name for the deck and saves current deck to the database with the given name
  /// </summary>
  [RelayCommand]
  public async Task SaveDeckDialog()
  {
    var saveName = await Dialogs.GetSaveDialog(CardDeck.Name).ShowAsync(GetDialogWrapper());
    if (string.IsNullOrEmpty(saveName)) { return; }
    else
    {
      if (saveName != CardDeck.Name && await DeckRepository.Exists(saveName))
      {
        // Deck with the given name exists already
        if (await Dialogs.GetOverrideDialog(saveName).ShowAsync(GetDialogWrapper()) == null) { return; }
      }
    }

    await SaveDeck(saveName);
  }

  /// <summary>
  /// Deletes current deck from the database
  /// </summary>
  [RelayCommand(CanExecute = nameof(DeckHasName))]
  public async Task DeleteDeckDialog()
  {
    if (!await DeckRepository.Exists(CardDeck.Name)) { return; }
    if (await Dialogs.GetDeleteDialog(CardDeck.Name).ShowAsync(GetDialogWrapper()) is true)
    {
      IsBusy = true;
      if (await Task.Run(() => DeckRepository.Remove(CardDeck)))
      {
        CardDeck = new();
        RaiseInAppNotification(NotificationService.NotificationType.Success, "The deck was deleted successfully.");
      }
      else { RaiseInAppNotification(NotificationService.NotificationType.Error, "Error. Could not delete the deck."); }
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
        stringBuilder.AppendLine(token.ScryfallId.ToString());
    }

    if (Commander != null)
    {
      foreach (var token in Commander.Model.Info.Tokens)
        stringBuilder.AppendLine(token.ScryfallId.ToString());
    }

    if (CommanderPartner != null)
    {
      foreach (var token in CommanderPartner.Model.Info.Tokens)
        stringBuilder.AppendLine(token.ScryfallId.ToString());
    }

    var tokens = (await CardAPI.FetchFromString(stringBuilder.ToString())).Found.Select(x
      => new MTGCardViewModel(x)).DistinctBy(x => x.Model.Info.OracleId).ToArray(); // Filter duplicates out using oracleId
    await Dialogs.GetTokenPrintDialog(tokens).ShowAsync(GetDialogWrapper());
  }

  /// <summary>
  /// Opens Deck testing page on a new window for the given deck
  /// </summary>
  [RelayCommand(CanExecute = nameof(DeckHasCards))]
  public async Task OpenPlaytestWindow(MTGCardDeck deck)
  {
    var stringBuilder = new StringBuilder();

    foreach (var card in CardDeck.DeckCards)
    {
      foreach (var token in card.Info.Tokens)
        stringBuilder.AppendLine(token.ScryfallId.ToString());
    }

    if (Commander != null)
    {
      foreach (var token in Commander.Model.Info.Tokens)
        stringBuilder.AppendLine(token.ScryfallId.ToString());
    }

    if (CommanderPartner != null)
    {
      foreach (var token in CommanderPartner.Model.Info.Tokens)
        stringBuilder.AppendLine(token.ScryfallId.ToString());
    }

    var tokens = (await CardAPI.FetchFromString(stringBuilder.ToString())).Found.Select(x
      => new DeckTestingMTGCardViewModel(x) { IsToken = true })
      .DistinctBy(x => x.Model.Info.OracleId).ToArray(); // Filter duplicates out using oracleId

    new ThemedWindow()
    {
      Title = "MTG Deck Testing",
      Content = new DeckTestingPage(deck, tokens),
    }.Activate();
  }

  /// <summary>
  /// Opens EDHREC card search page on a new window for the given deck
  /// </summary>
  [RelayCommand(CanExecute = nameof(DeckHasCommanders))]
  public async Task OpenEDHRECWindow(MTGCardDeck deck)
  {
    var themes = await CommanderAPI.GetThemes(new Models.Structs.Commanders(
        deck.Commander?.Info.Name ?? string.Empty, deck.CommanderPartner?.Info.Name ?? string.Empty));

    new ThemedWindow()
    {
      Title = "EDHREC Search",
      Content = new EDHRECSearchPage(themes),
      Width = 550
    }.Activate();
  }

  [RelayCommand]
  /// <summary>
  /// Sets deck's commander to the given card
  /// </summary>
  public void SetCommander(MTGCard card)
    => CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.SetCommanderCommand(CardDeck, card));

  [RelayCommand]
  /// <summary>
  /// Sets deck's commander partner to the given card
  /// </summary>
  /// <param name="card"></param>
  public void SetCommanderPartner(MTGCard card)
    => CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.SetCommanderPartnerCommand(CardDeck, card));

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

    if (await Dialogs.GetCardPrintDialog(printVMs.ToArray()).ShowAsync(GetDialogWrapper()) is MTGCardViewModel newPrint)
    {
      // Replace card
      card.Info = newPrint.Model.Info;
    }
  }

  /// <summary>
  /// Opens website page for the deck's commanders using the commander API
  /// </summary>
  [RelayCommand]
  public async Task OpenEDHRECWebsite()
  {
    await IOService.OpenUri(CommanderAPI.GetCommanderWebsiteUri(new(
        Commander?.Name ?? string.Empty, CommanderPartner?.Name ?? string.Empty)));
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
      RaiseInAppNotification(NotificationService.NotificationType.Success, $"Commander was imported successfully.");
    }
    else
    {
      RaiseInAppNotification(NotificationService.NotificationType.Error, $"Could not import the commander.");
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
      RaiseInAppNotification(NotificationService.NotificationType.Success, $"Partner was imported successfully.");
    }
    else
    {
      RaiseInAppNotification(NotificationService.NotificationType.Error, $"Could not import the partner.");
    }
  }

  /// <summary>
  /// Saves current deck with the given <paramref name="name"/>
  /// </summary>
  /// <exception cref="ArgumentNullException" />
  private async Task SaveDeck(string name)
  {
    IsBusy = true;
    var tempDeck = CardDeck.GetCopy();
    tempDeck.Name = name;
    if (await Task.Run(() => DeckRepository.AddOrUpdate(tempDeck)))
    {
      // TODO: can the temp deck be moved to the AddOrUpdate method?
      // Maybe add AddOrRename method?
      if (!string.IsNullOrEmpty(CardDeck?.Name) && name != CardDeck.Name)
      {
        await DeckRepository.Remove(CardDeck); // Delete old deck if the name was changed
      }
      CardDeck.Name = name;
      HasUnsavedChanges = false;
      RaiseInAppNotification(NotificationService.NotificationType.Success, "The deck was saved successfully.");
    }
    else { RaiseInAppNotification(NotificationService.NotificationType.Error, "Error: Could not save the deck."); }
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
    ColorDevotionChart = new MTGColorAndManaPolarChart(CardDeck.DeckCards);
  }

  #region RelayCommand CanExecute Methods
  /// <summary>
  /// Returns true if deck has a name
  /// </summary>
  public bool DeckHasName() => !string.IsNullOrEmpty(CardDeck.Name);

  /// <summary>
  /// Returns true if the deck has cards in it
  /// </summary>
  public bool DeckHasCards() => DeckCards.CardlistSize > 0;

  /// <summary>
  /// Returns true if the deck has commanders in it
  /// </summary>
  public bool DeckHasCommanders() => Commander != null || CommanderPartner != null;
  #endregion
}

// Dialogs
public partial class DeckBuilderViewModel
{
  /// <summary>
  /// Deck Builder tab dialogs
  /// </summary>
  public class DeckBuilderViewDialogs
  {
    public virtual DialogService.ConfirmationDialog GetOverrideDialog(string name)
      => new("Override existing deck?") { Message = $"Deck '{name}' already exist. Would you like to override the deck?", SecondaryButtonText = string.Empty };

    public virtual DialogService.ConfirmationDialog GetDeleteDialog(string name)
      => new("Delete deck?") { Message = $"Are you sure you want to delete '{name}'?", SecondaryButtonText = string.Empty };

    public virtual DialogService.ConfirmationDialog GetSaveUnsavedDialog(string name = "")
      => new("Save unsaved changes?") { Message = $"{(string.IsNullOrEmpty(name) ? "Unnamed deck" : $"'{name}'")} has unsaved changes. Would you like to save the deck?", PrimaryButtonText = "Save" };

    public virtual DialogService.GridViewDialog<MTGCardViewModel> GetCardPrintDialog(MTGCardViewModel[] printViewModels)
      => new("Change card print", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle") { Items = printViewModels, SecondaryButtonText = string.Empty };

    public virtual DialogService.GridViewDialog<MTGCardViewModel> GetTokenPrintDialog(MTGCardViewModel[] printViewModels)
      => new("Tokens", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle") { Items = printViewModels, SecondaryButtonText = string.Empty, PrimaryButtonText = string.Empty };

    public virtual DialogService.ComboBoxDialog GetLoadDialog(string[] names)
      => new("Open deck") { InputHeader = "Name", Items = names, PrimaryButtonText = "Open", SecondaryButtonText = string.Empty };

    public virtual DialogService.TextBoxDialog GetSaveDialog(string name)
      => new("Save your deck?") { InvalidInputCharacters = Path.GetInvalidFileNameChars(), TextInputText = name, PrimaryButtonText = "Save", SecondaryButtonText = string.Empty };
  }
}

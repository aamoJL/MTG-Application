using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MTGApplication.Enums;
using static MTGApplication.Services.DialogService;

namespace MTGApplication.ViewModels
{
  public partial class DeckBuilderViewModel : ViewModelBase
  {
    public class DeckBuilderViewDialogs
    {
      #region Dialogs
      public virtual ConfirmationDialog SaveUnsavedDialog { protected get; init; } = new()
      {
        Title = "Save unsaved changes?",
        Content = "Would you like to save unsaved changes?",
        YesButtonText = "Save",
      };
      public virtual ConfirmationDialog OverrideDialog { protected get; init; } = new()
      {
        Title = "Override existing deck?",
        NoButtonText = string.Empty,
      };
      public virtual ConfirmationDialog DeleteDialog { protected get; init; } = new()
      {
        Title = "Delete deck?",
        NoButtonText = string.Empty,
      };
      public virtual InputStringDialog SaveDialog { protected get; init; } = new()
      {
        Title = "Save your deck?",
        OkButtonText = "Save",
        InvalidCharacters = Path.GetInvalidFileNameChars(),
      };
      public virtual ComboboxDialog LoadDialog { protected get; init; } = new()
      {
        Title = "Open deck",
        OkButtonText = "Open",
        Header = "Name"
      };
      public virtual InputTextAreaDialog ImportDialog { protected get; init; } = new()
      {
        Title = "Import cards",
        InputPlaceholder = "Example:\n2 Black Lotus\nMox Ruby",
        OkButtonText = "Add to Collection",
      };
      public virtual InputTextAreaDialog ExportDialog { protected get; init; } = new()
      {
        Title = "Export deck",
        OkButtonText = "Copy to Clipboard",
      };
      #endregion

      #region Getters
      public ConfirmationDialog GetSaveUnsavedDialog() => SaveUnsavedDialog;
      public ConfirmationDialog GetOverrideDialog(string name)
      {
        OverrideDialog.Content = $"Deck '{name}' already exist. Would you like to override the deck?";
        return OverrideDialog;
      }
      public ConfirmationDialog GetDeleteDialog(string name)
      {
        DeleteDialog.Content = $"Are you sure you want to delete '{name}'?";
        return DeleteDialog;
      }
      public InputStringDialog GetSaveDialog(string name)
      {
        SaveDialog.DefaultText = name;
        return SaveDialog;
      }
      public ComboboxDialog GetLoadDialog(string[] names)
      {
        LoadDialog.Items = names;
        return LoadDialog;
      }
      public InputTextAreaDialog GetImportDialog() => ImportDialog;
      public InputTextAreaDialog GetExportDialog(string text)
      {
        ExportDialog.DefaultText = text;
        return ExportDialog;
      }
      #endregion
    }
    public partial class Cardlist : ObservableObject
    {
      public Cardlist(MTGCardDeck deck, CardlistType listType, DeckBuilderViewDialogs dialogs, ICardAPI<MTGCard> cardAPI, SortMTGProperty sortProp = SortMTGProperty.CMC, SortDirection sortDir = SortDirection.ASC, IO.ClipboardService clipboardService = null)
      {
        CardDeck = deck;
        ListType = listType;
        Dialogs = dialogs;
        CardAPI = cardAPI;
        SortDirection = sortDir;
        SortProperty = sortProp;
        ClipboardService = clipboardService ?? new();

        PropertyChanged += Cardlist_PropertyChanged;
        OnPropertyChanged(nameof(CardDeck));
      }

      private void Cardlist_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
        if (e.PropertyName == nameof(CardDeck))
        {
          CardDeck.GetCardlist(ListType).CollectionChanged += Cards_CollectionChanged;
          CardViewModels.Clear();
          foreach (var card in CardDeck.GetCardlist(ListType))
          {
            card.PropertyChanged += Card_PropertyChanged;
            CardViewModels.Add(new(card) { DeleteCardCommand = new RelayCommand<MTGCard>(RemoveFromCardlist) });
          }
          OnPropertyChanged(nameof(CardlistSize));
          SortCardViewModels();
          HasUnsavedChanges = false;
        }
        else if (e.PropertyName == nameof(SortDirection) || e.PropertyName == nameof(SortProperty))
        {
          SortCardViewModels();
        }
      }
      private void Card_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
        if (e.PropertyName == nameof(MTGCard.Count))
        {
          OnPropertyChanged(nameof(CardlistSize));
          HasUnsavedChanges = true;
        }
      }
      private void Cards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        // Sync models and viewmodels
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            var newCard = e.NewItems[0] as MTGCard;
            CardViewModels.Add(new(newCard) { DeleteCardCommand = new RelayCommand<MTGCard>(RemoveFromCardlist) });
            newCard.PropertyChanged += Card_PropertyChanged;
            SortCardViewModels();
            break;
          case NotifyCollectionChangedAction.Remove:
            var oldCard = e.OldItems[0] as MTGCard;
            CardViewModels.Remove(CardViewModels.FirstOrDefault(x => x.Model == oldCard));
            oldCard.PropertyChanged -= Card_PropertyChanged;
            break;
          case NotifyCollectionChangedAction.Reset:
            CardViewModels.Clear(); break;
          default: break;
        }
        OnPropertyChanged(nameof(CardlistSize));
        HasUnsavedChanges = true;
      }

      [ObservableProperty]
      private bool isBusy;
      [ObservableProperty]
      private MTGCardDeck cardDeck;
      [ObservableProperty]
      private SortMTGProperty sortProperty;
      [ObservableProperty]
      private SortDirection sortDirection;
      [ObservableProperty]
      private bool hasUnsavedChanges;

      private IO.ClipboardService ClipboardService { get; }
      private CardlistType ListType { get; }
      private DeckBuilderViewDialogs Dialogs { get; }
      private ICardAPI<MTGCard> CardAPI { get; }

      public ObservableCollection<MTGCardViewModel> CardViewModels { get; } = new();
      public int CardlistSize => CardViewModels.Sum(x => x.Model.Count);

      /// <summary>
      /// Asks cards to import and imports them to the deck
      /// </summary>
      [RelayCommand]
      public async Task ImportToCardlistDialog()
      {
        await ImportCards(await Dialogs.GetImportDialog().Show());
      }
      /// <summary>
      /// Shows dialog with formatted text of the cardlist cards
      /// </summary>
      [RelayCommand]
      public async Task ExportDeckCardsDialog()
      {
        await ExportCards();
      }
      /// <summary>
      /// Adds given card to the deck cardlist
      /// </summary>
      [RelayCommand]
      public void AddToCardlist(MTGCard card)
      {
        CardDeck.AddToCardlist(ListType, card);
      }
      /// <summary>
      /// Removes given card from the deck cardlist
      /// </summary>
      [RelayCommand]
      public void RemoveFromCardlist(MTGCard card)
      {
        CardDeck.RemoveFromCardlist(ListType, card);
      }

      private async Task ExportCards()
      {
        StringBuilder stringBuilder = new();
        foreach (var item in CardViewModels)
        {
          stringBuilder.AppendLine($"{item.Model.Count} {item.Model.Info.Name}");
        }

        var response = await Dialogs.GetExportDialog(stringBuilder.ToString()).Show();
        if (!string.IsNullOrEmpty(response))
        {
          ClipboardService.Copy(response);
        }
      }
      public async Task ImportCards(string importText)
      {
        var notFoundCount = 0;
        var found = Array.Empty<MTGCard>();

        if (!string.IsNullOrEmpty(importText))
        {
          IsBusy = true;
          (found, notFoundCount) = await CardAPI.FetchFromString(importText);
          foreach (var card in found)
          {
            CardDeck.AddToCardlist(ListType, card);
          }
        }

        if (notFoundCount == 0 && found.Length > 0)
          Notifications.RaiseNotification(Notifications.NotificationType.Success, $"{notFoundCount + found.Length} cards imported successfully.");
        else if(found.Length > 0 && notFoundCount > 0)
          Notifications.RaiseNotification(Notifications.NotificationType.Warning, $"{found.Length} / {notFoundCount + found.Length} cards imported successfully.{Environment.NewLine}{notFoundCount} cards were not found.");
        else if (found.Length == 0)
          Notifications.RaiseNotification(Notifications.NotificationType.Error, $"Error. No cards were imported.");

        IsBusy = false;
      }
      private void SortCardViewModels()
      {
        List<MTGCardViewModel> tempList = new();
        var prop = SortProperty;
        var dir = SortDirection;

        tempList = prop switch
        {
          SortMTGProperty.CMC => dir == SortDirection.ASC ? CardViewModels.OrderBy(x => x.Model.Info.CMC).ToList() : CardViewModels.OrderByDescending(x => x.Model.Info.CMC).ToList(),
          SortMTGProperty.Name => dir == SortDirection.ASC ? CardViewModels.OrderBy(x => x.Model.Info.Name).ToList() : CardViewModels.OrderByDescending(x => x.Model.Info.Name).ToList(),
          SortMTGProperty.Rarity => dir == SortDirection.ASC ? CardViewModels.OrderBy(x => x.Model.Info.RarityType).ToList() : CardViewModels.OrderByDescending(x => x.Model.Info.RarityType).ToList(),
          SortMTGProperty.Color => dir == SortDirection.ASC ? CardViewModels.OrderBy(x => x.Model.Info.Colors).ToList() : CardViewModels.OrderByDescending(x => x.Model.Info.Colors).ToList(),
          SortMTGProperty.Set => dir == SortDirection.ASC ? CardViewModels.OrderBy(x => x.Model.Info.SetName).ToList() : CardViewModels.OrderByDescending(x => x.Model.Info.SetName).ToList(),
          SortMTGProperty.Count => dir == SortDirection.ASC ? CardViewModels.OrderBy(x => x.Model.Count).ToList() : CardViewModels.OrderByDescending(x => x.Model.Count).ToList(),
          SortMTGProperty.Price => dir == SortDirection.ASC ? CardViewModels.OrderBy(x => x.Model.Info.Price).ToList() : CardViewModels.OrderByDescending(x => x.Model.Info.Price).ToList(),
          SortMTGProperty.Type => dir == SortDirection.ASC ? CardViewModels.OrderBy(x => x.Model.Info.SpellTypes[0]).ToList() : CardViewModels.OrderByDescending(x => x.Model.Info.SpellTypes[0]).ToList(),
          _ => throw new NotImplementedException(),
        };

        for (int i = 0; i < tempList.Count; i++)
        {
          CardViewModels.Move(CardViewModels.IndexOf(tempList[i]), i);
        }
      }
    }

    public DeckBuilderViewModel(ICardAPI<MTGCard> cardAPI, IDeckRepository<MTGCardDeck> deckService, IO.ClipboardService clipboardService = null, DeckBuilderViewDialogs dialogs = null)
    {
      DeckRepository = deckService;
      CardAPI = cardAPI;
      Dialogs = dialogs ?? new();

      PropertyChanged += DeckBuilderViewModel_PropertyChanged;
      CardDeck.PropertyChanged += CardDeck_PropertyChanged;

      clipboardService ??= new();
      DeckCards = new Cardlist(CardDeck, CardlistType.Deck, Dialogs, CardAPI, clipboardService: clipboardService);
      WishlistCards = new Cardlist(CardDeck, CardlistType.Wishlist, Dialogs, CardAPI, clipboardService: clipboardService);
      MaybelistCards = new Cardlist(CardDeck, CardlistType.Maybelist, Dialogs, CardAPI, clipboardService: clipboardService);

      DeckCards.PropertyChanged += Cardlist_PropertyChanged;
      WishlistCards.PropertyChanged += Cardlist_PropertyChanged;
      MaybelistCards.PropertyChanged += Cardlist_PropertyChanged;

      SelectedSortDirection = SortDirection.ASC;
      SelectedSortProperty = SortMTGProperty.CMC;
      UpdateCharts();
    }

    private void Cardlist_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Cardlist.IsBusy))
      {
        IsBusy = DeckCards.IsBusy || WishlistCards.IsBusy || MaybelistCards.IsBusy;
      }
      else if(e.PropertyName == nameof(HasUnsavedChanges)) { OnPropertyChanged(nameof(HasUnsavedChanges)); }
    }
    private void CardDeck_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(CardDeck.Name)) { OnPropertyChanged(nameof(CardDeckName)); }
    }
    private void DeckBuilderViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(CardDeck))
      {
        CardDeck.PropertyChanged += CardDeck_PropertyChanged;

        DeckCards.CardDeck = CardDeck;
        WishlistCards.CardDeck = CardDeck;
        MaybelistCards.CardDeck = CardDeck;
        UpdateCharts();
        OnPropertyChanged(nameof(CardDeckName));
      }
    }

    private MTGCardDeck cardDeck = new();

    private IDeckRepository<MTGCardDeck> DeckRepository { get; }
    private ICardAPI<MTGCard> CardAPI { get; }
    private MTGCardDeck CardDeck
    {
      get => cardDeck;
      set
      {
        cardDeck = value;
        OnPropertyChanged(nameof(CardDeck));
      }
    }
    private DeckBuilderViewDialogs Dialogs { get; }

    [ObservableProperty]
    private bool isBusy;

    public Cardlist DeckCards { get; set; }
    public Cardlist WishlistCards { get; set; }
    public Cardlist MaybelistCards { get; set; }
    public MTGCMCStackedColumnChart CMCChart { get; set; }
    public MTGSpellTypePieChart SpellTypeChart { get; set; }
    public MTGManaProductionPieChart ManaProductionChart { get; set; }
    public MTGColorPieChart ColorChart { get; set; }

    public SortMTGProperty SelectedSortProperty
    {
      set
      {
        DeckCards.SortProperty = value;
        WishlistCards.SortProperty = value;
        MaybelistCards.SortProperty = value;
      }
    }
    public SortDirection SelectedSortDirection
    {
      set
      {
        DeckCards.SortDirection = value;
        WishlistCards.SortDirection = value;
        MaybelistCards.SortDirection = value;
      }
    }
    public bool HasUnsavedChanges
    {
      get
      {
        return DeckCards.HasUnsavedChanges || WishlistCards.HasUnsavedChanges || MaybelistCards.HasUnsavedChanges;
      }
      set
      {
        DeckCards.HasUnsavedChanges = value;
        WishlistCards.HasUnsavedChanges = value;
        MaybelistCards.HasUnsavedChanges = value;
      }
    }
    public string CardDeckName => CardDeck.Name;

    /// <summary>
    /// Shows UnsavedChanges dialog and clear current card deck
    /// </summary>
    [RelayCommand]
    public async Task NewDeckDialog()
    {
      if (await ShowUnsavedDialogs()) await NewDeck();
    }

    /// <summary>
    /// Shows dialog with names of the saved decks and changes current deck to the selected deck from the database
    /// </summary>
    [RelayCommand]
    public async Task LoadDeckDialog()
    {
      if (await ShowUnsavedDialogs())
      {
        var loadName = await Dialogs.GetLoadDialog((await DeckRepository.Get()).Select(x => x.Name).ToArray()).Show();
        if (loadName != string.Empty)
        {
          await LoadDeck(loadName);
        }
      }
    }

    /// <summary>
    /// Shows dialog that asks name for the deck and saves current deck to the database with the given name
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteSaveDeckDialogCommand))]
    public async Task SaveDeckDialog()
    {
      var saveName = await Dialogs.GetSaveDialog(CardDeck.Name).Show();
      if (string.IsNullOrEmpty(saveName)) { return; }
      else
      {
        if (saveName != CardDeck.Name && await DeckRepository.Exists(saveName))
        {
          // Deck exists already
          if (await Dialogs.GetOverrideDialog(saveName).Show() == null) { return; }
        }
      }

      await SaveDeck(saveName);
    }

    /// <summary>
    /// Deletes current deck from the database
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteDeleteDeckDialogCommand))]
    public async Task DeleteDeckDialog()
    {
      if (!await DeckRepository.Exists(CardDeck.Name)) { return; }
      if (await Dialogs.GetDeleteDialog(CardDeck.Name).Show() is true)
      {
        await DeleteDeck();
      }
    }

    /// <summary>
    /// Sorts selected card deck by selected property using given direction
    /// </summary>
    /// <param name="dir">Sorting direction, NOT case-sensitive</param>
    [RelayCommand]
    public void SortByDirection(string dir)
    {
      if (Enum.TryParse(dir, true, out SortDirection sortDirection))
      {
        SelectedSortDirection = sortDirection;
      }
    }

    /// <summary>
    /// Sorts selected card deck by given property
    /// </summary>
    /// <param name="prop">Name of the property, NOT case-sensitive</param>
    [RelayCommand]
    public void SortByProperty(string prop)
    {
      if (Enum.TryParse(prop, true, out SortMTGProperty sortProperty))
      {
        SelectedSortProperty = sortProperty;
      }
    }

    #region CanExecuteCommand Methods
    private bool CanExecuteSaveDeckDialogCommand() => DeckCards.CardlistSize > 0;
    private bool CanExecuteDeleteDeckDialogCommand() => !string.IsNullOrEmpty(CardDeck.Name);
    #endregion

    private async Task NewDeck()
    {
      IsBusy = true;
      CardDeck = await Task.Run(() => new MTGCardDeck());
      IsBusy = false;
    }
    private async Task SaveDeck(string name)
    {
      IsBusy = true;
      var saveDeck = new MTGCardDeck()
      {
        Name = name,
        DeckCards = CardDeck.DeckCards,
        Maybelist = CardDeck.Maybelist,
        Wishlist = CardDeck.Wishlist,
      };
      if (await Task.Run(() => DeckRepository.AddOrUpdate(saveDeck)))
      {
        CardDeck.Name = name;
        HasUnsavedChanges = false;
        Notifications.RaiseNotification(Notifications.NotificationType.Success, "The deck was saved successfully.");
      }
      else { Notifications.RaiseNotification(Notifications.NotificationType.Error, "Error. Could not save the deck."); }
      IsBusy = false;
    }
    private async Task LoadDeck(string name)
    {
      IsBusy = true;
      if(await Task.Run(() => DeckRepository.Get(name)) is MTGCardDeck loadedDeck)
      {
        CardDeck = loadedDeck;
        Notifications.RaiseNotification(Notifications.NotificationType.Success, "The deck was loaded successfully.");
      }
      else { Notifications.RaiseNotification(Notifications.NotificationType.Error, "Error. Could not load the deck."); }
      IsBusy = false;
    }
    private async Task DeleteDeck()
    {
      IsBusy = true;
      if (await Task.Run(() => DeckRepository.Remove(CardDeck)))
      {
        CardDeck = new();
        Notifications.RaiseNotification(Notifications.NotificationType.Success, "The deck was deleted successfully.");
      }
      else { Notifications.RaiseNotification(Notifications.NotificationType.Error, "Error. Could not delete the deck."); }
      IsBusy = false;
    }
    private void UpdateCharts()
    {
      CMCChart = new MTGCMCStackedColumnChart(CardDeck.DeckCards);
      SpellTypeChart = new MTGSpellTypePieChart(CardDeck.DeckCards);
      ManaProductionChart = new MTGManaProductionPieChart(CardDeck.DeckCards);
      ColorChart = new MTGColorPieChart(CardDeck.DeckCards);
      OnPropertyChanged(nameof(CMCChart));
      OnPropertyChanged(nameof(SpellTypeChart));
      OnPropertyChanged(nameof(ManaProductionChart));
      OnPropertyChanged(nameof(ColorChart));
    }
    
    /// <summary>
    /// Asks user if they want to save the deck's unsaved changes.
    /// </summary>
    /// <returns><see langword="true"/>, if user does not cancel any of the dialogs</returns>
    public async Task<bool> ShowUnsavedDialogs()
    {
      if (HasUnsavedChanges)
      {
        // Deck has unsaved changes
        var wantSaveConfirmed = await Dialogs.GetSaveUnsavedDialog().Show();
        if (wantSaveConfirmed == null) { return false; }
        else if (wantSaveConfirmed is true)
        {
          // User wants to save the unsaved changes
          var saveName = await Dialogs.GetSaveDialog(CardDeck.Name).Show();
          if (string.IsNullOrEmpty(saveName)) { return false; }
          else
          {
            if (saveName != CardDeck.Name && await DeckRepository.Exists(saveName))
            {
              // Deck exists already
              var overrideConfirmed = await Dialogs.GetOverrideDialog(saveName).Show();
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
  }
}

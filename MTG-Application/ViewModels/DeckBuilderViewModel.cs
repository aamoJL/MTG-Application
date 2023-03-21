using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Services;
using MTGApplication.ViewModels.Charts;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MTGApplication.Enums;
using static MTGApplication.Views.Dialogs;

namespace MTGApplication.ViewModels
{
  public partial class DeckBuilderViewModel : ViewModelBase
  {
    public class DeckBuilderViewDialogs
    {
      #region Dialogs
      public CheckBoxDialog MultipleCardsAlreadyInDeckDialog { protected get; set; } = new CheckBoxDialog("Card already in the deck");
      public ConfirmationDialog CardAlreadyInDeckDialog { protected get; set; } = new ConfirmationDialog("Card already in the deck");
      public ConfirmationDialog SaveUnsavedDialog { protected get; set; } = new ConfirmationDialog("Save unsaved changes?");
      public ConfirmationDialog OverrideDialog { protected get; set; } = new ConfirmationDialog("Override existing deck?");
      public ConfirmationDialog DeleteDialog { protected get; set; } = new ConfirmationDialog("Delete deck?");
      public GridViewDialog CardPrintDialog { protected get; set; } = new GridViewDialog("Change card print");
      public TextAreaDialog ImportDialog { protected get; set; } = new TextAreaDialog("Import cards");
      public TextAreaDialog ExportDialog { protected get; set; } = new TextAreaDialog("Export deck");
      public TextBoxDialog SaveDialog { protected get; set; } = new TextBoxDialog("Save your deck?");
      public ComboBoxDialog LoadDialog { protected get; set; } = new ComboBoxDialog("Open deck");
      #endregion

      #region Getters
      public GridViewDialog GetCardPrintDialog(MTGCardViewModel[] printViewModels)
      {
        CardPrintDialog.Items = printViewModels;
        CardPrintDialog.SecondaryButtonText = string.Empty;
        CardPrintDialog.GridViewStyleName = "MTGAdaptiveGridViewStyle";
        CardPrintDialog.ItemTemplateName = "MTGPrintGridViewItemTemplate";
        return CardPrintDialog;
      }
      public CheckBoxDialog GetMultipleCardsAlreadyInDeckDialog(string name)
      {
        MultipleCardsAlreadyInDeckDialog.SecondaryButtonText = string.Empty;
        MultipleCardsAlreadyInDeckDialog.Message = $"Card '{name}' is already in the deck. Do you still want to add it?";
        MultipleCardsAlreadyInDeckDialog.CloseButtonText = "No";
        MultipleCardsAlreadyInDeckDialog.InputText = "Same for all cards.";
        return MultipleCardsAlreadyInDeckDialog;
      }
      public ConfirmationDialog GetCardAlreadyInDeckDialog(string name)
      {
        CardAlreadyInDeckDialog.SecondaryButtonText = string.Empty;
        CardAlreadyInDeckDialog.Message = $"Card '{name}' is already in the deck. Do you still want to add it?";
        CardAlreadyInDeckDialog.CloseButtonText = "No";
        return CardAlreadyInDeckDialog;
      }
      public ConfirmationDialog GetOverrideDialog(string name)
      {
        OverrideDialog.SecondaryButtonText = string.Empty;
        OverrideDialog.Message = $"Deck '{name}' already exist. Would you like to override the deck?";
        return OverrideDialog;
      }
      public ConfirmationDialog GetDeleteDialog(string name)
      {
        DeleteDialog.SecondaryButtonText = string.Empty;
        DeleteDialog.Message = $"Are you sure you want to delete '{name}'?";
        return DeleteDialog;
      }
      public ComboBoxDialog GetLoadDialog(string[] names)
      {
        LoadDialog.PrimaryButtonText = "Open";
        LoadDialog.SecondaryButtonText = string.Empty;
        LoadDialog.InputHeader = "Name";
        LoadDialog.Items = names;
        return LoadDialog;
      }
      public TextAreaDialog GetExportDialog(string text)
      {
        ExportDialog.PrimaryButtonText = "Copy to Clipboard";
        ExportDialog.SecondaryButtonText = string.Empty;
        ExportDialog.InputDefaultText = text;
        return ExportDialog;
      }
      public ConfirmationDialog GetSaveUnsavedDialog()
      {
        SaveUnsavedDialog.Message = "Would you like to save unsaved changes?";
        SaveUnsavedDialog.PrimaryButtonText = "Save";
        return SaveUnsavedDialog;
      }
      public TextBoxDialog GetSaveDialog(string name)
      {
        SaveDialog.PrimaryButtonText = "Save";
        SaveDialog.SecondaryButtonText = string.Empty;
        SaveDialog.InvalidInputCharacters = Path.GetInvalidFileNameChars();
        SaveDialog.InputDefaultText = name;
        return SaveDialog;
      }
      public TextAreaDialog GetImportDialog()
      {
        ImportDialog.InputPlaceholderText = "Example:\n2 Black Lotus\nMox Ruby\nbd8fa327-dd41-4737-8f19-2cf5eb1f7cdd";
        ImportDialog.SecondaryButtonText = string.Empty;
        ImportDialog.PrimaryButtonText = "Add to Collection";
        return ImportDialog;
      }
      #endregion
    }

    public partial class Cardlist : ObservableObject
    {
      public partial class CardFilters : ObservableObject
      {
        [ObservableProperty]
        private string nameText = string.Empty;
        [ObservableProperty]
        private string typeText = string.Empty;
        [ObservableProperty]
        private bool white = true;
        [ObservableProperty]
        private bool blue = true;
        [ObservableProperty]
        private bool black = true;
        [ObservableProperty]
        private bool red = true;
        [ObservableProperty]
        private bool green = true;
        [ObservableProperty]
        private bool colorless = true;
        [ObservableProperty]
        private string colorGroup = "All"; // All, Mono, Multi

        /// <summary>
        /// Returns <see langword="true"/> if any of the filter properties has been changed from the default value
        /// </summary>
        public bool FiltersApplied
        {
          get
          {
            return !string.IsNullOrEmpty(NameText) || !string.IsNullOrEmpty(TypeText) || !White || !Blue || !Black || !Red || !Green || !Colorless || ColorGroup != "All";
          }
        }

        /// <summary>
        /// returns <see langword="true"/> if the given <paramref name="card"/> is valid with the selected filters
        /// </summary>
        public bool CardValidation(MTGCardViewModel cardViewModel)
        {
          if (cardViewModel.Name.Contains(NameText, StringComparison.OrdinalIgnoreCase)
            && cardViewModel.TypeLine.Contains(TypeText, StringComparison.OrdinalIgnoreCase)
            && (White || !cardViewModel.Colors.Contains(MTGCard.ColorTypes.W))
            && (Blue || !cardViewModel.Colors.Contains(MTGCard.ColorTypes.U))
            && (Black || !cardViewModel.Colors.Contains(MTGCard.ColorTypes.B))
            && (Red || !cardViewModel.Colors.Contains(MTGCard.ColorTypes.R))
            && (Green || !cardViewModel.Colors.Contains(MTGCard.ColorTypes.G))
            && (Colorless || !cardViewModel.Colors.Contains(MTGCard.ColorTypes.C))
            && (ColorGroup == "All" 
              || ColorGroup == "Mono" && cardViewModel.Colors.Length == 1 
              || (ColorGroup == "Multi" && cardViewModel.Colors.Length > 1))) { return true; }
          else { return false; };
        }
        
        /// <summary>
        /// Reset filter properties to the default values
        /// </summary>
        [RelayCommand]
        public void Reset()
        {
          NameText = string.Empty;
          TypeText = string.Empty;
          White = true;
          Blue = true;
          Black = true;
          Red = true;
          Green = true;
          Colorless = true;
          ColorGroup = "All";
        }

        [RelayCommand]
        public void ChangeColorGroup(string group)
        {
          if (!new string[] {"All", "Mono", "Multi" }.Contains(group)) { throw new NotImplementedException(); }
          ColorGroup = group;
        }
      }

      public Cardlist(MTGCardDeck deck, CardlistType listType, DeckBuilderViewDialogs dialogs, ICardAPI<MTGCard> cardAPI, IO.ClipboardService clipboardService = default)
      {
        CardViewModels = new();
        FilteredAndSortedCardViewModels = new(CardViewModels, true);
        FilteredAndSortedCardViewModels.SortDescriptions.Add(new SortDescription(MTGCardViewModel.GetPropertyName(primarySortProperty), sortDirection));
        FilteredAndSortedCardViewModels.SortDescriptions.Add(new SortDescription(MTGCardViewModel.GetPropertyName(secondarySortProperty), sortDirection));
        Filters = new();

        CardDeck = deck;
        ListType = listType;
        Dialogs = dialogs;
        CardAPI = cardAPI;
        ClipboardService = clipboardService ?? new();

        Filters.PropertyChanged += Filters_PropertyChanged;
        PropertyChanged += Cardlist_PropertyChanged;
        CardViewModels.CollectionChanged += CardViewModels_CollectionChanged;
        OnPropertyChanged(nameof(CardDeck));
      }

      private void CardViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        OnPropertyChanged(nameof(CardlistSize));
      }
      private void Filters_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
        FilterViewModels();
      }
      private void CardViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
        switch (e.PropertyName)
        {
          case nameof(MTGCardViewModel.Count): OnPropertyChanged(nameof(CardlistSize)); HasUnsavedChanges = true; break;
          case nameof(MTGCardViewModel.Price): OnPropertyChanged(nameof(EuroPrice)); HasUnsavedChanges = true; break;
          default: break;
        }
      }
      private void Cardlist_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
        switch (e.PropertyName)
        {
          case nameof(CardDeck):
            CardDeck.GetCardlist(ListType).CollectionChanged += CardModels_CollectionChanged;
            CardViewModels.Clear();
            AddCardViewModels(CardDeck.GetCardlist(ListType).ToArray());
            HasUnsavedChanges = false; break;
          case nameof(SortDirection):
          case nameof(PrimarySortProperty):
          case nameof(SecondarySortProperty):
            FilteredAndSortedCardViewModels.SortDescriptions[0] = new SortDescription(MTGCardViewModel.GetPropertyName(PrimarySortProperty), SortDirection);
            FilteredAndSortedCardViewModels.SortDescriptions[1] = new SortDescription(MTGCardViewModel.GetPropertyName(SecondarySortProperty), SortDirection); break;
          case nameof(CardlistSize): OnPropertyChanged(nameof(EuroPrice)); break;
          default: break;
        }
      }
      private void CardModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        // Sync models and viewmodels
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add: AddCardViewModels(e.NewItems[0] as MTGCard); break;
          case NotifyCollectionChangedAction.Remove: RemoveCardViewModel(e.OldItems[0] as MTGCard); break;
          case NotifyCollectionChangedAction.Reset: CardViewModels.Clear(); break;
          default: break;
        }
        HasUnsavedChanges = true;
      }

      [ObservableProperty]
      private MTGSortProperty primarySortProperty = MTGSortProperty.CMC;
      [ObservableProperty]
      private MTGSortProperty secondarySortProperty = MTGSortProperty.Name;
      [ObservableProperty]
      private SortDirection sortDirection = SortDirection.Ascending;
      [ObservableProperty]
      private bool hasUnsavedChanges;
      [ObservableProperty]
      private MTGCardDeck cardDeck;
      [ObservableProperty]
      private bool isBusy;

      private ObservableCollection<MTGCardViewModel> CardViewModels { get; }
      private IO.ClipboardService ClipboardService { get; }
      private DeckBuilderViewDialogs Dialogs { get; }
      private ICardAPI<MTGCard> CardAPI { get; }
      private CardlistType ListType { get; }

      public AdvancedCollectionView FilteredAndSortedCardViewModels { get; }
      public CardFilters Filters { get; }
      
      /// <summary>
      /// Returns total card count of the cardlist cards
      /// </summary>
      public int CardlistSize => CardViewModels.Sum(x => x.Model.Count);
      /// <summary>
      /// Returns total euro price of the cardlist cards
      /// </summary>
      public float EuroPrice => (float)Math.Round(CardViewModels.Sum(x => x.Model.Info.Price * x.Model.Count), 0);

      /// <summary>
      /// Asks cards to import and imports them to the deck
      /// </summary>
      [RelayCommand]
      public async Task ImportToCardlistDialog()
      {
        if(await Dialogs.GetImportDialog().ShowAsync(App.MainRoot) is string importText)
        {
          await ImportCards(importText);
        }
      }
      /// <summary>
      /// Shows dialog with formatted text of the cardlist cards
      /// </summary>
      /// <param name="exportProperty">'Name' or 'Id'</param>
      [RelayCommand]
      public async Task ExportDeckCardsDialog(string exportProperty = "Name")
      {
        await ExportCards(exportProperty);
      }
      /// <summary>
      /// Adds given card to the deck cardlist
      /// </summary>
      [RelayCommand]
      public async Task AddToCardlist(MTGCard card)
      {
        if(CardDeck.GetCardlist(ListType).FirstOrDefault(x => x.Info.Name == card.Info.Name) is not null)
        {
          if((await Dialogs.GetCardAlreadyInDeckDialog(card.Info.Name).ShowAsync(App.MainRoot)) is true)
          {
            CardDeck.AddToCardlist(ListType, card);
          }
        }
        else { CardDeck.AddToCardlist(ListType, card); }
      }
      /// <summary>
      /// Removes given card from the deck cardlist
      /// </summary>
      [RelayCommand]
      public void RemoveFromCardlist(MTGCard card)
      {
        CardDeck.RemoveFromCardlist(ListType, card);
      }
      /// <summary>
      /// Shows a dialog with cards prints and changes the cards print to the selected print
      /// </summary>
      [RelayCommand]
      public async Task ChangePrintDialog(MTGCard card)
      {
        // Get prints
        IsBusy = true;
        var prints = await CardAPI.FetchCardsFromUri(card.Info.PrintSearchUri, paperOnly: true);
        var printViewModels = prints.Select(x => new MTGCardViewModel(x)).ToArray();
        IsBusy = false;

        if (await Dialogs.GetCardPrintDialog(printViewModels).ShowAsync(App.MainRoot) is MTGCardViewModel newPrint)
        {
          // Replace card
          var cardlist = CardDeck.GetCardlist(ListType);
          int index = cardlist.IndexOf(card);
          if (index > -1)
          {
            cardlist[index].Info = newPrint.Model.Info;
          }
        }
      }
      /// <summary>
      /// Imports cards from the card API using the <paramref name="importText"/>
      /// Shows a dialog that asks the user if they want to skip already existing cards.
      /// </summary>
      [RelayCommand]
      public async Task ImportCards(string importText)
      {
        var notFoundCount = 0;
        var found = Array.Empty<MTGCard>();
        var notImportedCount = 0;

        if (!string.IsNullOrEmpty(importText))
        {
          IsBusy = true;
          bool? import = true;
          bool? skipDialog = false;
          (found, notFoundCount) = await CardAPI.FetchFromString(importText);
          foreach (var card in found)
          {
            if (CardDeck.GetCardlist(ListType).FirstOrDefault(x => x.Info.Name == card.Info.Name) is not null)
            {
              // Card exist in the deck, ask if want to import, unless user has checked the dialog skip checkbox in the dialog
              if (skipDialog is not true)
              {
                (import, skipDialog) = await Dialogs.GetMultipleCardsAlreadyInDeckDialog(card.Info.Name).ShowAsync(App.MainRoot);
              }

              if (import is true) { CardDeck.AddToCardlist(ListType, card); }
              else { notImportedCount++; }
            }
            else { CardDeck.AddToCardlist(ListType, card); }
          }
        }

        if (notFoundCount == 0 && found.Length > 0)
          Notifications.RaiseNotification(Notifications.NotificationType.Success, $"{found.Length - notImportedCount} cards imported successfully." + (notImportedCount > 0 ? $" ({notImportedCount} cards skipped) " : ""));
        else if(found.Length > 0 && notFoundCount > 0)
          Notifications.RaiseNotification(Notifications.NotificationType.Warning,
            $"{found.Length} / {notFoundCount + found.Length} cards imported successfully.{Environment.NewLine}{notFoundCount} cards were not found." + (notImportedCount > 0 ? $" ({notImportedCount} cards skipped) " : ""));
        else if (found.Length == 0)
          Notifications.RaiseNotification(Notifications.NotificationType.Error, $"Error. No cards were imported.");

        IsBusy = false;
      }

      /// <summary>
      /// Adds given <paramref name="cards"/> to <see cref="CardViewModels"/> list as a <see cref="MTGCardViewModel"/>
      /// </summary>
      private void AddCardViewModels(MTGCard[] cards)
      {
        foreach (var card in cards)
        {
          var vm = new MTGCardViewModel(card) { DeleteCardCommand = RemoveFromCardlistCommand, ChangePrintDialogCommand = ChangePrintDialogCommand };
          vm.PropertyChanged += CardViewModel_PropertyChanged;
          CardViewModels.Add(vm);
        }
      }
      /// <summary>
      /// Adds given <paramref name="card"/> to <see cref="CardViewModels"/> list as a <see cref="MTGCardViewModel"/>
      /// </summary>
      private void AddCardViewModels(MTGCard card)
      {
        var vm = new MTGCardViewModel(card) { DeleteCardCommand = RemoveFromCardlistCommand, ChangePrintDialogCommand = ChangePrintDialogCommand };
        vm.PropertyChanged += CardViewModel_PropertyChanged;
        CardViewModels.Add(vm);
      }
      /// <summary>
      /// Removes given <paramref name="card"/> from the <see cref="CardViewModels"/> list
      /// </summary>
      private void RemoveCardViewModel(MTGCard card)
      {
        var vm = CardViewModels.FirstOrDefault(x => x.Model == card);
        if (vm != null)
        {
          vm.PropertyChanged -= CardViewModel_PropertyChanged;
          CardViewModels.Remove(vm);
        }
      }
      /// <summary>
      /// Filters <see cref="FilteredAndSortedCardViewModels"/> list with the given <paramref name="filterText"/>
      /// </summary>
      private void FilterViewModels()
      {
        if (Filters.FiltersApplied)
        {
          FilteredAndSortedCardViewModels.Filter = x =>
          {
            var vm = (MTGCardViewModel)x;
            return Filters.CardValidation(vm);
          };
          FilteredAndSortedCardViewModels.RefreshFilter();
        }
        else { FilteredAndSortedCardViewModels.Filter = null; }
      }

      private async Task ExportCards(string exportProperty)
      {
        var response = await Dialogs.GetExportDialog(GetExportString(exportProperty)).ShowAsync(App.MainRoot);
        if (!string.IsNullOrEmpty(response))
        {
          ClipboardService.Copy(response);
        }
      }

      public string GetExportString(string exportProperty = "Name")
      {
        StringBuilder stringBuilder = new();
        foreach (var item in CardViewModels)
        {
          if (exportProperty == "Name")
          {
            stringBuilder.AppendLine($"{item.Model.Count} {item.Model.Info.Name}");
          }
          else if (exportProperty == "Id")
          {
            stringBuilder.AppendLine($"{item.Model.Count} {item.Model.Info.ScryfallId}");
          }
        }

        return stringBuilder.ToString();
      }
    }

    public DeckBuilderViewModel(ICardAPI<MTGCard> cardAPI, IRepository<MTGCardDeck> deckRepository, IO.ClipboardService clipboardService = null, DeckBuilderViewDialogs dialogs = null)
    {
      DeckRepository = deckRepository;
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

      SelectedSortDirection = SortDirection.Ascending;
      SelectedPrimarySortProperty = MTGSortProperty.CMC;
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

    private IRepository<MTGCardDeck> DeckRepository { get; }
    private DeckBuilderViewDialogs Dialogs { get; }
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

    [ObservableProperty]
    private MTGManaProductionPieChart manaProductionChart;
    [ObservableProperty]
    private MTGSpellTypePieChart spellTypeChart;
    [ObservableProperty]
    private MTGCMCStackedColumnChart cMCChart;
    [ObservableProperty]
    private MTGColorPieChart colorChart;
    [ObservableProperty]
    private bool isBusy;

    public Cardlist DeckCards { get; set; }
    public Cardlist WishlistCards { get; set; }
    public Cardlist MaybelistCards { get; set; }

    public MTGSortProperty SelectedPrimarySortProperty
    {
      set
      {
        DeckCards.PrimarySortProperty = value;
        WishlistCards.PrimarySortProperty = value;
        MaybelistCards.PrimarySortProperty = value;
      }
    }
    public MTGSortProperty SelectedSecondarySortProperty
    {
      set
      {
        DeckCards.SecondarySortProperty = value;
        WishlistCards.SecondarySortProperty = value;
        MaybelistCards.SecondarySortProperty = value;
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
        var loadName = await Dialogs.GetLoadDialog((await DeckRepository.Get()).Select(x => x.Name).ToArray()).ShowAsync(App.MainRoot);
        if (loadName != null)
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
      var saveName = await Dialogs.GetSaveDialog(CardDeck.Name).ShowAsync(App.MainRoot);
      if (string.IsNullOrEmpty(saveName)) { return; }
      else
      {
        if (saveName != CardDeck.Name && await DeckRepository.Exists(saveName))
        {
          // Deck exists already
          if (await Dialogs.GetOverrideDialog(saveName).ShowAsync(App.MainRoot) == null) { return; }
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
      if (await Dialogs.GetDeleteDialog(CardDeck.Name).ShowAsync(App.MainRoot) is true)
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
    /// Sets <see cref="SelectedPrimarySortProperty"/> to the given property
    /// </summary>
    [RelayCommand]
    public void SetPrimarySortProperty(string prop)
    {
      if (Enum.TryParse(prop, true, out MTGSortProperty sortProperty))
      {
        SelectedPrimarySortProperty = sortProperty;
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
        SelectedSecondarySortProperty = sortProperty;
      }
    }

    private bool CanExecuteSaveDeckDialogCommand() => DeckCards.CardlistSize > 0;
    private bool CanExecuteDeleteDeckDialogCommand() => !string.IsNullOrEmpty(CardDeck.Name);

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
      CMCChart = new MTGCMCStackedColumnChart() { Models = CardDeck.DeckCards };
      SpellTypeChart = new MTGSpellTypePieChart() { Models = CardDeck.DeckCards };
      ManaProductionChart = new MTGManaProductionPieChart() { Models = CardDeck.DeckCards };
      ColorChart = new MTGColorPieChart(innerRadius: 60) { Models = CardDeck.DeckCards };
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
        var wantSaveConfirmed = await Dialogs.GetSaveUnsavedDialog().ShowAsync(App.MainRoot);
        if (wantSaveConfirmed == null) { return false; }
        else if (wantSaveConfirmed is true)
        {
          // User wants to save the unsaved changes
          var saveName = await Dialogs.GetSaveDialog(CardDeck.Name).ShowAsync(App.MainRoot);
          if (string.IsNullOrEmpty(saveName)) { return false; }
          else
          {
            if (saveName != CardDeck.Name && await DeckRepository.Exists(saveName))
            {
              // Deck exists already
              var overrideConfirmed = await Dialogs.GetOverrideDialog(saveName).ShowAsync(App.MainRoot);
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

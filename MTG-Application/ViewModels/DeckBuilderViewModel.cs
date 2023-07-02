﻿using CommunityToolkit.Mvvm.ComponentModel;
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

  public partial class Cardlist : ObservableObject
  {
    public partial class CardFilters : ObservableObject
    {
      public enum ColorGroups { All, Mono, Multi }

      [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
      private string nameText = string.Empty;
      [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
      private string typeText = string.Empty;
      [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
      private string oracleText = string.Empty;
      [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
      private bool white = true;
      [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
      private bool blue = true;
      [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
      private bool black = true;
      [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
      private bool red = true;
      [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
      private bool green = true;
      [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
      private bool colorless = true;
      [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
      private ColorGroups colorGroup = ColorGroups.All; // All, Mono, Multi
      [ObservableProperty, NotifyPropertyChangedFor(nameof(FiltersApplied))]
      private double cmc = double.NaN;

      /// <summary>
      /// Returns <see langword="true"/> if any of the filter properties has been changed from the default value
      /// </summary>
      public bool FiltersApplied => !string.IsNullOrEmpty(NameText) || !string.IsNullOrEmpty(TypeText) || !string.IsNullOrEmpty(OracleText)
        || !White || !Blue || !Black || !Red || !Green || !Colorless || ColorGroup != ColorGroups.All || !double.IsNaN(Cmc);

      /// <summary>
      /// returns <see langword="true"/> if the given <paramref name="card"/> is valid with the selected filters
      /// </summary>
      public bool CardValidation(MTGCardViewModel cardViewModel)
      {
        if (cardViewModel.Name.Contains(NameText, StringComparison.OrdinalIgnoreCase)
          && cardViewModel.TypeLine.Contains(TypeText, StringComparison.OrdinalIgnoreCase)
          && (cardViewModel.Model.Info.FrontFace.OracleText.Contains(OracleText, StringComparison.OrdinalIgnoreCase)
          || (cardViewModel.Model.Info.BackFace != null && cardViewModel.Model.Info.BackFace.Value.OracleText.Contains(OracleText, StringComparison.OrdinalIgnoreCase)))
          && (White || !cardViewModel.Colors.Contains(MTGCard.ColorTypes.W))
          && (Blue || !cardViewModel.Colors.Contains(MTGCard.ColorTypes.U))
          && (Black || !cardViewModel.Colors.Contains(MTGCard.ColorTypes.B))
          && (Red || !cardViewModel.Colors.Contains(MTGCard.ColorTypes.R))
          && (Green || !cardViewModel.Colors.Contains(MTGCard.ColorTypes.G))
          && (Colorless || !cardViewModel.Colors.Contains(MTGCard.ColorTypes.C))
          && (ColorGroup == ColorGroups.All
            || ColorGroup == ColorGroups.Mono && cardViewModel.Colors.Length == 1
            || (ColorGroup == ColorGroups.Multi && cardViewModel.Colors.Length > 1))
          && (double.IsNaN(Cmc) || cardViewModel.CMC == Cmc))
        { return true; }
        else
        { return false; };
      }

      /// <summary>
      /// Reset filter properties to the default values
      /// </summary>
      [RelayCommand]
      public void Reset()
      {
        NameText = string.Empty;
        TypeText = string.Empty;
        OracleText = string.Empty;
        White = true;
        Blue = true;
        Black = true;
        Red = true;
        Green = true;
        Colorless = true;
        ColorGroup = ColorGroups.All;
        Cmc = double.NaN;
      }

      /// <summary>
      /// Changes <see cref="ColorGroup"/> to the given <paramref name="group"/>
      /// </summary>
      [RelayCommand]
      public void ChangeColorGroup(string group)
      {
        if (Enum.TryParse(group, out ColorGroups colorGroup))
        {
          ColorGroup = colorGroup;
        }
      }
    }

    public Cardlist(MTGCardDeck deck, CardlistType listType, DeckBuilderViewDialogs dialogs, ICardAPI<MTGCard> cardAPI, IOService.ClipboardService clipboardService = default, CardFilters filters = default, CommandService commandService = default, string name = "")
    {
      Name = name;
      CardViewModels = new();
      FilteredAndSortedCardViewModels = new(CardViewModels, true);
      FilteredAndSortedCardViewModels.SortDescriptions.Add(new SortDescription(MTGCardViewModel.GetPropertyName(primarySortProperty), sortDirection));
      FilteredAndSortedCardViewModels.SortDescriptions.Add(new SortDescription(MTGCardViewModel.GetPropertyName(secondarySortProperty), sortDirection));
      Filters = filters ?? new();

      CardDeck = deck;
      ListType = listType;
      Dialogs = dialogs;
      CardAPI = cardAPI;
      ClipboardService = clipboardService ?? new();

      Filters.PropertyChanged += Filters_PropertyChanged;
      PropertyChanged += Cardlist_PropertyChanged;
      CardViewModels.CollectionChanged += CardViewModels_CollectionChanged;
      OnPropertyChanged(nameof(CardDeck));
      CommandService = commandService ?? new();
    }

    private void CardViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnPropertyChanged(nameof(CardlistSize));

    private void Filters_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => FilterViewModels();

    private void CardViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case nameof(MTGCardViewModel.Count):
          OnPropertyChanged(nameof(CardlistSize));
          HasUnsavedChanges = true;
          break;
        case nameof(MTGCardViewModel.Price):
          OnPropertyChanged(nameof(EuroPrice));
          HasUnsavedChanges = true;
          break;
        default:
          break;
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
          HasUnsavedChanges = false;
          break;
        case nameof(SortDirection):
        case nameof(PrimarySortProperty):
        case nameof(SecondarySortProperty):
          FilteredAndSortedCardViewModels.SortDescriptions[0] = new SortDescription(MTGCardViewModel.GetPropertyName(PrimarySortProperty), SortDirection);
          FilteredAndSortedCardViewModels.SortDescriptions[1] = new SortDescription(MTGCardViewModel.GetPropertyName(SecondarySortProperty), SortDirection);
          break;
        case nameof(CardlistSize):
          OnPropertyChanged(nameof(EuroPrice));
          break;
        default:
          break;
      }
    }

    private void CardModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      // Sync models and viewmodels
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          AddCardViewModels(e.NewItems[0] as MTGCard);
          break;
        case NotifyCollectionChangedAction.Remove:
          RemoveCardViewModel(e.OldItems[0] as MTGCard);
          break;
        case NotifyCollectionChangedAction.Reset:
          CardViewModels.Clear();
          break;
        default:
          break;
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
    private IOService.ClipboardService ClipboardService { get; }
    private DeckBuilderViewDialogs Dialogs { get; }
    private ICardAPI<MTGCard> CardAPI { get; }
    private CardFilters Filters { get; set; }

    public string Name { get; }
    public CardlistType ListType { get; }
    public CommandService CommandService { get; }
    public AdvancedCollectionView FilteredAndSortedCardViewModels { get; }
    /// <summary>
    /// Returns total card count of the cardlist cards
    /// </summary>
    public int CardlistSize => CardViewModels.Sum(x => x.Model.Count);
    /// <summary>
    /// Returns total euro price of the cardlist cards
    /// </summary>
    public float EuroPrice => (float)Math.Round(CardViewModels.Sum(x => x.Model.Info.Price * x.Model.Count), 0);
    /// <summary>
    /// Returns the deck's <see cref="MTGCard"/> collection that this cardlist represents
    /// </summary>
    public ObservableCollection<MTGCard> CardCollection => CardDeck.GetCardlist(ListType);

    #region Relay Commands
    /// <summary>
    /// Asks cards to import and imports them to the deck
    /// </summary>
    [RelayCommand]
    public async Task ImportToCardlistDialog()
    {
      if (await Dialogs.GetImportDialog().ShowAsync() is string importText)
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
      var response = await Dialogs.GetExportDialog(GetExportString(exportProperty)).ShowAsync();

      if (!string.IsNullOrEmpty(response))
      {
        ClipboardService.Copy(response);
      }
    }

    /// <summary>
    /// Removes given card from the deck cardlist
    /// </summary>
    [RelayCommand]
    public void RemoveFromCardlist(MTGCard card) => CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.RemoveCardsFromCardlistCommand(CardDeck, ListType, new[] { card }));

    /// <summary>
    /// Shows a dialog with cards prints and changes the cards print to the selected print
    /// </summary>
    [RelayCommand]
    public async Task ChangePrintDialog(MTGCard card)
    {
      // Get prints
      IsBusy = true;

      var pageUri = card.Info.PrintSearchUri;
      var prints = new List<MTGCard>();
      while (pageUri != string.Empty)
      {
        var result = await CardAPI.FetchFromUri(pageUri, paperOnly: true);
        prints.AddRange(result.Found);
        pageUri = result.NextPageUri;
      }

      var printViewModels = prints.Select(x => new MTGCardViewModel(x)).ToArray();
      IsBusy = false;

      if (await Dialogs.GetCardPrintDialog(printViewModels).ShowAsync() is MTGCardViewModel newPrint)
      {
        // Replace card
        var cardlist = CardDeck.GetCardlist(ListType);
        var index = cardlist.IndexOf(card);
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
          if (CardDeck.GetCardlist(ListType).FirstOrDefault(x => x.Info.Name == card.Info.Name) is not null)
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
        CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.AddCardsToCardlistCommand(CardDeck, ListType, importCards.ToArray()));
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
    /// Removes all items in the card list
    /// </summary>
    [RelayCommand]
    public void Clear() => CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.RemoveCardsFromCardlistCommand(CardDeck, ListType, CardDeck.GetCardlist(ListType).ToArray()));
    #endregion

    /// <summary>
    /// Adds given card to the deck cardlist
    /// </summary>
    public async Task AddToCardlist(MTGCard card)
    {
      if (CardCollection.FirstOrDefault(x => x.Info.Name == card.Info.Name) is not null)
      {
        if ((await Dialogs.GetCardAlreadyInCardlistDialog(card.Info.Name, Name).ShowAsync()) is true)
        {
          CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.AddCardsToCardlistCommand(CardDeck, ListType, new[] { card }));
        }
      }
      else { CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.AddCardsToCardlistCommand(CardDeck, ListType, new[] { card })); }
    }

    /// <summary>
    /// Moves the given card from the origin list to this list
    /// </summary>
    public async Task MoveToCardlist(MTGCard card, Cardlist origin)
    {
      if (CardCollection.FirstOrDefault(x => x.Info.Name == card.Info.Name) is not null)
      {
        if ((await Dialogs.GetCardAlreadyInCardlistDialog(card.Info.Name, Name).ShowAsync()) is true)
        {
          CommandService.Execute(new CombinedCommand(new ICommand[]
          {
            new MTGCardDeck.MTGCardDeckCommands.AddCardsToCardlistCommand(CardDeck, ListType, new[]{card}),
            new MTGCardDeck.MTGCardDeckCommands.RemoveCardsFromCardlistCommand(origin.CardDeck, origin.ListType, new[]{card})
          }));
        }
      }
      else
      {
        CommandService.Execute(new CombinedCommand(new ICommand[]
          {
            new MTGCardDeck.MTGCardDeckCommands.AddCardsToCardlistCommand(CardDeck, ListType, new[]{card}),
            new MTGCardDeck.MTGCardDeckCommands.RemoveCardsFromCardlistCommand(origin.CardDeck, origin.ListType, new[]{card})
          }));
      }
    }

    /// <summary>
    /// Adds given <paramref name="cards"/> to <see cref="CardViewModels"/> list as a <see cref="MTGCardViewModel"/>
    /// </summary>
    private void AddCardViewModels(MTGCard[] cards)
    {
      foreach (var card in cards)
      {
        var vm = new MTGCardViewModel(card) { DeleteCardCommand = RemoveFromCardlistCommand, ShowPrintsDialogCommand = ChangePrintDialogCommand };
        vm.PropertyChanged += CardViewModel_PropertyChanged;
        CardViewModels.Add(vm);
      }
    }

    /// <summary>
    /// Adds given <paramref name="card"/> to <see cref="CardViewModels"/> list as a <see cref="MTGCardViewModel"/>
    /// </summary>
    private void AddCardViewModels(MTGCard card)
    {
      var vm = new MTGCardViewModel(card) { DeleteCardCommand = RemoveFromCardlistCommand, ShowPrintsDialogCommand = ChangePrintDialogCommand };
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
      else
      { FilteredAndSortedCardViewModels.Filter = null; }
    }

    /// <summary>
    /// Returns exportable string of the lists cards using the <paramref name="exportProperty"/>
    /// </summary>
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

  public DeckBuilderViewModel(ICardAPI<MTGCard> cardAPI, IRepository<MTGCardDeck> deckRepository, IOService.ClipboardService clipboardService = null, DeckBuilderViewDialogs dialogs = null)
  {
    DeckRepository = deckRepository;
    CardAPI = cardAPI;
    Dialogs = dialogs ?? new();

    PropertyChanged += DeckBuilderViewModel_PropertyChanged;
    CardDeck.PropertyChanged += CardDeck_PropertyChanged;

    clipboardService ??= new();
    DeckCards = new Cardlist(CardDeck, CardlistType.Deck, Dialogs, CardAPI, clipboardService, CardFilters, CommandService, "Deck");
    WishlistCards = new Cardlist(CardDeck, CardlistType.Wishlist, Dialogs, CardAPI, clipboardService, CardFilters, CommandService, "Wishlist");
    MaybelistCards = new Cardlist(CardDeck, CardlistType.Maybelist, Dialogs, CardAPI, clipboardService, CardFilters, CommandService, "Maybelist");
    RemovelistCards = new Cardlist(CardDeck, CardlistType.Removelist, Dialogs, CardAPI, clipboardService, CardFilters, CommandService, "Removelist");

    DeckCards.PropertyChanged += Cardlist_PropertyChanged;
    DeckCards.PropertyChanged += DeckCards_PropertyChanged;
    WishlistCards.PropertyChanged += Cardlist_PropertyChanged;
    MaybelistCards.PropertyChanged += Cardlist_PropertyChanged;
    RemovelistCards.PropertyChanged += Cardlist_PropertyChanged;

    SelectedSortDirection = SortDirection.Ascending;
    SelectedPrimarySortProperty = MTGSortProperty.CMC;
    UpdateCharts();
  }

  private void DeckCards_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(Cardlist.CardlistSize))
    {
      OnPropertyChanged(nameof(DeckSize));
    }
  }

  private void CardDeck_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(CardDeck.Commander):
        Commander = CardDeck?.Commander != null ? new(CardDeck.Commander) : null;
        HasUnsavedChanges = true;
        OnPropertyChanged(nameof(DeckSize));
        break;
      case nameof(CardDeck.CommanderPartner):
        CommanderPartner = CardDeck?.CommanderPartner != null ? new(CardDeck.CommanderPartner) : null;
        HasUnsavedChanges = true;
        OnPropertyChanged(nameof(DeckSize));
        break;
    }
  }

  private void Cardlist_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(Cardlist.IsBusy):
        IsBusy = DeckCards.IsBusy || WishlistCards.IsBusy || MaybelistCards.IsBusy || RemovelistCards.IsBusy; break;
      case nameof(HasUnsavedChanges): OnPropertyChanged(nameof(HasUnsavedChanges)); break;
    }
  }

  private void DeckBuilderViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(CardDeck))
    {
      CardDeck.PropertyChanged += CardDeck_PropertyChanged;
      DeckCards.CardDeck = CardDeck;
      WishlistCards.CardDeck = CardDeck;
      MaybelistCards.CardDeck = CardDeck;
      RemovelistCards.CardDeck = CardDeck;
      Commander = CardDeck?.Commander != null ? new(CardDeck.Commander) : null;
      CommanderPartner = CardDeck?.CommanderPartner != null ? new(CardDeck.CommanderPartner) : null;
      CommandService.Clear();
      UpdateCharts();
    }
  }

  private IRepository<MTGCardDeck> DeckRepository { get; }
  private DeckBuilderViewDialogs Dialogs { get; }
  private ICardAPI<MTGCard> CardAPI { get; }

  [ObservableProperty, NotifyCanExecuteChangedFor(nameof(OpenPlaytestWindowCommand))]
  private MTGCardDeck cardDeck = new();
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
  [ObservableProperty]
  private MTGCardViewModel commander;
  [ObservableProperty]
  private MTGCardViewModel commanderPartner;

  public Cardlist DeckCards { get; }
  public Cardlist WishlistCards { get; }
  public Cardlist MaybelistCards { get; }
  public Cardlist RemovelistCards { get; }
  public Cardlist.CardFilters CardFilters { get; } = new();
  public CommandService CommandService { get; } = new();
  public MTGSortProperty SelectedPrimarySortProperty
  {
    set
    {
      DeckCards.PrimarySortProperty = value;
      WishlistCards.PrimarySortProperty = value;
      MaybelistCards.PrimarySortProperty = value;
      RemovelistCards.PrimarySortProperty = value;
    }
  }
  public MTGSortProperty SelectedSecondarySortProperty
  {
    set
    {
      DeckCards.SecondarySortProperty = value;
      WishlistCards.SecondarySortProperty = value;
      MaybelistCards.SecondarySortProperty = value;
      RemovelistCards.SecondarySortProperty = value;
    }
  }
  public SortDirection SelectedSortDirection
  {
    set
    {
      DeckCards.SortDirection = value;
      WishlistCards.SortDirection = value;
      MaybelistCards.SortDirection = value;
      RemovelistCards.SortDirection = value;
    }
  }
  public int DeckSize => DeckCards.CardlistSize + (CardDeck.Commander != null ? 1 : 0) + (CardDeck.CommanderPartner != null ? 1 : 0);

  #region ISavable implementation
  public bool HasUnsavedChanges
  {
    get => DeckCards.HasUnsavedChanges || WishlistCards.HasUnsavedChanges || MaybelistCards.HasUnsavedChanges || RemovelistCards.HasUnsavedChanges;
    set
    {
      DeckCards.HasUnsavedChanges = value;
      WishlistCards.HasUnsavedChanges = value;
      MaybelistCards.HasUnsavedChanges = value;
      RemovelistCards.HasUnsavedChanges = value;
    }
  }

  public async Task<bool> SaveUnsavedChanges() => await ShowUnsavedDialogs();
  #endregion

  #region Relay Commands
  /// <summary>
  /// Shows UnsavedChanges dialog and clear current card deck
  /// </summary>
  [RelayCommand]
  public async Task NewDeckDialog()
  {
    if (await ShowUnsavedDialogs())
      await NewDeck();
  }

  /// <summary>
  /// Shows dialog with names of the saved decks and changes current deck to the selected deck from the database
  /// </summary>
  [RelayCommand]
  public async Task LoadDeckDialog()
  {
    if (await ShowUnsavedDialogs())
    {
      var loadName = await Dialogs.GetLoadDialog((await DeckRepository.Get()).Select(x => x.Name).OrderBy(x => x).ToArray()).ShowAsync();
      if (loadName != null)
      {
        await LoadDeck(loadName);
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
    if (string.IsNullOrEmpty(saveName))
    { return; }
    else
    {
      if (saveName != CardDeck.Name && await DeckRepository.Exists(saveName))
      {
        // Deck with the given name exists already
        if (await Dialogs.GetOverrideDialog(saveName).ShowAsync() == null)
        { return; }
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
    if (!await DeckRepository.Exists(CardDeck.Name))
    { return; }
    if (await Dialogs.GetDeleteDialog(CardDeck.Name).ShowAsync() is true)
    {
      await DeleteDeck();
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

  /// <summary>
  /// Shows deck cards' tokens in a dialog
  /// </summary>
  [RelayCommand]
  public async void TokensDialog()
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
  [RelayCommand(CanExecute = nameof(DeckIsLoaded))]
  public void OpenPlaytestWindow(MTGCardDeck deck)
  {
    if (string.IsNullOrEmpty(deck.Name)) { return; }

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
  #endregion

  /// <summary>
  /// Sets deck's commander to the given card and removes the card from the origin
  /// </summary>
  public void MoveToCommander(MTGCard card, Cardlist origin)
  {
    CommandService.Execute(new CombinedCommand(new ICommand[]
    {
      new MTGCardDeck.MTGCardDeckCommands.SetCommanderCommand(CardDeck, card),
      new MTGCardDeck.MTGCardDeckCommands.RemoveCardsFromCardlistCommand(CardDeck, origin.ListType, new[] { card }),
    }));
  }

  /// <summary>
  /// Sets deck's commander partner to the given card and removes the card from the origin
  /// </summary>
  public void MoveToCommanderPartner(MTGCard card, Cardlist origin)
  {
    CommandService.Execute(new CombinedCommand(new ICommand[]
    {
      new MTGCardDeck.MTGCardDeckCommands.SetCommanderPartnerCommand(CardDeck, card),
      new MTGCardDeck.MTGCardDeckCommands.RemoveCardsFromCardlistCommand(CardDeck, origin.ListType, new[] { card }),
    }));
  }

  /// <summary>
  /// Asks user if they want to save the deck's unsaved changes.
  /// </summary>
  /// <returns><see langword="true"/>, if user does not cancel any of the dialogs</returns>
  private async Task<bool> ShowUnsavedDialogs()
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

  /// <summary>
  /// Changes current deck to a new deck
  /// </summary>
  private async Task NewDeck()
  {
    IsBusy = true;
    CardDeck = await Task.Run(() => new MTGCardDeck());
    IsBusy = false;
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
  /// Loads a deck with the given <paramref name="name"/> and changes current deck to the loaded deck
  /// </summary>
  private async Task LoadDeck(string name)
  {
    IsBusy = true;
    if (await Task.Run(() => DeckRepository.Get(name)) is MTGCardDeck loadedDeck)
    {
      CardDeck = loadedDeck;
      NotificationService.RaiseNotification(NotificationService.NotificationType.Success, "The deck was loaded successfully.");
    }
    else
    { NotificationService.RaiseNotification(NotificationService.NotificationType.Error, "Error. Could not load the deck."); }
    IsBusy = false;
  }

  /// <summary>
  /// Deletes the current deck
  /// </summary>
  private async Task DeleteDeck()
  {
    IsBusy = true;
    if (await Task.Run(() => DeckRepository.Remove(CardDeck)))
    {
      CardDeck = new();
      NotificationService.RaiseNotification(NotificationService.NotificationType.Success, "The deck was deleted successfully.");
    }
    else
    { NotificationService.RaiseNotification(NotificationService.NotificationType.Error, "Error. Could not delete the deck."); }
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

  /// <summary>
  /// Returns true if deck has been loaded from a database
  /// </summary>
  private bool DeckIsLoaded() => !string.IsNullOrEmpty(CardDeck.Name);
}

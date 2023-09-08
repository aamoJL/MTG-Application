using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Services.CommandService;
using static MTGApplication.Services.MTGService;
using static MTGApplication.ViewModels.DeckBuilderViewModel;

namespace MTGApplication.ViewModels;

public partial class DeckCardlistViewModel : ObservableObject, IInAppNotifier
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
        SavableChangesOccurred?.Invoke();
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
    SavableChangesOccurred?.Invoke();
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
  public Action SavableChangesOccurred { get; set; }
  public string Name { get; init; }

  /// <summary>
  /// Returns total card count of the cardlist cards
  /// </summary>
  public int CardlistSize => CardViewModels.Sum(x => x.Model.Count);
  /// <summary>
  /// Returns total euro price of the cardlist cards
  /// </summary>
  public float EuroPrice => CardViewModels.Sum(x => x.Model.Info.Price * x.Model.Count);

  #region IInAppNotifier implementation
  
  public event EventHandler<NotificationService.NotificationEventArgs> OnNotification;

  public void RaiseInAppNotification(NotificationService.NotificationType type, string text) => OnNotification?.Invoke(this, new(type, text));

  #endregion

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
      RaiseInAppNotification(NotificationService.NotificationType.Info, "Copied to clipboard.");
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

          if (import is true) { importCards.Add(card); }
        }
        else { importCards.Add(card); }
      }
    }

    if (importCards.Count != 0)
    {
      CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.AddCardsToCardlistCommand(Cardlist, importCards.ToArray()));
    }

    if (notFoundCount == 0 && found.Length > 0)
      RaiseInAppNotification(NotificationService.NotificationType.Success, $"{importCards.Count} cards imported successfully." + ((found.Length - importCards.Count) > 0 ? $" ({(found.Length - importCards.Count)} cards skipped) " : ""));
    else if (found.Length > 0 && notFoundCount > 0)
      RaiseInAppNotification(NotificationService.NotificationType.Warning,
        $"{found.Length} / {notFoundCount + found.Length} cards imported successfully.{Environment.NewLine}{notFoundCount} cards were not found." + ((found.Length - importCards.Count) > 0 ? $" ({(found.Length - importCards.Count)} cards skipped) " : ""));
    else if (found.Length == 0)
      RaiseInAppNotification(NotificationService.NotificationType.Error, $"Error. No cards were imported.");

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
        CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.AddCardsToCardlistCommand(Cardlist, new[] { card }));
      }
    }
    else { CommandService.Execute(new MTGCardDeck.MTGCardDeckCommands.AddCardsToCardlistCommand(Cardlist, new[] { card })); }
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

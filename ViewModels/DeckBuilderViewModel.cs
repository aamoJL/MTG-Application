using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Charts;
using MTGApplication.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MTGApplication.Models.MTGCardDeck;

namespace MTGApplication.ViewModels
{
  public partial class DeckBuilderViewModel : ViewModelBase
  {
    public partial class MTGSearch : ObservableObject
    {
      public ObservableCollection<MTGCardViewModel> SearchCards { get; set; } = new();

      public string SearchQuery { get; set; }
      [ObservableProperty]
      private bool isBusy;

      /// <summary>
      /// Fetches cards from API using selected query, and replaces current cards with the fetched cards
      /// </summary>
      public async Task Search()
      {
        IsBusy = true;
        SearchCards.Clear();
        if (!string.IsNullOrEmpty(SearchQuery))
        {
          var cards = await Task.Run(() => App.CardAPI.FetchCards(SearchQuery));
          foreach (var item in cards)
          {
            SearchCards.Add(new(item));
          }
        }
        IsBusy = false;
      }
    }
    public partial class MTGDeckBuilder : ObservableObject
    {
      public MTGDeckBuilder()
      {
        PropertyChanged += MTGDeckBuilder_PropertyChanged;
        CardDeck.DeckCards.CollectionChanged += Cardlist_CollectionChanged;
        CardDeck.Wishlist.CollectionChanged += Wishlist_CollectionChanged;
        CardDeck.Maybelist.CollectionChanged += Maybelist_CollectionChanged;
        CardDeck.PropertyChanged += CardDeck_PropertyChanged;
        DeckCardsViewModels.CollectionChanged += DecklistViewModels_CollectionChanged;
        WishlistViewModels.CollectionChanged += WishlistViewModels_CollectionChanged;
        MaybelistViewModels.CollectionChanged += MaybelistViewModels_CollectionChanged;
      }

      private async void MaybelistViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            //await SortCardViewModels(MaybelistViewModels, SelectedSortProperty, SelectedSortDirection); break;
          default: break;
        }
      }
      private async void WishlistViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            //await SortCardViewModels(WishlistViewModels, SelectedSortProperty, SelectedSortDirection); break;
          default: break;
        }
      }
      private async void DecklistViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            //await SortCardViewModels(DeckCardsViewModels, SelectedSortProperty, SelectedSortDirection); break;
          default: break;
        }
      }
      private void CardDeck_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
        if (e.PropertyName == nameof(MTGCardDeck.DeckSize)) { HasUnsavedChanges = true; }
        if (e.PropertyName == nameof(MTGCardDeck.WishlistSize)) { HasUnsavedChanges = true; }
        if (e.PropertyName == nameof(MTGCardDeck.MaybelistSize)) { HasUnsavedChanges = true; }
      }
      private void MTGDeckBuilder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
        if (e.PropertyName == nameof(CardDeck))
        {
          CardDeck.DeckCards.CollectionChanged += Cardlist_CollectionChanged;
          CardDeck.Wishlist.CollectionChanged += Wishlist_CollectionChanged;
          CardDeck.Maybelist.CollectionChanged += Maybelist_CollectionChanged;
          CardDeck.PropertyChanged += CardDeck_PropertyChanged;

          DeckCardsViewModels.Clear();
          foreach (var item in CardDeck.DeckCards)
          {
            DeckCardsViewModels.Add(new(item));
          }

          WishlistViewModels.Clear();
          foreach (var item in CardDeck.Wishlist)
          {
            WishlistViewModels.Add(new(item));
          }

          MaybelistViewModels.Clear();
          foreach (var item in CardDeck.Maybelist)
          {
            MaybelistViewModels.Add(new(item));
          }
        }
      }
      private void Maybelist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        // Sync Maybelist and MaybelistViewModels
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            MaybelistViewModels.Add(new(e.NewItems[0] as MTGCard));
            break;
          case NotifyCollectionChangedAction.Remove:
            MaybelistViewModels.Remove(MaybelistViewModels.FirstOrDefault(x => x.Model == e.OldItems[0])); break;
          case NotifyCollectionChangedAction.Reset:
            MaybelistViewModels.Clear();
            break;
          case NotifyCollectionChangedAction.Move:
            MaybelistViewModels.Move(e.OldStartingIndex, e.NewStartingIndex);
            break;
          default:
            break;
        }
        HasUnsavedChanges = true;
      }
      private void Wishlist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        // Sync Wishlist and WishlistViewModels
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            WishlistViewModels.Add(new(e.NewItems[0] as MTGCard));
            break;
          case NotifyCollectionChangedAction.Remove:
            WishlistViewModels.Remove(WishlistViewModels.FirstOrDefault(x => x.Model == e.OldItems[0])); break;
          case NotifyCollectionChangedAction.Reset:
            WishlistViewModels.Clear();
            break;
          case NotifyCollectionChangedAction.Move:
            WishlistViewModels.Move(e.OldStartingIndex, e.NewStartingIndex);
            break;
          default:
            break;
        }
        HasUnsavedChanges = true;
      }
      private void Cardlist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        // Sync cardlist and cardlistviewmodels
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            DeckCardsViewModels.Add(new(e.NewItems[0] as MTGCard));
            break;
          case NotifyCollectionChangedAction.Remove:
            DeckCardsViewModels.Remove(DeckCardsViewModels.FirstOrDefault(x => x.Model == e.OldItems[0])); break;
          case NotifyCollectionChangedAction.Reset:
            DeckCardsViewModels.Clear();
            break;
          case NotifyCollectionChangedAction.Move:
            DeckCardsViewModels.Move(e.OldStartingIndex, e.NewStartingIndex);
            break;
          default:
            break;
        }
        HasUnsavedChanges = true;
      }

      [ObservableProperty]
      private MTGCardDeck cardDeck = new();
      [ObservableProperty]
      private bool isBusy;
      
      public bool HasUnsavedChanges { get; set; }
      public SortProperty SelectedSortProperty { get; set; } = SortProperty.CMC;
      public SortDirection SelectedSortDirection { get; set; } = SortDirection.ASC;
      public ObservableCollection<MTGCardViewModel> DeckCardsViewModels { get; set; } = new();
      public ObservableCollection<MTGCardViewModel> WishlistViewModels { get; set; } = new();
      public ObservableCollection<MTGCardViewModel> MaybelistViewModels { get; set; } = new();

      /// <summary>
      /// Clears current card deck
      /// </summary>
      public void NewDeck()
      {
        IsBusy = true;
        CardDeck = new();
        HasUnsavedChanges = false;
        IsBusy = false;
      }
      
      /// <summary>
      /// Saves current card deck to the database
      /// </summary>
      /// <param name="saveName">Deck name</param>
      public async Task SaveDeck(string saveName)
      {
        IsBusy = true;
        if(await MTGCardDeck.SaveDeck(CardDeck, saveName) is MTGCardDeck savedDeck)
        {
          CardDeck = savedDeck;
          HasUnsavedChanges = false;
        }
        else
        {
          // TODO: notification
        }
        IsBusy = false;
      }
      
      /// <summary>
      /// Replaces current card deck with saved deck from the database
      /// </summary>
      /// <param name="loadName"></param>
      public async Task LoadDeck(string loadName)
      {
        IsBusy = true;
        var newDeck = await Task.Run(() => MTGCardDeck.LoadDeck(loadName));
        if(newDeck != null)
        {
          CardDeck = newDeck;
          HasUnsavedChanges = false;
        }
        else
        {
          // TODO: notification
        }
        IsBusy = false;
      }

      /// <summary>
      /// Deletes current card deck from the database
      /// </summary>
      public async Task DeleteDeck()
      {
        IsBusy = true;
        if(await Task.Run(() => MTGCardDeck.DeleteDeck(CardDeck)))
        {
          NewDeck();
        }
        else
        {
          // TODO: notification
        }
        IsBusy = false;
      }
      
      /// <summary>
      /// Sorts current card viewmodel lists using selected property and direction
      /// </summary>
      public async Task SortDeck()
      {
        IsBusy = true;
        await SortCardViewModels(SelectedSortProperty, SelectedSortDirection);
        IsBusy = false;
      }

      /// <summary>
      /// Adds cards to cardlist from the given text using card API
      /// </summary>
      public async Task ImportCards(ObservableCollection<MTGCard> cardlist, string importText)
      {
        IsBusy = true;
        if (!string.IsNullOrEmpty(importText))
        {
          var cards = await App.CardAPI.FetchImportedCards(importText);
          foreach (var item in cards)
          {
            cardlist.Add(item);
          }
        }
        IsBusy = false;
      }

      /// <summary>
      /// Sorts MTGCArdViewModel collections by given <paramref name="prop"/> and <paramref name="dir"/>
      /// </summary>
      public async Task SortCardViewModels(SortProperty prop, SortDirection dir)
      {
        List<Task> tasks = new()
        {
          SortCardViewModels(DeckCardsViewModels, prop, dir),
          SortCardViewModels(WishlistViewModels, prop, dir),
          SortCardViewModels(MaybelistViewModels, prop, dir),
        };

        await Task.WhenAll(tasks);
      }
      private static async Task SortCardViewModels(ObservableCollection<MTGCardViewModel> viewModels, SortProperty prop, SortDirection dir)
      {
        List<MTGCardViewModel> tempList = new();
        tempList = await Task.Run(() => prop switch
        {
          SortProperty.CMC => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Info.CMC).ToList() : viewModels.OrderByDescending(x => x.Model.Info.CMC).ToList(),
          SortProperty.Name => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Info.Name).ToList() : viewModels.OrderByDescending(x => x.Model.Info.Name).ToList(),
          SortProperty.Rarity => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Info.RarityType).ToList() : viewModels.OrderByDescending(x => x.Model.Info.RarityType).ToList(),
          SortProperty.Color => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Info.ColorType).ToList() : viewModels.OrderByDescending(x => x.Model.Info.ColorType).ToList(),
          SortProperty.Set => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Info.SetName).ToList() : viewModels.OrderByDescending(x => x.Model.Info.SetName).ToList(),
          SortProperty.Count => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Count).ToList() : viewModels.OrderByDescending(x => x.Model.Count).ToList(),
          SortProperty.Price => dir == SortDirection.ASC ? viewModels.OrderBy(x => x.Model.Info.Price).ToList() : viewModels.OrderByDescending(x => x.Model.Info.Price).ToList(),
          _ => throw new NotImplementedException(),
        });

        for (int i = 0; i < tempList.Count; i++)
        {
          viewModels.Move(viewModels.IndexOf(tempList[i]), i);
        }
      }
    }

    public DeckBuilderViewModel()
    {
      DeckBuilder.PropertyChanged += DeckBuilder_PropertyChanged;
      CMCChart = new CMCChart(DeckBuilder.CardDeck.DeckCards);
      SpellTypeChart = new SpellTypeChart(DeckBuilder.CardDeck.DeckCards);
    }

    private void DeckBuilder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if(e.PropertyName == nameof(DeckBuilder.CardDeck))
      {
        CMCChart = new CMCChart(DeckBuilder.CardDeck.DeckCards);
        SpellTypeChart = new SpellTypeChart(DeckBuilder.CardDeck.DeckCards);
        OnPropertyChanged(nameof(CMCChart));
        OnPropertyChanged(nameof(SpellTypeChart));
      }
    }

    public MTGSearch Search { get; set; } = new();
    public MTGDeckBuilder DeckBuilder { get; set; } = new();
    public CMCChart CMCChart { get; set; }
    public SpellTypeChart SpellTypeChart { get; set; }

    /// <summary>
    /// Fetches cards using API and adds them to the search cardlist
    /// </summary>
    [RelayCommand]
    public async Task SearchSubmit()
    {
      await Search.Search();
    }
    
    /// <summary>
    /// Shows UnsavedChanges dialog and clear current card deck
    /// </summary>
    [RelayCommand]
    public async Task NewDeckDialog()
    {
      if (DeckBuilder.HasUnsavedChanges)
      {
        // Deck has unsaved changes
        var wantSaveConfirmed = await GetUnsavedDialog();
        if (wantSaveConfirmed == null) { return; }
        else if (wantSaveConfirmed is true)
        {
          // User wants to save the unsaved changes
          var saveName = await GetSaveDialog(DeckBuilder.CardDeck.Name);
          if (saveName == string.Empty) { return; }
          else
          {
            if (saveName != DeckBuilder.CardDeck.Name && MTGCardDeck.Exists(saveName))
            {
              // Deck exists already
              var overrideConfirmed = await GetOverrideDialog(saveName);
              if (overrideConfirmed == null) { return; }
              else if (overrideConfirmed is true)
              {
                // User wants to override the deck
                await DeckBuilder.SaveDeck(saveName);
              }
            }
          }
        }
      }
      DeckBuilder.NewDeck();
    }
    
    /// <summary>
    /// Shows dialog with names of the saved decks and changes current deck to the selected deck from the database
    /// </summary>
    [RelayCommand]
    public async Task LoadDeckDialog()
    {
      if (DeckBuilder.HasUnsavedChanges)
      {
        // Collection has unsaved changes
        var wantSaveConfirmed = await GetUnsavedDialog();
        if (wantSaveConfirmed == null) { return; }
        else if (wantSaveConfirmed is true)
        {
          // User wants to save the unsaved changes
          var saveName = await GetSaveDialog(DeckBuilder.CardDeck.Name);
          if (saveName == string.Empty) { return; }
          else
          {
            if (saveName != DeckBuilder.CardDeck.Name && MTGCardDeck.Exists(saveName))
            {
              // Deck exists already
              var overrideConfirmed = await GetOverrideDialog(saveName);
              if (overrideConfirmed == null) { return; }
              else if (overrideConfirmed is true)
              {
                // User wants to override the deck
                await DeckBuilder.SaveDeck(saveName);
              }
            }
          }
        }
      }

      using var db = new Database.CardDatabaseContext();

      var deckNames = db.MTGCardDecks.Select(x => x.Name).ToArray();
      var loadName = await GetLoadDialog(deckNames);
      if (loadName != string.Empty)
      {
        await DeckBuilder.LoadDeck(loadName);
      }
    }
    
    /// <summary>
    /// Shows dialog that asks name for the deck and saves current deck to the database with the given name
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteSaveDeckDialogCommand))]
    public async Task SaveDeckDialog()
    {
      var saveName = await GetSaveDialog(DeckBuilder.CardDeck.Name);
      if (saveName == string.Empty) { return; }
      else
      {
        if (saveName != DeckBuilder.CardDeck.Name && Exists(saveName))
        {
          // Deck exists already
          if (await GetOverrideDialog(saveName) == null) { return; }
        }
      }

      await DeckBuilder.SaveDeck(saveName);
    }
    
    /// <summary>
    /// Deletes current deck from the database
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteDeleteDeckDialogCommand))]
    public async Task DeleteDeckDialog()
    {
      if (!MTGCardDeck.Exists(DeckBuilder.CardDeck.Name)) { return; }
      if (await GetDeleteDialog(DeckBuilder.CardDeck.Name) is true)
      {
        await DeckBuilder.DeleteDeck();
      }
    }
    
    /// <summary>
    /// Imports given cards to the given <paramref name="cardlist"/>
    /// </summary>
    [RelayCommand]
    public async Task ImportCardsDialog(ObservableCollection<MTGCard> cardlist)
    {
      var response = await GetImportDialog();
      await DeckBuilder.ImportCards(cardlist, response);
    }
    
    /// <summary>
    /// Shows dialog with formatted text of the given cardlist cards
    /// </summary>
    [RelayCommand]
    public static async Task ExportCardsDialog(ObservableCollection<MTGCard> cardlist)
    {
      StringBuilder stringBuilder = new();
      foreach (var item in cardlist)
      {
        stringBuilder.AppendLine($"{item.Count} {item.Info.Name}");
      }

      var response = await GetExportDialog(stringBuilder.ToString());
      if (!string.IsNullOrEmpty(response))
      {
        IO.CopyToClipboard(response);
      }
    }
    
    /// <summary>
    /// Sorts selected card deck by selected property using given direction
    /// </summary>
    /// <param name="dir">Sorting direction, NOT case-sensitive</param>
    [RelayCommand]
    public async Task SortByDirection(string dir)
    {
      if (Enum.TryParse(dir, true, out MTGCardDeck.SortDirection sortDirection))
      {
        DeckBuilder.SelectedSortDirection = sortDirection;
        await DeckBuilder.SortDeck();
      }
    }
    
    /// <summary>
    /// Sorts selected card deck by given property
    /// </summary>
    /// <param name="prop">Name of the property, NOT case-sensitive</param>
    [RelayCommand]
    public async Task SortByProperty(string prop)
    {
      if (Enum.TryParse(prop, true, out MTGCardDeck.SortProperty sortProperty))
      {
        DeckBuilder.SelectedSortProperty = sortProperty;
        await DeckBuilder.SortDeck();
      }
    }

    #region Dialogs
    private static async Task<bool?> GetUnsavedDialog()
    {
      return await App.MainRoot.ConfirmationDialogAsync(
          title: "Save unsaved changes?",
          content: "Would you like to save unsaved changes?",
          yesButtonText: "Save");
    }
    private static async Task<string> GetSaveDialog(string collectionName)
    {
      return await App.MainRoot.InputStringDialogAsync(
        title: "Save your deck?",
        defaultText: collectionName,
        okButtonText: "Save",
        invalidCharacters: Path.GetInvalidFileNameChars());
    }
    private static async Task<bool?> GetOverrideDialog(string saveName)
    {
      return await App.MainRoot.ConfirmationDialogAsync(
            title: "Override existing deck?",
            content: $"Deck '{saveName}' already exist. Would you like to override the deck?",
            noButtonText: string.Empty);
    }
    private static async Task<string> GetLoadDialog(string[] fileNames)
    {
      return await App.MainRoot.ComboboxDialogAsync(
        title: "Open deck",
        okButtonText: "Open",
        items: fileNames,
        header: "Name"
        );
    }
    private static async Task<bool?> GetDeleteDialog(string collectionName)
    {
      return await App.MainRoot.ConfirmationDialogAsync(
        title: "Delete deck?",
        content: $"Deleting '{collectionName}'. Are you sure?",
        noButtonText: string.Empty);
    }
    private static async Task<string> GetImportDialog()
    {
      return await App.MainRoot.TextAreaInputDialogAsync(
            title: "Import cards",
            inputPlaceholder: "Example:\n2 Black Lotus\nMox Ruby",
            okButtonText: "Add to Collection");
    }
    private static async Task<string> GetExportDialog(string text)
    {
      return await App.MainRoot.TextAreaInputDialogAsync(
            title: "Export deck",
            defaultText: text,
            okButtonText: "Copy to Clipboard");
    }
    #endregion

    #region CanExecuteCommand Methods
    private bool CanExecuteSaveDeckDialogCommand() => DeckBuilder.CardDeck.DeckSize > 0;
    private bool CanExecuteDeleteDeckDialogCommand() => DeckBuilder.CardDeck.MTGCardDeckId != 0;
    #endregion
  }
}

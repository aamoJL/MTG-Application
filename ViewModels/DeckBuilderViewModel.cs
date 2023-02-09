using CommunityToolkit.Mvvm.Input;
using MTGApplication.Charts;
using MTGApplication.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MTGApplication.Models.MTGCardDeck;

namespace MTGApplication.ViewModels
{
  public partial class DeckBuilderViewModel : ViewModelBase
  {
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

    /// 
    /// <summary> Asks cards to import and imports them to the deck
    /// 
    [RelayCommand]
    public async Task ImportToDeckCardsDialog()
    {
        var response = await GetImportDialog();
        await DeckBuilder.ImportCards(CardlistType.Deck, response);
    }
    
    /// 
    /// <summary> Asks cards to import and imports them to the deck
    /// 
    [RelayCommand]
    public async Task ImportToWishlistDialog()
    {
      var response = await GetImportDialog();
      await DeckBuilder.ImportCards(CardlistType.Wishlist, response);
    }
    
    /// 
    /// <summary> Asks cards to import and imports them to the deck
    /// 
    [RelayCommand]
    public async Task ImportToMaybelistDialog()
    {
      var response = await GetImportDialog();
      await DeckBuilder.ImportCards(CardlistType.Maybelist, response);
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
      if (Enum.TryParse(dir, true, out SortDirection sortDirection))
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
      if (Enum.TryParse(prop, true, out SortProperty sortProperty))
      {
        DeckBuilder.SelectedSortProperty = sortProperty;
        await DeckBuilder.SortDeck();
      }
    }

    public void AddToDeckCards(MTGCard card)
    {
      DeckBuilder.CardDeck.AddToCardlist(CardlistType.Deck, card);
    }
    public void AddToWishlist(MTGCard card)
    {
      DeckBuilder.CardDeck.AddToCardlist(CardlistType.Wishlist, card);
    }
    public void AddToMaybelist(MTGCard card)
    {
      DeckBuilder.CardDeck.AddToCardlist(CardlistType.Maybelist, card);
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

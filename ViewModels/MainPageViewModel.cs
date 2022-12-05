using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Charts;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels
{
  public enum MTGSearchFormatTypes
  {
    Any, Modern, Standard, Commander,
  }
  public enum MTGSearchUniqueTypes
  {
    Cards, Prints, Art
  }
  public enum MTGSearchOrderTypes
  {
    Released, Set, CMC, Name, Rarity, Color, Eur
  }
  public enum MTGSearchDirectionTypes
  {
    Asc, Desc
  }

  public partial class MainPageViewModel : ViewModelBase
  {
    public MainPageViewModel() 
    {
      DeckViewModel.PropertyChanged += DeckViewModel_PropertyChanged;
      DeckMaybelistViewModel.PropertyChanged += DeckMaybelistViewModel_PropertyChanged;
      DeckWishlistViewModel.PropertyChanged += DeckWishlistViewModel_PropertyChanged;

      CMCChart = new CMCChart(DeckViewModel.CardModels);
      SpellTypeChart = new SpellTypeChart(DeckViewModel.CardModels);
    }

    private void DeckWishlistViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      // Check if deck commands can execute when the deck changes
      if (e.PropertyName == nameof(MTGDeckViewModel.TotalCount))
      {
        ExportCollectionDialogCommand.NotifyCanExecuteChanged();
      }
    }
    private void DeckMaybelistViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      // Check if deck commands can execute when the deck changes
      if (e.PropertyName == nameof(MTGDeckViewModel.TotalCount))
      {
        ExportCollectionDialogCommand.NotifyCanExecuteChanged();
      }
    }
    private void DeckViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      // Check if deck commands can execute when the deck changes
      if(e.PropertyName == nameof(MTGDeckViewModel.TotalCount)) { 
        SaveCollectionDialogCommand.NotifyCanExecuteChanged();
        ExportCollectionDialogCommand.NotifyCanExecuteChanged();
      }
      else if(e.PropertyName == nameof(MTGDeckViewModel.Name)) { DeleteCollectionDialogCommand.NotifyCanExecuteChanged(); }
    }

    public APISearchCardCollectionViewModel APISearchCollectionViewModel { get; } = new(new());
    public MTGDeckViewModel DeckViewModel { get; } = new(new());
    public MTGWishlistViewModel DeckWishlistViewModel { get; } = new(new());
    public MTGMaybelistViewModel DeckMaybelistViewModel { get; } = new(new());
    public CMCChart CMCChart { get; }
    public SpellTypeChart SpellTypeChart { get; }

    private string SearchQuery
    {
      get
      {
        return SearchText == "" ? "" :
        $"{SearchText}+" +
        $"unique:{SearchUnique}+" +
        $"order:{SearchOrder}+" +
        $"direction:{SearchDirection}+" +
        $"format:{SearchFormat}";
      }
    }
    [ObservableProperty]
    private string searchText;
    [ObservableProperty]
    private MTGSearchFormatTypes searchFormat;
    [ObservableProperty]
    private MTGSearchUniqueTypes searchUnique;
    [ObservableProperty]
    private MTGSearchOrderTypes searchOrder;
    [ObservableProperty]
    private MTGSearchDirectionTypes searchDirection;

    [RelayCommand]
    public async void SearchSubmit()
    {
      await APISearchCollectionViewModel.Search(SearchQuery);
    }
    [RelayCommand]
    public async Task ResetCollectionDialog()
    {
      if (DeckViewModel.HasUnsavedChanges)
      {
        // Collection has unsaved changes
        var wantSaveConfirmed = await GetUnsavedDialog();
        if (wantSaveConfirmed == null) { return; }
        else if (wantSaveConfirmed is true)
        {
          // User wants to save the unsaved changes
          var saveName = await GetSaveDialog(DeckViewModel.Name);
          if (saveName == string.Empty) { return; }
          else
          {
            if (saveName != DeckViewModel.Name && Database.CardDeck.Exists(saveName))
            {
              // File exists already
              var overrideConfirmed = await GetOverrideDialog(saveName);
              if (overrideConfirmed == null) { return; }
              else if (overrideConfirmed is true)
              {
                // User wants to override save
                SaveCollections(saveName);
              }
            }
          }
        }
      }
      ResetCollections();
    }
    [RelayCommand]
    public async Task OpenCollectionDialog()
    {
      if (DeckViewModel.HasUnsavedChanges)
      {
        // Collection has unsaved changes
        var wantSaveConfirmed = await GetUnsavedDialog();
        if (wantSaveConfirmed == null) { return; }
        else if (wantSaveConfirmed is true)
        {
          // User wants to save the unsaved changes
          var saveName = await GetSaveDialog(DeckViewModel.Name);
          if (saveName == string.Empty) { return; }
          else
          {
            if (saveName != DeckViewModel.Name && Database.CardDeck.Exists(saveName))
            {
              // File exists already
              var overrideConfirmed = await GetOverrideDialog(saveName);
              if (overrideConfirmed == null) { return; }
              else if (overrideConfirmed is true)
              {
                // User wants to override save
                SaveCollections(saveName);
              }
            }
          }
        }
      }

      using var db = new Database.CardCollectionContext();

      var deckNames = db.CardDecks.Select(x => x.Name).ToArray();
      var openName = await GetOpenDialog(deckNames);
      if (openName != string.Empty)
      {
        LoadCollections(openName);
      }
    }
    [RelayCommand(CanExecute = nameof(CanSaveCollection))]
    public async Task SaveCollectionDialog()
    {
      var saveName = await GetSaveDialog(DeckViewModel.Name);
      if (saveName == string.Empty) { return; }
      else
      {
        if (saveName != DeckViewModel.Name && Database.CardDeck.Exists(saveName))
        {
          // File exists already
          if (GetOverrideDialog(saveName) == null) { return; }
        }
      }

      SaveCollections(saveName);
    }
    [RelayCommand(CanExecute = nameof(CanDeleteCollection))]
    public async Task DeleteCollectionDialog()
    {
      if (string.IsNullOrEmpty(DeckViewModel.Name)) { return; }
      if (await GetDeleteDialog(DeckViewModel.Name) is true)
      {
        if(await DeckViewModel.DeleteDeckAsync())
        {
          ResetCollections();
        }
      }
    }
    [RelayCommand]
    public async Task ImportCollectionDialog(MTGCardCollectionViewModel collectionVM)
    {
      var response = await GetImportDialog();
      if (!string.IsNullOrEmpty(response))
      {
        _ = collectionVM.ImportFromStringAsync(response);
      }
    }
    [RelayCommand(CanExecute = nameof(CanExportCollection))]
    public async Task ExportCollectionDialog(MTGCardCollectionViewModel collectionVM)
    {
      StringBuilder stringBuilder = new();
      foreach (var item in collectionVM.CardModels)
      {
        stringBuilder.AppendLine($"{item.Count} {item.Info.Name}");
      }

      var response = await GetExportDialog(stringBuilder.ToString());
      if (!string.IsNullOrEmpty(response))
      {
        IO.CopyToClipboard(response);
      }
    }

    // Dialogs
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
    private static async Task<string> GetOpenDialog(string[] fileNames)
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

    private void SaveCollections(string saveName)
    {
      _ = DeckViewModel.Save(saveName);
      _ = DeckWishlistViewModel.Save(saveName);
      _ = DeckMaybelistViewModel.Save(saveName);
    }
    private void LoadCollections(string openName)
    {
      _ = DeckViewModel.LoadAsync(openName);
      _ = DeckWishlistViewModel.LoadAsync(openName);
      _ = DeckMaybelistViewModel.LoadAsync(openName);
    }
    private void ResetCollections()
    {
      DeckViewModel.Reset();
      DeckWishlistViewModel.Reset();
      DeckMaybelistViewModel.Reset();
    }
    
    // Command CanExecute functions
    private bool CanSaveCollection() => DeckViewModel.TotalCount > 0;
    private bool CanDeleteCollection() => DeckViewModel.Name != string.Empty;
    private bool CanExportCollection(MTGCardCollectionViewModel collectionVM)
    {
      if(collectionVM == null) { return false; }
      return collectionVM.TotalCount > 0;
    }
  }
}

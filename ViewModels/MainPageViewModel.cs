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
      CollectionViewModel.PropertyChanged += CollectionViewModel_PropertyChanged;

      CMCChart = new CMCChart(CollectionViewModel.CardModels);
      SpellTypeChart = new SpellTypeChart(CollectionViewModel.CardModels);
    }

    private void CollectionViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if(e.PropertyName == nameof(MTGCardCollectionViewModel.TotalCount)) { SaveCollectionDialogCommand.NotifyCanExecuteChanged(); }
      else if(e.PropertyName == nameof(MTGCardCollectionViewModel.Name)) { DeleteCollectionDialogCommand.NotifyCanExecuteChanged(); }
    }

    public MTGCardCollectionViewModel ScryfallCardViewModels { get; } = new(new());
    public MTGCardCollectionViewModel CollectionViewModel { get; } = new(new());
    public MTGCardCollectionViewModel CollectionMaybeViewModel { get; } = new(new());
    public MTGCardCollectionViewModel CollectionWishlistViewModel { get; } = new(new());
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
      await ScryfallCardViewModels.LoadFromAPIAsync(SearchQuery);
    }
    [RelayCommand]
    public async Task ResetCollectionDialog()
    {
      if (CollectionViewModel.HasUnsavedChanges)
      {
        // Collection has unsaved changes
        var wantSaveConfirmed = await GetUnsavedDialog();
        if (wantSaveConfirmed == null) { return; }
        else if (wantSaveConfirmed is true)
        {
          // User wants to save the unsaved changes
          var saveName = await GetSaveDialog(CollectionViewModel.Name);
          if (saveName == string.Empty) { return; }
          else
          {
            if (saveName != CollectionViewModel.Name && IO.GetJsonFileNames(IO.CollectionsPath).Contains(saveName))
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
      if (CollectionViewModel.HasUnsavedChanges)
      {
        // Collection has unsaved changes
        var wantSaveConfirmed = await GetUnsavedDialog();
        if (wantSaveConfirmed == null) { return; }
        else if (wantSaveConfirmed is true)
        {
          // User wants to save the unsaved changes
          var saveName = await GetSaveDialog(CollectionViewModel.Name);
          if (saveName == string.Empty) { return; }
          else
          {
            if (saveName != CollectionViewModel.Name && IO.GetJsonFileNames(IO.CollectionsPath).Contains(saveName))
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

      var jsonNames = IO.GetJsonFileNames(IO.CollectionsPath);
      var openName = await GetOpenDialog(jsonNames);
      if (openName != string.Empty)
      {
        await LoadCollections(openName);
      }
    }
    [RelayCommand(CanExecute = nameof(CanSaveCollection))]
    public async Task SaveCollectionDialog()
    {
      var saveName = await GetSaveDialog(CollectionViewModel.Name);
      if (saveName == string.Empty) { return; }
      else
      {
        if (saveName != CollectionViewModel.Name && IO.GetJsonFileNames(IO.CollectionsPath).Contains(saveName))
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
      if (string.IsNullOrEmpty(CollectionViewModel.Name)) { return; }
      if (await GetDeleteDialog(CollectionViewModel.Name) is true)
      {
        CollectionViewModel.DeleteDeckFile();
      }
    }
    [RelayCommand]
    public async Task ImportCollectionDialog()
    {
      var response = await GetImportDialog();
      if (!string.IsNullOrEmpty(response))
      {
        await CollectionViewModel.ImportFromString(response);
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
      CollectionViewModel.Save(IO.CollectionsPath, saveName);
      CollectionMaybeViewModel.Save(IO.CollectionsMaybePath, saveName);
      CollectionWishlistViewModel.Save(IO.CollectionsWishlistPath, saveName);
    }
    private async Task LoadCollections(string openName)
    {
      await CollectionViewModel.LoadAsync(IO.CollectionsPath, openName);
      await CollectionMaybeViewModel.LoadAsync(IO.CollectionsMaybePath, openName);
      await CollectionWishlistViewModel.LoadAsync(IO.CollectionsWishlistPath, openName);
    } 
    private void ResetCollections()
    {
      CollectionViewModel.Reset();
      CollectionMaybeViewModel.Reset();
      CollectionWishlistViewModel.Reset();
    }
    // Command CanExecute functions
    private bool CanSaveCollection() => CollectionViewModel.TotalCount > 0;
    private bool CanDeleteCollection() => CollectionViewModel.Name != string.Empty;
    private bool CanExportCollection() => CollectionViewModel.TotalCount > 0;
  }
}

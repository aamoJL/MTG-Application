using CommunityToolkit.Mvvm.Input;
using MTGApplication.Charts;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels
{
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

    public string SearchQuery
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
    public string SearchText { get; set; } = "";
    public string SearchFormat { get; set; } = "Any";
    public string SearchUnique { get; set; } = "Cards";
    public string SearchOrder { get; set; } = "Released";
    public string SearchDirection { get; set; } = "Asc";

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
        var wantSaveConfirmed = await App.MainRoot.ConfirmationDialogAsync(
          title: "Save unsaved changes?",
          content: "Would you like to save unsaved changes?",
          yesButtonText: "Save");

        if (wantSaveConfirmed == null) { return; }
        else if (wantSaveConfirmed is true)
        {
          // User wants to save the unsaved changes
          var saveName = await App.MainRoot.InputStringDialogAsync(
            title: "Save your deck?",
            defaultText: CollectionViewModel.Name,
            okButtonText: "Save");

          if (saveName == string.Empty) { return; }
          else
          {
            if (saveName != CollectionViewModel.Name && IO.GetJsonFileNames(IO.CollectionsPath).Contains(saveName))
            {
              // File exists already
              var overrideConfirmed = await App.MainRoot.ConfirmationDialogAsync(
                title: "Override existing deck?",
                content: $"Deck '{saveName}' already exist. Would you like to override the deck?",
                noButtonText: string.Empty);

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
        var wantSaveConfirmed = await App.MainRoot.ConfirmationDialogAsync(
          title: "Save unsaved changes?",
          content: "Would you like to save unsaved changes?",
          yesButtonText: "Save");

        if (wantSaveConfirmed == null) { return; }
        else if (wantSaveConfirmed is true)
        {
          // User wants to save the unsaved changes
          var saveName = await App.MainRoot.InputStringDialogAsync(
            title: "Save your deck?",
            defaultText: CollectionViewModel.Name,
            okButtonText: "Save");

          if (saveName == string.Empty) { return; }
          else
          {
            if (saveName != CollectionViewModel.Name && IO.GetJsonFileNames(IO.CollectionsPath).Contains(saveName))
            {
              // File exists already
              var overrideConfirmed = await App.MainRoot.ConfirmationDialogAsync(
                title: "Override existing deck?",
                content: $"Deck '{saveName}' already exist. Would you like to override the deck?",
                noButtonText: string.Empty);

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
      var openName = await App.MainRoot.ComboboxDialogAsync(
        title: "Open deck",
        okButtonText: "Open",
        items: jsonNames,
        header: "Name"
        );

      if (openName != string.Empty)
      {
        await LoadCollections(openName);
      }
    }
    [RelayCommand(CanExecute = nameof(CanSaveCollection))]
    public async Task SaveCollectionDialog()
    {
      var saveName = await App.MainRoot.InputStringDialogAsync(
        title: "Save your deck?",
        defaultText: CollectionViewModel.Name,
        okButtonText: "Save");

      if (saveName == string.Empty) { return; }
      else
      {
        if (saveName != CollectionViewModel.Name && IO.GetJsonFileNames(IO.CollectionsPath).Contains(saveName))
        {
          // File exists already
          var overrideConfirmed = await App.MainRoot.ConfirmationDialogAsync(
            title: "Override existing deck?",
            content: $"Deck '{saveName}' already exist. Would you like to override the deck?",
            noButtonText: string.Empty);

          if (overrideConfirmed == null) { return; }
        }
      }

      SaveCollections(saveName);
    }
    [RelayCommand(CanExecute = nameof(CanDeleteCollection))]
    public async Task DeleteCollectionDialog()
    {
      if (string.IsNullOrEmpty(CollectionViewModel.Name)) { return; }

      var confirmed = await App.MainRoot.ConfirmationDialogAsync(
        title: "Delete deck?",
        content: $"Deleting '{CollectionViewModel.Name}'. Are you sure?",
        noButtonText: string.Empty);

      if (confirmed is true)
      {
        CollectionViewModel.DeleteDeckFile();
      }
    }
    [RelayCommand]
    public async Task ImportCollectionDialog()
    {
      var response = await App.MainRoot.TextAreaInputDialogAsync(
            title: "Import cards",
            inputPlaceholder: "Example:\n2 Black Lotus\nMox Ruby",
            okButtonText: "Add to Collection");

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

      var response = await App.MainRoot.TextAreaInputDialogAsync(
            title: "Export deck",
            defaultText: stringBuilder.ToString(),
            okButtonText: "Copy to Clipboard");

      if (!string.IsNullOrEmpty(response))
      {
        IO.CopyToClipboard(response);
      }
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

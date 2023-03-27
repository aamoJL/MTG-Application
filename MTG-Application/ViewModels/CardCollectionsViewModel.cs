using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Services;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MTGApplication.Services.DialogService;
using static MTGApplication.Services.IOService;

namespace MTGApplication.ViewModels;

/// <summary>
/// Card Collections Tab view model
/// </summary>
public partial class CardCollectionsViewModel : ViewModelBase, ISavable
{
  /// <summary>
  /// Card Collections tab dialogs
  /// </summary>
  public class CardCollectionsDialogs
  {
    public virtual CollectionListContentDialog GetEditCollectionListDialog(string nameInputText, string queryInputText)
      => new("Edit list") { NameInputText = nameInputText, QueryInputText = queryInputText, PrimaryButtonText = "Update" };

    public virtual CollectionListContentDialog GetNewCollectionListDialog() => new("Add new list");
    
    public virtual ConfirmationDialog GetDeleteCollectionDialog(string name)
      => new("Delete collection?") { Message = $"Are you sure you want to delete collection '{name}'?", SecondaryButtonText = string.Empty };
    
    public virtual ConfirmationDialog GetDeleteListDialog(string name)
      => new("Delete list?") { Message = $"Are you sure you want to delete list '{name}'?", SecondaryButtonText = string.Empty };
    
    public virtual ConfirmationDialog GetOverrideDialog(string name)
      => new("Override existing collection?") { Message = $"Collection '{name}' already exist. Would you like to override the collection?", SecondaryButtonText = string.Empty };
    
    public virtual ConfirmationDialog GetSaveUnsavedDialog()
      => new("Save unsaved changes?") { Message = "Collection has unsaved changes. Would you like to save the collection?", PrimaryButtonText = "Save" };
    
    public virtual GridViewDialog GetCardPrintDialog(MTGCardViewModel[] printViewModels)
      => new("Illustration prints", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle") { Items = printViewModels, SecondaryButtonText = string.Empty, PrimaryButtonText = string.Empty, CloseButtonText = "Close" };
    
    public virtual ComboBoxDialog GetLoadDialog(string[] names)
      => new("Open collection") { InputHeader = "Name", Items = names, PrimaryButtonText = "Open", SecondaryButtonText = string.Empty };
    
    public virtual TextBoxDialog GetSaveDialog(string name)
      => new("Save your collection?") { InvalidInputCharacters = Path.GetInvalidFileNameChars(), TextInputText = name, PrimaryButtonText = "Save", SecondaryButtonText = string.Empty };
    
    public virtual TextAreaDialog GetExportDialog(string text)
      => new("Export list") { TextInputText = text, PrimaryButtonText = "Copy to Clipboard", SecondaryButtonText = string.Empty };
    
    public virtual TextAreaDialog GetImportDialog()
      => new("Import list") { InputPlaceholderText = "Black lotus\nMox Ruby", SecondaryButtonText = string.Empty, PrimaryButtonText = "Add to Collection" };

    public class CollectionListContentDialog : Dialog<MTGCardCollectionList>
    {
      protected TextBox nameBox;
      protected TextBox searchQueryBox;

      public string NameInputText { get; set; }
      public string QueryInputText { get; set; }

      public CollectionListContentDialog(string title = "") : base(title) { }

      public override ContentDialog GetDialog()
      {
        var dialog = base.GetDialog();

        nameBox = new()
        {
          PlaceholderText = "List name...",
          Header = "Name",
          Text = NameInputText,
          IsSpellCheckEnabled = false,
          Margin = new() { Bottom = 10 },
        };
        searchQueryBox = new()
        {
          PlaceholderText = "Search query...",
          Text = QueryInputText,
          Header = new StackPanel()
          {
            Children =
            {
              new TextBlock(){ Text = "Search query" },
              new HyperlinkButton()
              {
                Content = "syntax?",
                NavigateUri = new Uri("https://scryfall.com/docs/syntax"),
                Padding = new Thickness(5, 0, 5, 0),
                Margin = new Thickness(5, 0, 5, 0),
              }
            },
            Orientation = Orientation.Horizontal,
          },
          IsSpellCheckEnabled = false,
        };

        nameBox.TextChanged += (sender, args) =>
        {
          dialog.IsPrimaryButtonEnabled = nameBox.Text != string.Empty && searchQueryBox.Text != string.Empty;
          NameInputText = nameBox.Text;
        };
        searchQueryBox.TextChanged += (sender, args) =>
        {
          dialog.IsPrimaryButtonEnabled = nameBox.Text != string.Empty && searchQueryBox.Text != string.Empty;
          QueryInputText = searchQueryBox.Text;
        };

        dialog.Content = new StackPanel()
        {
          Children =
          {
            nameBox,
            searchQueryBox,
          },
          Orientation = Orientation.Vertical,
        };

        dialog.IsPrimaryButtonEnabled = !string.IsNullOrEmpty(nameBox.Text) && !string.IsNullOrEmpty(searchQueryBox.Text);
        dialog.SecondaryButtonText = string.Empty;
        dialog.PrimaryButtonText = "Add";

        return dialog;
      }

      public override MTGCardCollectionList ProcessResult(ContentDialogResult result)
      {
        return result switch
        {
          ContentDialogResult.Primary => new MTGCardCollectionList() { Name = NameInputText, SearchQuery = QueryInputText },
          _ => null
        };
      }
    }
  }

  public CardCollectionsViewModel(ICardAPI<MTGCard> cardAPI, IRepository<MTGCardCollection> collectionRepository, ClipboardService clipboardService = default)
  {
    CardAPI = cardAPI;
    CollectionRepository = collectionRepository;
    MTGSearchViewModel = new(CardAPI);
    ClipboardService = clipboardService ?? new();

    PropertyChanged += CardCollectionsViewModel_PropertyChanged;
    MTGSearchViewModel.PropertyChanged += MTGSearchViewModel_PropertyChanged;
  }

  private async void MTGSearchViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(MTGSearchViewModel.IsBusy))
    { IsBusy = MTGSearchViewModel.IsBusy; }
    else if (e.PropertyName == nameof(MTGSearchViewModel.SearchQuery))
    {
      await MTGSearchViewModel.SearchSubmit();
    }
    else if (e.PropertyName == nameof(MTGSearchViewModel.SearchCards))
    {
      MTGSearchViewModel.SearchCards.CollectionChanged += (s, e) =>
      {
        switch (e.Action)
        {
          case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
            var newCard = e.NewItems[0] as MTGCardCollectionCardViewModel;
            if (SelectedList.Cards.FirstOrDefault(x => x.Info.ScryfallId == newCard.Model.Info.ScryfallId) != null)
            { newCard.IsOwned = true; }
            newCard.ShowPrintsDialogCommand = ShowCardIllustrationsCommand;
            break;
          default:
            break;
        }
      };
    }
  }
  
  private async void CardCollectionsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(SelectedList))
    {
      OnPropertyChanged(nameof(SelectedListCardCount));
      if (SelectedList != null)
      {
        SelectedList.Cards.CollectionChanged += SelectedListCards_CollectionChanged;
      }
      // Search does not update itself if the query is same as previous query, so it has to be updated manually.
      if (MTGSearchViewModel.SearchQuery == SelectedList?.SearchQuery)
      { await MTGSearchViewModel.SearchSubmit(); }
      else
      { MTGSearchViewModel.SearchQuery = SelectedList?.SearchQuery ?? string.Empty; }
    }
  }
  
  private void SelectedListCards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
    {
      if (MTGSearchViewModel.SearchCards.FirstOrDefault(x => x.Model.Info.ScryfallId == (e.NewItems[0] as MTGCard).Info.ScryfallId) is MTGCardCollectionCardViewModel cardVM)
      {
        cardVM.IsOwned = true;
      }
    }
    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
    {
      if (MTGSearchViewModel.SearchCards.FirstOrDefault(x => x.Model.Info.ScryfallId == (e.OldItems[0] as MTGCard).Info.ScryfallId) is MTGCardCollectionCardViewModel cardVM)
      {
        cardVM.IsOwned = false;
      }
    }

    HasUnsavedChanges = true;
    OnPropertyChanged(nameof(SelectedListCardCount));
  }

  [ObservableProperty,
    NotifyCanExecuteChangedFor(nameof(EditCollectionListDialogCommand)),
    NotifyCanExecuteChangedFor(nameof(DeleteCollectionListDialogCommand)),
    NotifyCanExecuteChangedFor(nameof(ImportCollectionListDialogCommand)),
    NotifyCanExecuteChangedFor(nameof(ExportCollectionListDialogCommand))]
  private MTGCardCollectionList selectedList;
  [ObservableProperty]
  private MTGCardCollection collection = new();
  [ObservableProperty]
  private bool isBusy = false;

  public MTGAPISearch<MTGCardCollectionCardViewModelSource, MTGCardCollectionCardViewModel> MTGSearchViewModel { get; }
  public CardCollectionsDialogs Dialogs { get; init; } = new();
  public int SelectedListCardCount => SelectedList?.Cards.Count ?? 0;

  private IRepository<MTGCardCollection> CollectionRepository { get; }
  private ICardAPI<MTGCard> CardAPI { get; }
  private ClipboardService ClipboardService { get; set; }

  #region ISavable implementation
  [ObservableProperty]
  private bool hasUnsavedChanges = false;

  public async Task<bool> SaveUnsavedChanges() => await ShowUnsavedDialogs();
  #endregion

  #region Relay Commands
  /// <summary>
  /// Asks if user wants to save unsaved changes and clears the <see cref="Collection"/>
  /// </summary>
  [RelayCommand]
  public async Task NewCollectionDialog()
  {
    if (await ShowUnsavedDialogs()) 
      NewCollection();
  }

  /// <summary>
  /// Asks if the user wants to save unsaved changes and changes the current <see cref="Collection"/>
  /// to the loaded collection
  /// </summary>
  [RelayCommand]
  public async Task LoadCollectionDialog()
  {
    if (await ShowUnsavedDialogs())
    {
      var loadName = await Dialogs.GetLoadDialog((await CollectionRepository.Get()).Select(x => x.Name).ToArray()).ShowAsync();
      if (loadName != null)
      {
        await LoadCollection(loadName);
      }
    }
  }

  /// <summary>
  /// Saves the current <see cref="Collection"/> with the given name to the database
  /// </summary>
  [RelayCommand(CanExecute = nameof(SaveCollectionCommandCanExecute))]
  public async Task SaveCollectionDialog()
  {
    var saveName = await Dialogs.GetSaveDialog(Collection.Name).ShowAsync();
    if (string.IsNullOrEmpty(saveName))
    { return; }
    else
    {
      if (saveName != Collection.Name && await CollectionRepository.Exists(saveName))
      {
        // Collection exists already
        if (await Dialogs.GetOverrideDialog(saveName).ShowAsync() == null)
        { return; }
      }
    }

    await SaveCollection(saveName);
  }

  /// <summary>
  /// Deletes the current <see cref="Collection"/> from the database
  /// </summary>
  [RelayCommand(CanExecute = nameof(DeleteCollectionCommandCanExecute))]
  public async Task DeleteCollectionDialog()
  {
    if (!await CollectionRepository.Exists(Collection.Name))
    { return; }
    else if (await Dialogs.GetDeleteCollectionDialog(Collection.Name).ShowAsync() is true)
    {
      await DeleteCollection();
    }
  }

  /// <summary>
  /// Adds given collection list to the <see cref="Collection"/>
  /// </summary>
  [RelayCommand]
  public async Task NewCollectionListDialog()
  {
    if (await Dialogs.GetNewCollectionListDialog().ShowAsync() is MTGCardCollectionList newList)
    {
      NewCollectionList(newList);
    }
  }

  /// <summary>
  /// Changes the <see cref="SelectedList"/>'s information to the given information
  /// </summary>
  [RelayCommand(CanExecute = nameof(SelectedListCommandCanExecute))]
  public async Task EditCollectionListDialog()
  {
    if (SelectedList != null && await Dialogs.GetEditCollectionListDialog(SelectedList.Name, SelectedList.SearchQuery).ShowAsync() is MTGCardCollectionList updatedList)
    {
      UpdateSelectedCollectionList(updatedList);
    }
  }

  /// <summary>
  /// Changes <see cref="SelectedList"/> to the given <paramref name="list"/>
  /// </summary>
  [RelayCommand]
  public void ChangeSelectedCollectionList(MTGCardCollectionList list) => SelectedList = list;

  /// <summary>
  /// Removes the <see cref="SelectedList"/> from the <see cref="Collection"/>
  /// </summary>
  [RelayCommand(CanExecute = nameof(SelectedListCommandCanExecute))]
  public async Task DeleteCollectionListDialog()
  {
    if (SelectedList == null)
    { return; }
    else if (await Dialogs.GetDeleteListDialog(SelectedList.Name).ShowAsync() is true)
    {
      DeleteSelectedCollectionList();
    }
  }

  /// <summary>
  /// Shows dialog that can be used to import cards to the selected list
  /// </summary>
  [RelayCommand(CanExecute = nameof(SelectedListCommandCanExecute))]
  public async Task ImportCollectionListDialog()
  {
    if (await Dialogs.GetImportDialog().ShowAsync() is string importText)
    {
      await ImportCards(importText);
    }
  }

  /// <summary>
  /// Shows dialog with selected list's card IDs
  /// </summary>
  [RelayCommand(CanExecute = nameof(SelectedListCommandCanExecute))]
  public async Task ExportCollectionListDialog()
  {
    var response = await Dialogs.GetExportDialog(GetExportString()).ShowAsync();
    if (!string.IsNullOrEmpty(response))
    {
      ClipboardService.Copy(response);
    }
  }

  /// <summary>
  /// Shows the prints with the same illustrations than the given card's print's illustration
  /// </summary>
  [RelayCommand]
  private async Task ShowCardIllustrations(MTGCard card)
  {
    // Get prints
    IsBusy = true;
    var prints = (await CardAPI.FetchCardsWithParameters($"!\"{card.Info.Name}\"+unique:prints+game:paper")).Found.Where(x => x.Info.FrontFace.IllustrationId == card.Info.FrontFace.IllustrationId).ToArray();
    var printViewModels = prints.Select(x => new MTGCardViewModel(x)).ToArray();
    IsBusy = false;

    await Dialogs.GetCardPrintDialog(printViewModels).ShowAsync();
  }
  #endregion

  /// <summary>
  /// Changes current collection to new collection
  /// </summary>
  private void NewCollection()
  {
    Collection = new MTGCardCollection();
    hasUnsavedChanges = false;
  }
  
  /// <summary>
  /// Saves current collection with the given <paramref name="name"/>
  /// </summary>
  private async Task SaveCollection(string name)
  {
    IsBusy = true;
    var saveCollection = new MTGCardCollection()
    {
      Name = name,
      CollectionLists = Collection.CollectionLists,
    };
    if (await Task.Run(() => CollectionRepository.AddOrUpdate(saveCollection)))
    {
      Collection.Name = name;
      HasUnsavedChanges = false;
      NotificationService.RaiseNotification(NotificationService.NotificationType.Success, "The collection was saved successfully.");
    }
    else
    { NotificationService.RaiseNotification(NotificationService.NotificationType.Error, "Error. Could not save the collection."); }
    IsBusy = false;
  }
  
  /// <summary>
  /// Loads collection with the given <paramref name="name"/> and sets the current collection to the loaded collection
  /// </summary>
  private async Task LoadCollection(string name)
  {
    IsBusy = true;
    if (await Task.Run(() => CollectionRepository.Get(name)) is MTGCardCollection loadedCollection)
    {
      Collection = loadedCollection;
      if (Collection.CollectionLists.Count > 0)
      { ChangeSelectedCollectionList(Collection.CollectionLists[0]); }
      NotificationService.RaiseNotification(NotificationService.NotificationType.Success, "The collection was loaded successfully.");
    }
    else
    { NotificationService.RaiseNotification(NotificationService.NotificationType.Error, "Error. Could not load the collection."); }
    IsBusy = false;
  }
  
  /// <summary>
  /// Deletes current collection
  /// </summary>
  private async Task DeleteCollection()
  {
    IsBusy = true;
    if (await Task.Run(() => CollectionRepository.Remove(Collection)))
    {
      Collection = new();
      HasUnsavedChanges = false;
      NotificationService.RaiseNotification(NotificationService.NotificationType.Success, "The collection was deleted successfully.");
    }
    else
    { NotificationService.RaiseNotification(NotificationService.NotificationType.Error, "Error. Could not delete the collection."); }
    IsBusy = false;
  }
  
  /// <summary>
  /// Adds the given <paramref name="list"/> to the current collection
  /// </summary>
  private void NewCollectionList(MTGCardCollectionList list)
  {
    if (Collection.CollectionLists.FirstOrDefault(x => x.Name == list.Name) == null)
    {
      Collection.CollectionLists.Add(list);
      ChangeSelectedCollectionList(list);
      HasUnsavedChanges = true;
      NotificationService.RaiseNotification(NotificationService.NotificationType.Success, "List added to the collection successfully.");
    }
    else
    { NotificationService.RaiseNotification(NotificationService.NotificationType.Error, "Error. List already exists in the collection."); }
  }
  
  /// <summary>
  /// Updates the <see cref="SelectedList"/> to be <paramref name="list"/>
  /// </summary>
  private void UpdateSelectedCollectionList(MTGCardCollectionList list)
  {
    if (SelectedList == null)
    { return; }
    SelectedList.Name = list.Name;
    SelectedList.SearchQuery = list.SearchQuery;
    MTGSearchViewModel.SearchQuery = list.SearchQuery;
    HasUnsavedChanges = true;
  }
  
  /// <summary>
  /// Deletes <see cref="SelectedList"/> from the current collection
  /// </summary>
  private void DeleteSelectedCollectionList()
  {
    if (Collection.CollectionLists.Remove(SelectedList))
    {
      SelectedList = Collection.CollectionLists.Count > 0 ? Collection.CollectionLists[0] : null;
      HasUnsavedChanges = true;
      NotificationService.RaiseNotification(NotificationService.NotificationType.Success, "The list was deleted successfully.");
    }
    else
    { NotificationService.RaiseNotification(NotificationService.NotificationType.Error, "Error. Could not delete the list."); }
  }
  
  /// <summary>
  /// Returns exportable string of the <see cref="SelectedList"/>'s cards
  /// </summary>
  private string GetExportString()
  {
    if (SelectedList == null)
    { return string.Empty; }
    StringBuilder stringBuilder = new();
    foreach (var item in SelectedList.Cards)
    {
      stringBuilder.AppendLine($"{item.Info.ScryfallId}");
    }

    return stringBuilder.ToString();
  }
  
  /// <summary>
  /// Imports cards from the <paramref name="importText"/> to the <see cref="SelectedList"/>
  /// </summary>
  private async Task ImportCards(string importText)
  {
    var notFoundCount = 0;
    var found = Array.Empty<MTGCard>();
    var notImportedCount = 0;

    if (!string.IsNullOrEmpty(importText) && SelectedList != null)
    {
      IsBusy = true;
      var result = await CardAPI.FetchFromString(importText);
      found = result.Found;
      notFoundCount = result.NotFoundCount;
      foreach (var card in found)
      {
        if (SelectedList.Cards.FirstOrDefault(x => x.Info.ScryfallId == card.Info.ScryfallId) is null)
        {
          // Card is not in the list
          SelectedList.AddToList(card);
        }
        else
        { notImportedCount++; }
      }
    }

    if (notFoundCount == 0 && found.Length > 0)
      NotificationService.RaiseNotification(NotificationService.NotificationType.Success, $"{found.Length - notImportedCount} cards imported successfully." + (notImportedCount > 0 ? $" ({notImportedCount} cards skipped) " : ""));
    else if (found.Length > 0 && notFoundCount > 0)
      NotificationService.RaiseNotification(NotificationService.NotificationType.Warning,
        $"{found.Length} / {notFoundCount + found.Length} cards imported successfully.{Environment.NewLine}{notFoundCount} cards were not found." + (notImportedCount > 0 ? $" ({notImportedCount} cards skipped) " : ""));
    else if (found.Length == 0)
      NotificationService.RaiseNotification(NotificationService.NotificationType.Error, $"Error. No cards were imported.");

    IsBusy = false;
  }

  /// <summary>
  /// Asks user if they want to save the collections's unsaved changes.
  /// </summary>
  /// <returns><see langword="true"/>, if user does not cancel any of the dialogs</returns>
  private async Task<bool> ShowUnsavedDialogs()
  {
    if (HasUnsavedChanges)
    {
      // Collection has unsaved changes
      var wantSaveConfirmed = await Dialogs.GetSaveUnsavedDialog().ShowAsync();
      if (wantSaveConfirmed == null)
      { return false; }
      else if (wantSaveConfirmed is true)
      {
        // User wants to save the unsaved changes
        if (!SaveCollectionCommandCanExecute())
        {
          // Collection can't be saved if it has no lists.
          NotificationService.RaiseNotification(NotificationService.NotificationType.Error, "Error. Collection can't be saved, because it has no lists.");
          return false;
        }
        var saveName = await Dialogs.GetSaveDialog(Collection.Name).ShowAsync();
        if (string.IsNullOrEmpty(saveName))
        { return false; }
        else
        {
          if (saveName != Collection.Name && await CollectionRepository.Exists(saveName))
          {
            // Collection exists already
            var overrideConfirmed = await Dialogs.GetOverrideDialog(saveName).ShowAsync();
            if (overrideConfirmed == null)
            { return false; }
            else if (overrideConfirmed is true)
            {
              // User wants to override the colleciton
              await SaveCollection(saveName);
            }
          }
          else
          { await SaveCollection(saveName); }
        }
      }
    }
    return true;
  }

  #region CanExecute command methods
  private bool SaveCollectionCommandCanExecute() => Collection.CollectionLists.Count > 0;
  
  private bool DeleteCollectionCommandCanExecute() => !string.IsNullOrEmpty(Collection.Name);
  
  private bool SelectedListCommandCanExecute() => SelectedList != null;
  #endregion
}

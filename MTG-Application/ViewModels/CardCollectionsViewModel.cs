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
using System.Threading.Tasks;
using static MTGApplication.Views.Dialogs;

namespace MTGApplication.ViewModels
{
  public partial class CardCollectionsViewModel : ViewModelBase, ISavable
  {
    public class CardCollectionsDialogs : ViewModelDialogs
    {
      public virtual CollectionListContentDialog GetEditCollectionListDialog(string nameInputText, string queryInputText)
      {
        return new CollectionListContentDialog("Edit list", DialogWrapper)
        {
          NameInputText = nameInputText,
          QueryInputText = queryInputText,
          PrimaryButtonText = "Update",
        };
        
      }
      public virtual CollectionListContentDialog GetNewCollectionListDialog()
      {
        return new CollectionListContentDialog("Add new list", DialogWrapper);
      }
      public virtual ConfirmationDialog GetDeleteCollectionDialog(string name)
      {
        return new ConfirmationDialog("Delete collection?", DialogWrapper)
        {
          Message = $"Are you sure you want to delete collection '{name}'?",
          SecondaryButtonText = string.Empty,
        };
      }
      public virtual ConfirmationDialog GetDeleteListDialog(string name)
      {
        return new ConfirmationDialog("Delete list?", DialogWrapper)
        {
          Message = $"Are you sure you want to delete list '{name}'?",
          SecondaryButtonText = string.Empty,
        };
      }
      public virtual ConfirmationDialog GetOverrideDialog(string name)
      {
        return new ConfirmationDialog("Override existing collection?", DialogWrapper)
        {
          Message = $"Collection '{name}' already exist. Would you like to override the collection?",
          SecondaryButtonText = string.Empty,
        };
      }
      public virtual ConfirmationDialog GetSaveUnsavedDialog()
      {
        return new ConfirmationDialog("Save unsaved changes?", DialogWrapper)
        {
          Message = "Collection has unsaved changes. Would you like to save the collection?",
          PrimaryButtonText = "Save",
        };
      }
      public virtual GridViewDialog GetCardPrintDialog(MTGCardViewModel[] printViewModels)
      {
        return new GridViewDialog("Illustration prints", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle", DialogWrapper)
        {
          Items = printViewModels,
          SecondaryButtonText = string.Empty,
          PrimaryButtonText = string.Empty,
          CloseButtonText = "Close",
        };
      }
      public virtual ComboBoxDialog GetLoadDialog(string[] names)
      {
        return new ComboBoxDialog("Open collection", DialogWrapper)
        {
          InputHeader = "Name",
          Items = names,
          PrimaryButtonText = "Open",
          SecondaryButtonText = string.Empty,
        };
      }
      public virtual TextBoxDialog GetSaveDialog(string name)
      {
        return new TextBoxDialog("Save your collection?", DialogWrapper)
        {
          InvalidInputCharacters = Path.GetInvalidFileNameChars(),
          TextInputText = name,
          PrimaryButtonText = "Save",
          SecondaryButtonText = string.Empty,
        };
      }

      public class CollectionListContentDialog : Dialog<MTGCardCollectionList>
      {
        protected TextBox nameBox;
        protected TextBox searchQueryBox;
        
        public string NameInputText { get; set; }
        public string QueryInputText { get; set; }

        public CollectionListContentDialog(string title = "", DialogWrapper dialogWrapper = default) : base(title, dialogWrapper) { }

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

    public CardCollectionsViewModel(ICardAPI<MTGCard> cardAPI, IRepository<MTGCardCollection> collectionRepository)
    {
      CardAPI = cardAPI;
      CollectionRepository = collectionRepository;
      MTGSearchViewModel = new(CardAPI);

      PropertyChanged += CardCollectionsViewModel_PropertyChanged;
      MTGSearchViewModel.PropertyChanged += MTGSearchViewModel_PropertyChanged;
    }

    private async void MTGSearchViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if(e.PropertyName == nameof(MTGSearchViewModel.IsBusy)) { IsBusy = MTGSearchViewModel.IsBusy; }
      else if(e.PropertyName == nameof(MTGSearchViewModel.SearchQuery))
      {
        await MTGSearchViewModel.SearchSubmit();
      }
      else if(e.PropertyName == nameof(MTGSearchViewModel.SearchCards))
      {
        MTGSearchViewModel.SearchCards.CollectionChanged += (s, e) =>
        {
          switch (e.Action)
          {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
              var newCard = e.NewItems[0] as MTGCardCollectionCardViewModel;
              if (SelectedList.Cards.FirstOrDefault(x => x.Info.ScryfallId == newCard.Model.Info.ScryfallId) != null) { newCard.IsOwned = true; }
              newCard.ShowPrintsDialogCommand = ShowCardIllustrationsCommand;
              break;
            default: break;
          }
        };
      }
    }
    private async void CardCollectionsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if(e.PropertyName == nameof(SelectedList)) 
      {
        if (SelectedList != null) { SelectedList.Cards.CollectionChanged += (s, e) => { HasUnsavedChanges = true; }; }
        // Search does not update itself if the query is same as previous query, so it has to be updated manually.
        if(MTGSearchViewModel.SearchQuery == SelectedList?.SearchQuery) { await MTGSearchViewModel.SearchSubmit(); }
        else { MTGSearchViewModel.SearchQuery = SelectedList?.SearchQuery ?? string.Empty; }
      }
    }

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(EditCollectionListDialogCommand)), NotifyCanExecuteChangedFor(nameof(DeleteCollectionListDialogCommand))]
    private MTGCardCollectionList selectedList;
    [ObservableProperty]
    private MTGCardCollection collection = new();
    [ObservableProperty]
    private bool hasUnsavedChanges = false;
    [ObservableProperty]
    private bool isBusy = false;

    public MTGAPISearch<MTGCardCollectionCardViewModelSource, MTGCardCollectionCardViewModel> MTGSearchViewModel { get; }
    public CardCollectionsDialogs Dialogs { get; init; } = new();
    
    private IRepository<MTGCardCollection> CollectionRepository { get; }
    private ICardAPI<MTGCard> CardAPI { get; }

    /// <summary>
    /// Asks if user wants to save unsaved changes and clears the <see cref="Collection"/>
    /// </summary>
    [RelayCommand]
    public async Task NewCollectionDialog()
    {
      if (await ShowUnsavedDialogs()) NewCollection();
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
    [RelayCommand(CanExecute = nameof(SaveCollectionDialogCommandCanExecute))]
    public async Task SaveCollectionDialog()
    {
      var saveName = await Dialogs.GetSaveDialog(Collection.Name).ShowAsync();
      if (string.IsNullOrEmpty(saveName)) { return; }
      else
      {
        if (saveName != Collection.Name && await CollectionRepository.Exists(saveName))
        {
          // Collection exists already
          if (await Dialogs.GetOverrideDialog(saveName).ShowAsync() == null) { return; }
        }
      }

      await SaveCollection(saveName);
    }

    /// <summary>
    /// Deletes the current <see cref="Collection"/> from the database
    /// </summary>
    [RelayCommand(CanExecute = nameof(DeleteCollectionDialogCommandCanExecute))]
    public async Task DeleteCollectionDialog()
    {
      if (!await CollectionRepository.Exists(Collection.Name)) { return; }
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
      if(await Dialogs.GetNewCollectionListDialog().ShowAsync() is MTGCardCollectionList newList)
      {
        NewCollectionList(newList);
      }
    }

    /// <summary>
    /// Changes the <see cref="SelectedList"/>'s information to the given information
    /// </summary>
    [RelayCommand(CanExecute = nameof(EditCollectionListDialogCommandCanExecute))]
    public async Task EditCollectionListDialog()
    {
      if(SelectedList != null && await Dialogs.GetEditCollectionListDialog(SelectedList.Name, SelectedList.SearchQuery).ShowAsync() is MTGCardCollectionList updatedList)
      {
        UpdateSelectedCollectionList(updatedList);
      }
    }

    /// <summary>
    /// Changes <see cref="SelectedList"/> to the given <paramref name="list"/>
    /// </summary>
    [RelayCommand]
    public void ChangeSelectedCollectionList(MTGCardCollectionList list)
    {
      SelectedList = list;
    }

    /// <summary>
    /// Removes the <see cref="SelectedList"/> from the <see cref="Collection"/>
    /// </summary>
    [RelayCommand(CanExecute = nameof(DeleteCollectionListDialogCommandCanExecute))]
    public async Task DeleteCollectionListDialog()
    {
      if(SelectedList == null) { return; }
      else if (await Dialogs.GetDeleteListDialog(SelectedList.Name).ShowAsync() is true)
      {
        DeleteSelectedCollectionList();
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
      var prints = (await CardAPI.FetchCardsWithParameters($"!\"{card.Info.Name}\"+unique:prints+game:paper")).Where(x => x.Info.FrontFace.IllustrationId == card.Info.FrontFace.IllustrationId).ToArray();
      var printViewModels = prints.Select(x => new MTGCardViewModel(x)).ToArray();
      IsBusy = false;

      await Dialogs.GetCardPrintDialog(printViewModels).ShowAsync();
    }

    private void NewCollection()
    {
      Collection = new MTGCardCollection();
    }
    private async Task SaveCollection(string name)
    {
      IsBusy = true;
      var saveCollection = new MTGCardCollection()
      {
        Name = name,
        CollectionLists = Collection.CollectionLists,
      };
      if(await Task.Run(() => CollectionRepository.AddOrUpdate(saveCollection)))
      {
        Collection.Name = name;
        HasUnsavedChanges = false;
        Notifications.RaiseNotification(Notifications.NotificationType.Success, "The collection was saved successfully.");
      }
      else { Notifications.RaiseNotification(Notifications.NotificationType.Error, "Error. Could not save the collection."); }
      IsBusy = false;
    }
    private async Task LoadCollection(string name)
    {
      IsBusy = true;
      if (await Task.Run(() => CollectionRepository.Get(name)) is MTGCardCollection loadedCollection)
      {
        Collection = loadedCollection;
        if(Collection.CollectionLists.Count > 0) { ChangeSelectedCollectionList(Collection.CollectionLists[0]); }
        Notifications.RaiseNotification(Notifications.NotificationType.Success, "The collection was loaded successfully.");
      }
      else { Notifications.RaiseNotification(Notifications.NotificationType.Error, "Error. Could not load the collection."); }
      IsBusy = false;
    }
    private async Task DeleteCollection()
    {
      IsBusy = true;
      if (await Task.Run(() => CollectionRepository.Remove(Collection)))
      {
        Collection = new();
        HasUnsavedChanges = false;
        Notifications.RaiseNotification(Notifications.NotificationType.Success, "The collection was deleted successfully.");
      }
      else { Notifications.RaiseNotification(Notifications.NotificationType.Error, "Error. Could not delete the collection."); }
      IsBusy = false;
    }
    private void NewCollectionList(MTGCardCollectionList newList)
    {
      if (Collection.CollectionLists.FirstOrDefault(x => x.Name == newList.Name) == null)
      {
        Collection.CollectionLists.Add(newList);
        ChangeSelectedCollectionList(newList);
        HasUnsavedChanges = true;
        Notifications.RaiseNotification(Notifications.NotificationType.Success, "List added to the collection successfully.");
      }
      else { Notifications.RaiseNotification(Notifications.NotificationType.Error, "Error. List already exists in the collection."); }
    }
    private void UpdateSelectedCollectionList(MTGCardCollectionList updatedList)
    {
      if(SelectedList == null) { return; }
      SelectedList.Name = updatedList.Name;
      SelectedList.SearchQuery = updatedList.SearchQuery;
      MTGSearchViewModel.SearchQuery = updatedList.SearchQuery;
      HasUnsavedChanges = true;
    }
    private void DeleteSelectedCollectionList()
    {
      if (Collection.CollectionLists.Remove(SelectedList))
      {
        SelectedList = Collection.CollectionLists.Count > 0 ? Collection.CollectionLists[0] : null;
        HasUnsavedChanges = true;
        Notifications.RaiseNotification(Notifications.NotificationType.Success, "The list was deleted successfully.");
      }
      else { Notifications.RaiseNotification(Notifications.NotificationType.Error, "Error. Could not delete the list."); }
    }

    /// <summary>
    /// Asks user if they want to save the collections's unsaved changes.
    /// </summary>
    /// <returns><see langword="true"/>, if user does not cancel any of the dialogs</returns>
    public async Task<bool> ShowUnsavedDialogs()
    {
      if (HasUnsavedChanges)
      {
        // Collection has unsaved changes
        var wantSaveConfirmed = await Dialogs.GetSaveUnsavedDialog().ShowAsync();
        if (wantSaveConfirmed == null) { return false; }
        else if (wantSaveConfirmed is true)
        {
          // User wants to save the unsaved changes
          var saveName = await Dialogs.GetSaveDialog(Collection.Name).ShowAsync();
          if (string.IsNullOrEmpty(saveName)) { return false; }
          else
          {
            if (saveName != Collection.Name && await CollectionRepository.Exists(saveName))
            {
              // Collection exists already
              var overrideConfirmed = await Dialogs.GetOverrideDialog(saveName).ShowAsync();
              if (overrideConfirmed == null) { return false; }
              else if (overrideConfirmed is true)
              {
                // User wants to override the colleciton
                await SaveCollection(saveName);
              }
            }
            else { await SaveCollection(saveName); }
          }
        }
      }
      return true;
    }
    public async Task<bool> SaveUnsavedChanges()
    {
      return await ShowUnsavedDialogs();
    }

    private bool SaveCollectionDialogCommandCanExecute() => Collection.CollectionLists.Count > 0;
    private bool DeleteCollectionDialogCommandCanExecute() => !string.IsNullOrEmpty(Collection.Name);
    private bool EditCollectionListDialogCommandCanExecute() => SelectedList != null;
    private bool DeleteCollectionListDialogCommandCanExecute() => SelectedList != null;
  }
}

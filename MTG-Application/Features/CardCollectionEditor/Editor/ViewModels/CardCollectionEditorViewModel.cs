using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.Features.CardCollectionEditor.Editor.Services.Factories;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.ViewModels;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static MTGApplication.Features.CardCollection.Editor.UseCases.CardCollectionEditorViewModelCommands;
using static MTGApplication.Features.CardCollectionEditor.CardCollectionList.UseCases.CardCollectionEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollection.Editor.ViewModels;

public partial class CardCollectionEditorViewModel(IMTGCardImporter importer) : ObservableObject, ISavable, IWorker
{
  [NotNull]
  public CardCollectionEditorCardCollection? Collection
  {
    get => field ??= Collection = new();
    set
    {
      if (field == value)
        return;

      // Visual state trigger will not work if the OnPropertyChanged(nameof(CollectionName))
      //  is called when the old name was the same as the new name.
      var nameChanged = field?.Name != value?.Name;

      if (field != null)
      {
        field.PropertyChanged -= CardCollection_PropertyChanged;
        field.CollectionLists.CollectionChanged -= CollectionLists_CollectionChanged;

        foreach (var list in field.CollectionLists)
        {
          list.PropertyChanged -= CollectionList_PropertyChanged;
          list.Cards.CollectionChanged -= CollectionListCards_CollectionChanged;

          foreach (var card in list.Cards)
            card.PropertyChanged -= CollectionListCardsItem_PropertyChanged;
        }
      }

      SetProperty(ref field, value);

      if (field != null)
      {
        field.PropertyChanged += CardCollection_PropertyChanged;
        field.CollectionLists.CollectionChanged += CollectionLists_CollectionChanged;

        foreach (var list in field.CollectionLists)
        {
          list.PropertyChanged += CollectionList_PropertyChanged;
          list.Cards.CollectionChanged += CollectionListCards_CollectionChanged;

          foreach (var card in list.Cards)
            card.PropertyChanged += CollectionListCardsItem_PropertyChanged;
        }
      }

      SelectedCardCollectionListViewModel.CollectionList = Collection.CollectionLists.FirstOrDefault() ?? new();

      if (nameChanged)
        OnPropertyChanged(nameof(CollectionName));
    }
  }

  public IMTGCardImporter Importer { get; } = importer;

  public CardCollectionEditorConfirmers Confirmers { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public IRepository<MTGCardCollectionDTO> Repository { get; init; } = new CardCollectionDTORepository();
  public ClipboardService ClipboardService { get; init; } = new();

  [NotNull] public CardCollectionListViewModel? SelectedCardCollectionListViewModel => field ??= new CardCollectionFactory(this).CreateCardCollectionListViewModel();

  public string CollectionName
  {
    get => Collection.Name;
    set => Collection.Name = value;
  }

  [ObservableProperty] public partial bool IsBusy { get; set; }
  [ObservableProperty] public partial bool HasUnsavedChanges { get; set; }

  [NotNull] public IAsyncRelayCommand<ISavable.ConfirmArgs>? ConfirmUnsavedChangesCommand => field ??= new ConfirmUnsavedChanges(this).Command;
  [NotNull] public IAsyncRelayCommand? NewCollectionCommand => field ??= new ConfirmNewCollection(this).Command;
  [NotNull] public IAsyncRelayCommand? OpenCollectionCommand => field ??= new ConfirmOpenCollection(this).Command;
  [NotNull] public IAsyncRelayCommand? SaveCollectionCommand => field ??= new ConfirmSaveCollection(this).Command;
  [NotNull] public IAsyncRelayCommand? DeleteCollectionCommand => field ??= new ConfirmDeleteCollection(this).Command;
  [NotNull] public IAsyncRelayCommand? NewListCommand => field ??= new ConfirmNewList(this).Command;
  [NotNull] public IAsyncRelayCommand<MTGCardCollectionList>? DeleteListCommand => field ??= new ConfirmDeleteList(this).Command;
  [NotNull] public IRelayCommand<MTGCardCollectionList>? ChangeListCommand => field ??= new ChangeList(this).Command;
  [NotNull] public IAsyncRelayCommand<MTGCardCollectionList>? EditListCommand => field ??= new EditList(this).Command;
  [NotNull] public IAsyncRelayCommand<CardCollectionMTGCard>? ShowCardPrintsCommand => field ??= new ShowCardPrints(this).Command;

  private void CardCollection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(CardCollectionEditorCardCollection.Name))
      OnPropertyChanged(nameof(CollectionName));
  }

  private void CollectionLists_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
    {
      foreach (var item in e.NewItems.OfType<MTGCardCollectionList>())
      {
        item.PropertyChanged += CollectionList_PropertyChanged;
        item.Cards.CollectionChanged += CollectionListCards_CollectionChanged;

        foreach (var card in item.Cards)
          card.PropertyChanged += CollectionListCardsItem_PropertyChanged;
      }

      SelectedCardCollectionListViewModel.CollectionList = e.NewItems.OfType<MTGCardCollectionList>().LastOrDefault() ?? new();
    }
    else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
    {
      foreach (var item in e.OldItems.OfType<MTGCardCollectionList>())
      {
        item.PropertyChanged -= CollectionList_PropertyChanged;
        item.Cards.CollectionChanged -= CollectionListCards_CollectionChanged;

        foreach (var card in item.Cards)
          card.PropertyChanged -= CollectionListCardsItem_PropertyChanged;

        if (SelectedCardCollectionListViewModel.CollectionList == item)
          SelectedCardCollectionListViewModel.CollectionList = Collection.CollectionLists.FirstOrDefault() ?? new();
      }
    }
  }

  private void CollectionList_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    => HasUnsavedChanges = true;

  private void CollectionListCards_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
    {
      foreach (var item in e.NewItems.OfType<MTGCardCollectionList>())
        item.PropertyChanged += CollectionListCardsItem_PropertyChanged;
    }
    else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
    {
      foreach (var item in e.OldItems.OfType<MTGCardCollectionList>())
        item.PropertyChanged -= CollectionListCardsItem_PropertyChanged;
    }

    HasUnsavedChanges = true;
  }

  private void CollectionListCardsItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    => HasUnsavedChanges = true;
}
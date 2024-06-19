using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollection.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.CardCollection.Editor.UseCases.CardCollectionEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollection.Editor.ViewModels;

public partial class CardCollectionEditorViewModel : ViewModelBase, ISavable, IWorker
{
  public CardCollectionEditorViewModel(MTGCardImporter importer, CardCollectionEditorConfirmers confirmers, Notifier notifier, IRepository<MTGCardCollectionDTO> repository, ClipboardService clipboardService)
  {
    Importer = importer;
    Confirmers = confirmers;
    Notifier = notifier;
    Repository = repository;
    ClipboardService = clipboardService;

    PropertyChanged += CardCollectionEditorViewModel_PropertyChanged;

    CardCollectionViewModel = CreateCardCollectionViewModel(new());
    CardCollectionListViewModel = CreateCardCollectionListViewModel(new());
  }

  public MTGCardImporter Importer { get; }
  public CardCollectionEditorConfirmers Confirmers { get; }
  public Notifier Notifier { get; }
  public IRepository<MTGCardCollectionDTO> Repository { get; }
  public ClipboardService ClipboardService { get; }

  public IWorker Worker => this;
  public CardCollectionViewModel CardCollectionViewModel
  {
    get => cardCollectionViewModel;
    private set => SetProperty(ref cardCollectionViewModel, value);
  }
  public CardCollectionListViewModel CardCollectionListViewModel
  {
    get => cardCollectionListViewModel;
    private set => SetProperty(ref cardCollectionListViewModel, value);
  }
  public bool HasUnsavedChanges
  {
    get => CardCollectionViewModel.HasUnsavedChanges || cardCollectionListViewModel.HasUnsavedChanges;
    set
    {
      CardCollectionViewModel.HasUnsavedChanges = value;
      CardCollectionListViewModel.HasUnsavedChanges = value;

      OnPropertyChanged(nameof(HasUnsavedChanges));
    }
  }
  public bool IsBusy
  {
    get => isBusy || CardCollectionViewModel.IsBusy || CardCollectionListViewModel.IsBusy;
    set => SetProperty(ref isBusy, value);
  }

  [ObservableProperty] private MTGCardCollectionList selectedCardCollectionList = new();
  // Used to bind collection name to visual state trigger, so it does not update when CardCollectionViewModel changes with the same name
  [ObservableProperty] private string collectionName;

  public IAsyncRelayCommand<ISavable.ConfirmArgs> ConfirmUnsavedChangesCommand => (confirmUnsavedChanges ??= new ConfirmUnsavedChanges(this)).Command;
  public IAsyncRelayCommand NewCollectionCommand => (newCollection ??= new ConfirmNewCollection(this)).Command;
  public IAsyncRelayCommand OpenCollectionCommand => (openCollection ??= new ConfirmOpenCollection(this)).Command;
  public IAsyncRelayCommand<MTGCardCollectionList> ChangeListCommand => (changeList ??= new ChangeList(this)).Command;

  private bool isBusy;
  private CardCollectionViewModel cardCollectionViewModel;
  private CardCollectionListViewModel cardCollectionListViewModel;
  private ConfirmUnsavedChanges confirmUnsavedChanges;
  private ConfirmNewCollection newCollection;
  private ConfirmOpenCollection openCollection;
  private ChangeList changeList;

  public async Task ChangeCollection(MTGCardCollection collection)
  {
    CardCollectionViewModel = CreateCardCollectionViewModel(collection);
    await ChangeCollectionList(collection.CollectionLists.FirstOrDefault());
  }

  public async Task ChangeCollectionList(MTGCardCollectionList list)
  {
    if (!ChangeListCommand.CanExecute(list)) return;

    SelectedCardCollectionList = list ?? new();
    CardCollectionListViewModel = CreateCardCollectionListViewModel(list);

    await CardCollectionListViewModel.UpdateQueryCards();
  }

  private CardCollectionViewModel CreateCardCollectionViewModel(MTGCardCollection model)
  {
    var viewmodel = new CardCollectionViewModel.Factory()
    {
      Confirmers = Confirmers.CardCollectionConfirmers,
      Notifier = Notifier,
      Repository = Repository,
      OnDeleted = async () => await ChangeCollection(new()),
      OnListAdded = ChangeCollectionList,
      OnListRemoved = async (list) => { if (SelectedCardCollectionList == list) await ChangeCollectionList(CardCollectionViewModel.CollectionLists.FirstOrDefault()); }
    }.Build(model, Importer);

    viewmodel.PropertyChanged += (_, e) =>
    {
      switch (e.PropertyName)
      {
        case nameof(CardCollectionViewModel.Name): CollectionName = CardCollectionViewModel.Name; break;
        case nameof(ISavable.HasUnsavedChanges): HasUnsavedChanges = viewmodel.HasUnsavedChanges; break;
        case nameof(IWorker.IsBusy): OnPropertyChanged(nameof(IsBusy)); break;
      }
    };

    return viewmodel;
  }

  private CardCollectionListViewModel CreateCardCollectionListViewModel(MTGCardCollectionList model)
  {
    var viewmodel = new CardCollectionListViewModel(
      model,
      Importer,
      existsValidation: (name) => CardCollectionViewModel.CollectionLists.FirstOrDefault(x => x.Name == name) is not null)
    {
      Confirmers = Confirmers.CardCollectionListConfirmers,
      Notifier = Notifier,
      ClipboardService = ClipboardService,
    };

    viewmodel.PropertyChanged += (_, e) =>
    {
      switch (e.PropertyName)
      {
        case nameof(ISavable.HasUnsavedChanges): HasUnsavedChanges = viewmodel.HasUnsavedChanges; break;
        case nameof(IWorker.IsBusy): OnPropertyChanged(nameof(IsBusy)); break;
      }
    };

    return viewmodel;
  }

  private void CardCollectionEditorViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(CardCollectionViewModel): CollectionName = CardCollectionViewModel.Name; break;
    }
  }
}
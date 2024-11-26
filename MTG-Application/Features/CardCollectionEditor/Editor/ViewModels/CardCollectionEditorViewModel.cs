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

public partial class CardCollectionEditorViewModel : ObservableObject, ISavable, IWorker
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

  [ObservableProperty] public partial CardCollectionViewModel CardCollectionViewModel { get; set; }
  [ObservableProperty] public partial CardCollectionListViewModel CardCollectionListViewModel { get; set; }
  public IWorker Worker => this;
  public bool HasUnsavedChanges
  {
    get => CardCollectionViewModel.HasUnsavedChanges || CardCollectionListViewModel.HasUnsavedChanges;
    set
    {
      if (CardCollectionViewModel.HasUnsavedChanges != value || CardCollectionListViewModel.HasUnsavedChanges != value)
      {
        CardCollectionViewModel.HasUnsavedChanges = value;
        CardCollectionListViewModel.HasUnsavedChanges = value;

        OnPropertyChanged(nameof(HasUnsavedChanges));
      }
    }
  }
  public bool IsBusy
  {
    get => field || CardCollectionViewModel.IsBusy || CardCollectionListViewModel.IsBusy;
    set => SetProperty(ref field, value);
  }

  [ObservableProperty] public partial MTGCardCollectionList SelectedCardCollectionList { get; set; } = new();
  // Used to bind collection name to visual state trigger, so it does not update when CardCollectionViewModel changes with the same name
  [ObservableProperty] public partial string CollectionName { get; set; } = string.Empty;

  public IAsyncRelayCommand<ISavable.ConfirmArgs> ConfirmUnsavedChangesCommand => field ??= (new ConfirmUnsavedChanges(this).Command);
  public IAsyncRelayCommand NewCollectionCommand => field ??= (new ConfirmNewCollection(this).Command);
  public IAsyncRelayCommand OpenCollectionCommand => field ??= (new ConfirmOpenCollection(this).Command);
  public IAsyncRelayCommand<MTGCardCollectionList> ChangeListCommand => field ??= (new ChangeList(this).Command);

  public async Task ChangeCollection(MTGCardCollection collection)
  {
    CardCollectionViewModel = CreateCardCollectionViewModel(collection);
    HasUnsavedChanges = false;

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
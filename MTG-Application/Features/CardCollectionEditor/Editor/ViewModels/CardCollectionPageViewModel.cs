using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollection.Services;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.CardCollection.UseCases.CardCollectionPageViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionEditorViewModel : ViewModelBase, ISavable, IWorker
{
  public CardCollectionEditorViewModel(MTGCardImporter importer)
  {
    Importer = importer;

    CardCollectionViewModel = CreateCardCollectionViewModel(new());
    CardCollectionListViewModel = new(model: new(), importer);

    PropertyChanged += CardCollectionPageViewModel_PropertyChanged;
  }

  public CardCollectionEditorConfirmers Confirmers { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public IRepository<MTGCardCollectionDTO> Repository { get; init; } = new CardCollectionDTORepository();
  public ClipboardService ClipboardService { get; init; } = new();
  public MTGCardImporter Importer { get; }
  public IWorker Worker => this;

  [ObservableProperty] private CardCollectionViewModel cardCollectionViewModel;
  [ObservableProperty] private CardCollectionListViewModel cardCollectionListViewModel;
  [ObservableProperty] private MTGCardCollection selectedCardCollection = new();
  [ObservableProperty] private MTGCardCollectionList selectedCardCollectionList = new();
  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public IAsyncRelayCommand<ISavable.ConfirmArgs> ConfirmUnsavedChangesCommand => (confirmUnsavedChanges ??= new ConfirmUnsavedChanges(this)).Command;
  public IAsyncRelayCommand NewCollectionCommand => (newCollection ??= new ConfirmNewCollection(this)).Command;
  public IAsyncRelayCommand OpenCollectionCommand => (openCollection ??= new ConfirmOpenCollection(this)).Command;

  private ConfirmUnsavedChanges confirmUnsavedChanges;
  private ConfirmNewCollection newCollection;
  private ConfirmOpenCollection openCollection;

  private async void CardCollectionPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(SelectedCardCollection):
        CardCollectionViewModel = CreateCardCollectionViewModel(SelectedCardCollection);
        SelectedCardCollectionList = SelectedCardCollection.CollectionLists.FirstOrDefault();
        break;
      case nameof(SelectedCardCollectionList):
        CardCollectionListViewModel = await CreateCardCollectionListViewModel(SelectedCardCollectionList);
        break;
    }
  }

  private CardCollectionViewModel CreateCardCollectionViewModel(MTGCardCollection model)
  {
    var viewmodel = new CardCollectionViewModel.Factory()
    {
      Confirmers = Confirmers.CardCollectionConfirmers,
      Notifier = Notifier,
      Repository = Repository,
      OnDeleted = () => SelectedCardCollection = new(),
      OnListAdded = (list) => SelectedCardCollectionList = list,
      OnListRemoved = (list) => { if (SelectedCardCollectionList == list) SelectedCardCollectionList = SelectedCardCollection.CollectionLists.FirstOrDefault(); }
    }.Build(model, Importer);

    viewmodel.PropertyChanged += (_, e) =>
    {
      switch (e.PropertyName)
      {
        case nameof(ISavable.HasUnsavedChanges): HasUnsavedChanges = viewmodel.HasUnsavedChanges; break;
        case nameof(IWorker.IsBusy): IsBusy = viewmodel.IsBusy; break;
      }
    };

    return viewmodel;
  }

  private async Task<CardCollectionListViewModel> CreateCardCollectionListViewModel(MTGCardCollectionList model)
  {
    var viewmodel = await Worker.DoWork(new CardCollectionListViewModel.Factory()
    {
      Confirmers = Confirmers.CardCollectionListConfirmers,
      Notifier = Notifier,
      ClipboardService = ClipboardService,
    }.Build(
      model: model ?? new(),
      importer: Importer,
      nameValidation: (name) => !string.IsNullOrEmpty(name) && SelectedCardCollection.CollectionLists.FirstOrDefault(x => x.Name == name) is not null));

    viewmodel.PropertyChanged += (_, e) =>
    {
      switch (e.PropertyName)
      {
        case nameof(ISavable.HasUnsavedChanges): HasUnsavedChanges = viewmodel.HasUnsavedChanges; break;
        case nameof(IWorker.IsBusy): IsBusy = viewmodel.IsBusy; break;
      }
    };

    return viewmodel;
  }
}
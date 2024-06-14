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
using static MTGApplication.Features.CardCollection.UseCases.CardCollectionViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionViewModel(MTGCardImporter importer) : ViewModelBase, IWorker, ISavable
{
  public QueryCardsViewModel QueryCardsViewModel { get; } = new(importer);
  public CardCollectionConfirmers Confirmers { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public ClipboardService ClipboardService { get; init; } = new();
  public IRepository<MTGCardCollectionDTO> Repository { get; init; } = new CardCollectionDTORepository();
  public MTGCardImporter Importer { get; } = importer;

  [ObservableProperty] private MTGCardCollection collection = new();
  [ObservableProperty] private MTGCardCollectionList selectedList;
  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public int SelectedListCardCount => SelectedList?.Cards.Count ?? 0;
  public IWorker Worker => this;

  public IAsyncRelayCommand<ISavable.ConfirmArgs> ConfirmUnsavedChangesCommand => (confirmUnsavedChanges ??= new ConfirmUnsavedChanges(this)).Command;
  public IAsyncRelayCommand NewCollectionCommand => (newCollection ??= new NewCollection(this)).Command;
  public IAsyncRelayCommand OpenCollectionCommand => (openCollection ??= new OpenCollection(this)).Command;
  public IAsyncRelayCommand SaveCollectionCommand => (saveCollection ??= new SaveCollection(this)).Command;
  public IAsyncRelayCommand DeleteCollectionCommand => (deleteCollection ??= new DeleteCollection(this)).Command;
  public IAsyncRelayCommand NewListCommand => (newList ??= new NewList(this)).Command;
  public IAsyncRelayCommand EditListCommand => (editList ??= new EditList(this)).Command;
  public IAsyncRelayCommand DeleteListCommand => (deleteList ??= new DeleteList(this)).Command;
  public IAsyncRelayCommand ImportCardsCommand => (importCards ??= new ImportCards(this)).Command;
  public IAsyncRelayCommand ExportCardsCommand => (exportCards ??= new ExportCards(this)).Command;
  public IAsyncRelayCommand<MTGCardCollectionList> SelectListCommand => (selectList ??= new SelectList(this)).Command;
  public IAsyncRelayCommand<CardCollectionMTGCard> ShowCardPrintsCommand => (showCardPrints ??= new ShowCardPrints(this)).Command;
  public IRelayCommand<CardCollectionMTGCard> SwitchCardOwnershipCommand => (switchCardOwnership ??= new SwitchCardOwnership(this)).Command;

  private ConfirmUnsavedChanges confirmUnsavedChanges;
  private NewCollection newCollection;
  private OpenCollection openCollection;
  private SaveCollection saveCollection;
  private DeleteCollection deleteCollection;
  private NewList newList;
  private EditList editList;
  private DeleteList deleteList;
  private ImportCards importCards;
  private ExportCards exportCards;
  private SelectList selectList;
  private ShowCardPrints showCardPrints;
  private SwitchCardOwnership switchCardOwnership;

  public async Task SetCollection(MTGCardCollection collection)
  {
    Collection = collection;
    HasUnsavedChanges = false;

    await SelectListCommand.ExecuteAsync(Collection.CollectionLists.FirstOrDefault());
  }
}
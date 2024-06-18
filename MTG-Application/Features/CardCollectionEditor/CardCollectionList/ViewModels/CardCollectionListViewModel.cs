using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollection.Services;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static MTGApplication.Features.CardCollection.UseCases.CardCollectionEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionListViewModel : ObservableObject, ISavable, IWorker
{
  public CardCollectionListViewModel(MTGCardCollectionList model, MTGCardImporter importer, Func<string, bool> existsValidation = null)
  {
    Model = model ?? new();

    Importer = importer;
    ExistsValidation = existsValidation;

    QueryCardsViewModel = new(OwnedCards, importer);
  }

  public MTGCardImporter Importer { get; }
  public QueryCardsViewModel QueryCardsViewModel { get; }
  public CardCollectionListConfirmers Confirmers { get; set; } = new();
  public Notifier Notifier { get; set; } = new();
  public ClipboardService ClipboardService { get; set; } = new();
  public IWorker Worker => this;

  public string Name
  {
    get => Model.Name;
    set
    {
      if (Model.Name == value) return;

      Model.Name = value;
      OnPropertyChanged(nameof(Name));
    }
  }
  public string Query
  {
    get => Model.SearchQuery;
    private set
    {
      if (Model.SearchQuery == value) return;

      Model.SearchQuery = value;
      OnPropertyChanged(nameof(Query));
    }
  }
  public ObservableCollection<CardCollectionMTGCard> OwnedCards => Model.Cards;

  [ObservableProperty] private bool hasUnsavedChanges;
  [ObservableProperty] private bool isBusy;

  public Func<string, bool> ExistsValidation { get; }

  private MTGCardCollectionList Model { get; }

  public IAsyncRelayCommand EditListCommand => (editList ??= new EditList(this)).Command;
  public IAsyncRelayCommand ImportCardsCommand => (importCards ??= new ImportCards(this)).Command;
  public IAsyncRelayCommand ExportCardsCommand => (exportCards ??= new ExportCards(this)).Command;
  public IAsyncRelayCommand<CardCollectionMTGCard> ShowCardPrintsCommand => (showCardPrints ??= new ShowCardPrints(this)).Command;
  public IRelayCommand<CardCollectionMTGCard> SwitchCardOwnershipCommand => (switchCardOwnership ??= new SwitchCardOwnership(this)).Command;

  private EditList editList;
  private ImportCards importCards;
  private ExportCards exportCards;
  private ShowCardPrints showCardPrints;
  private SwitchCardOwnership switchCardOwnership;

  public async Task UpdateQueryCards() => await Worker.DoWork(QueryCardsViewModel.UpdateQueryCards(Model.SearchQuery));

  public async Task UpdateQuery(string query)
  {
    if (Query == query) return;

    Query = query;

    await UpdateQueryCards();
  }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Collections;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.ViewModels;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.CardCollectionEditor.CardCollectionList.UseCases.CardCollectionEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;

public partial class CardCollectionListViewModel : ObservableObject
{
  public CardCollectionListViewModel(IMTGCardImporter importer)
  {
    Importer = importer;

    QueryCards = new(new CardCollectionIncrementalCardSource(importer));

    QueryCards.Collection.CollectionChanged += QueryCards_CollectionChanged;
    QueryCards.PropertyChanged += QueryCards_PropertyChanged;
  }

  [NotNull]
  public MTGCardCollectionList? CollectionList
  {
    get => field ??= CollectionList = new();
    set
    {
      if (field == value)
        return;

      if (field != null)
      {
        field.PropertyChanged -= CollectionList_PropertyChanged;
        field.Cards.CollectionChanged -= Cards_CollectionChanged;
      }

      SetProperty(ref field, value);

      if (field != null)
      {
        field.PropertyChanged += CollectionList_PropertyChanged;
        field.Cards.CollectionChanged += Cards_CollectionChanged;
      }

      OnPropertyChanged(nameof(Name));
      OnPropertyChanged(nameof(Query));
      OnPropertyChanged(nameof(OwnedCount));
      OnPropertyChanged(nameof(TotalCount));

      UpdateCards();
    }
  }

  public IMTGCardImporter Importer { get; }
  public CardCollectionListConfirmers Confirmers { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public ClipboardService ClipboardService { get; init; } = new();
  public IWorker Worker { get; init; } = IWorker.Default;

  public string Name
  {
    get => CollectionList.Name;
    set => CollectionList.Name = value;
  }
  public string Query
  {
    get => CollectionList.SearchQuery;
    set => CollectionList.SearchQuery = value;
  }
  public int OwnedCount => CollectionList.Cards.Count;
  public int TotalCount => QueryCards.TotalCardCount;

  public IncrementalLoadingCollection<IncrementalCardSource<CardCollectionMTGCard>, CardCollectionMTGCard> Cards => QueryCards.Collection;

  private IncrementalLoadingCardCollection<CardCollectionMTGCard> QueryCards { get; }
  private NotifyTaskCompletion<CardImportResult>? FetchingCardsTask { get; set; }
  private Task? UpdatingCardsTask { get; set; }

  [NotNull] public IAsyncRelayCommand? ImportCardsCommand => field ??= new ImportCards(this).Command;
  [NotNull] public IAsyncRelayCommand? ExportCardsCommand => field ??= new ExportCards(this).Command;
  [NotNull] public IRelayCommand<CardCollectionMTGCard>? SwitchCardOwnershipCommand => field ??= new SwitchCardOwnership(this).Command;

  public async Task WaitForCardUpdate()
  {
    if (FetchingCardsTask != null)
      await FetchingCardsTask.Task;

    if (UpdatingCardsTask != null)
      await UpdatingCardsTask;
  }

  private void UpdateCards()
  {
    if (FetchingCardsTask != null)
      FetchingCardsTask.PropertyChanged -= UpdatingCardsTask_PropertyChanged;

    FetchingCardsTask = new(Worker.DoWork(Task.Run(async () => await new FetchCardsWithSearchQuery(Importer).Execute(Query))));
    FetchingCardsTask.PropertyChanged += UpdatingCardsTask_PropertyChanged;
    FetchingCardsTask.Start();
  }

  private void UpdatingCardsTask_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(FetchingCardsTask.Result))
    {
      if (FetchingCardsTask?.Result == null)
        return;

      var cards = FetchingCardsTask.Result.Found.Select(x => new CardCollectionMTGCard(x.Info)
      {
        IsOwned = CollectionList.Cards.Any(clc => clc.Info.ScryfallId == clc.Info.ScryfallId),
      }).ToList();

      UpdatingCardsTask = Worker.DoWork(QueryCards.SetCollection(
        cards: cards,
        nextPageUri: FetchingCardsTask.Result.NextPageUri,
        totalCount: FetchingCardsTask.Result.TotalCount));
    }
  }

  private void CollectionList_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(MTGCardCollectionList.Name))
      OnPropertyChanged(nameof(Name));
    else if (e.PropertyName == nameof(MTGCardCollectionList.SearchQuery))
    {
      UpdateCards();

      OnPropertyChanged(nameof(Query));
    }
  }

  private void Cards_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
    {
      foreach (var item in e.NewItems.OfType<CardCollectionMTGCard>())
        if (Cards.FirstOrDefault(x => x.Info.ScryfallId == item.Info.ScryfallId) is CardCollectionMTGCard card)
          card.IsOwned = true;
    }
    else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
    {
      foreach (var item in e.OldItems.OfType<CardCollectionMTGCard>())
        if (Cards.FirstOrDefault(x => x.Info.ScryfallId == item.Info.ScryfallId) is CardCollectionMTGCard card)
          card.IsOwned = false;
    }

    OnPropertyChanged(nameof(OwnedCount));
  }

  private void QueryCards_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(QueryCards.TotalCardCount))
      OnPropertyChanged(nameof(TotalCount));
  }

  private void QueryCards_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
    {
      foreach (var item in e.NewItems.OfType<CardCollectionMTGCard>())
      {
        item.IsOwned = CollectionList.Cards
          .FirstOrDefault(x => x.Info.ScryfallId == item.Info.ScryfallId) != null;
      }
    }

    OnPropertyChanged(nameof(TotalCount));
  }
}
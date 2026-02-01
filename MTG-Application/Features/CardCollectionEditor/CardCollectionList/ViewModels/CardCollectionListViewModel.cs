using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.Services.IOServices;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.ObjectModel;
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
    private set
    {
      if (field == value)
        return;

      if (field != null)
        field.Cards.CollectionChanged -= Cards_CollectionChanged;

      SetProperty(ref field, value);

      if (field != null)
        field.Cards.CollectionChanged += Cards_CollectionChanged;

      OnPropertyChanged(nameof(Name));
      OnPropertyChanged(nameof(Query));
      OnPropertyChanged(nameof(OwnedCount));
      OnPropertyChanged(nameof(TotalCount));
    }
  }

  public IMTGCardImporter Importer { get; }
  public CardCollectionListConfirmers Confirmers { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public ClipboardService ClipboardService { get; init; } = new();
  public Worker Worker { get; init; } = new();

  public Func<string, bool> NameValidator { get; init; } = (_) => true;

  public string Name
  {
    get => CollectionList.Name;
    set
    {
      CollectionList.Name = value;
      OnPropertyChanged();
    }
  }
  public string Query
  {
    get => CollectionList.SearchQuery;
    set
    {
      CollectionList.SearchQuery = value;
      OnPropertyChanged();
    }
  }
  public int OwnedCount => CollectionList.Cards.Count;
  public int TotalCount => QueryCards.TotalCardCount;
  public ObservableCollection<CardCollectionMTGCard> Cards => CollectionList.Cards;

  public IncrementalLoadingCardCollection<CardCollectionMTGCard> QueryCards { get; }

  [NotNull] public IAsyncRelayCommand? ImportCardsCommand => field ??= new ImportCards(this).Command;
  [NotNull] public IAsyncRelayCommand? ExportCardsCommand => field ??= new ExportCards(this).Command;
  [NotNull] public IAsyncRelayCommand? EditListCommand => field ??= new EditList(this).Command;
  [NotNull] public IRelayCommand<CardCollectionMTGCard>? SwitchCardOwnershipCommand => field ??= new SwitchCardOwnership(this).Command;

  public async Task ChangeCollectionList(MTGCardCollectionList list)
  {
    CollectionList = list;

    // Refresh cards
    if ((await new FetchCardsWithSearchQuery(Importer).Execute(Query)) is not CardImportResult fetchResult)
      return;

    var cards = fetchResult.Found.Select(x => new CardCollectionMTGCard(x.Info)
    {
      IsOwned = CollectionList.Cards.Any(clc => clc.Info.ScryfallId == clc.Info.ScryfallId),
    }).ToList();

    await QueryCards.SetCollection(
      cards: cards,
      nextPageUri: fetchResult.NextPageUri,
      totalCount: fetchResult.TotalCount);
  }

  private void Cards_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
    {
      foreach (var item in e.NewItems.OfType<CardCollectionMTGCard>())
        if (QueryCards.Collection.FirstOrDefault(x => x.Info.ScryfallId == item.Info.ScryfallId) is CardCollectionMTGCard card)
          card.IsOwned = true;
    }
    else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
    {
      foreach (var item in e.OldItems.OfType<CardCollectionMTGCard>())
        if (QueryCards.Collection.FirstOrDefault(x => x.Info.ScryfallId == item.Info.ScryfallId) is CardCollectionMTGCard card)
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
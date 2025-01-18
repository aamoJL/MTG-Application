using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Collections;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;

public partial class QueryCardsViewModel : ObservableObject
{
  public QueryCardsViewModel(ObservableCollection<CardCollectionMTGCard> ownedCards, MTGCardImporter importer)
  {
    Importer = importer;
    OwnedCards = ownedCards;
    QueryCards = new(new CardCollectionIncrementalCardSource(importer));

    QueryCards.Collection.CollectionChanged += QueryCardsCollection_CollectionChanged;
    OwnedCards.CollectionChanged += OwnedCards_CollectionChanged;
  }

  public MTGCardImporter Importer { get; }

  private IncrementalLoadingCardCollection<CardCollectionMTGCard> QueryCards { get; }

  private ObservableCollection<CardCollectionMTGCard> OwnedCards { get; }

  public IncrementalLoadingCollection<IncrementalCardSource<CardCollectionMTGCard>, CardCollectionMTGCard> Collection => QueryCards.Collection;
  public int TotalCardCount => QueryCards.TotalCardCount;

  public async Task UpdateQueryCards(string query)
  {
    var searchResult = await new FetchCardsWithSearchQuery(Importer).Execute(query);
    await QueryCards.SetCollection([.. searchResult.Found.Select(x => new CardCollectionMTGCard(x.Info))], searchResult.NextPageUri, searchResult.TotalCount);

    OnPropertyChanged(nameof(TotalCardCount));
  }

  private void OwnedCards_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
        if (QueryCards.Collection.FirstOrDefault(x => x.Info.ScryfallId == (e.NewItems?[0] as MTGCard)?.Info.ScryfallId)
          is CardCollectionMTGCard existingNew)
          existingNew.IsOwned = true;
        break;
      case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
        if (QueryCards.Collection.FirstOrDefault(x => x.Info.ScryfallId == (e.OldItems?[0] as MTGCard)?.Info.ScryfallId)
          is CardCollectionMTGCard existingOld)
          existingOld.IsOwned = false;
        break;
    }
  }

  private void QueryCardsCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
        if (e.NewItems?[0] is CardCollectionMTGCard newCard)
          newCard.IsOwned = OwnedCards.FirstOrDefault(x => x.Info.ScryfallId == newCard.Info.ScryfallId) != null;
        break;
    }
  }
}
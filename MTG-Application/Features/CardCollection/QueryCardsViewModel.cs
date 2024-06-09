using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection;

public partial class QueryCardsViewModel : ObservableObject
{
  public QueryCardsViewModel(ICardImporter<DeckEditorMTGCard> cardAPI)
  {
    CardAPI = cardAPI;
    QueryCards = new(new CardCollectionIncrementalCardSource(cardAPI));

    QueryCards.Collection.CollectionChanged += QueryCardsCollection_CollectionChanged;
    PropertyChanging += QueryCardsViewModel_PropertyChanging;
    PropertyChanged += QueryCardsViewModel_PropertyChanged;
  }

  public ICardImporter<DeckEditorMTGCard> CardAPI { get; }
  
  private IncrementalLoadingCardCollection<CardCollectionMTGCard> QueryCards { get; }

  [ObservableProperty] private ObservableCollection<DeckEditorMTGCard> ownedCards = [];
  
  public IncrementalLoadingCollection<IncrementalCardSource<CardCollectionMTGCard>, CardCollectionMTGCard> Collection => QueryCards.Collection;
  public int TotalCardCount => QueryCards.TotalCardCount;

  public async Task UpdateQueryCards(string query)
  {
    var searchResult = await new GetMTGCardsBySearchQuery(CardAPI).Execute(query);
    QueryCards.SetCollection([.. searchResult.Found], searchResult.NextPageUri, searchResult.TotalCount);

    OnPropertyChanged(nameof(TotalCardCount));
  }

  private void QueryCardsViewModel_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
  {
    if (e.PropertyName == nameof(OwnedCards))
      OwnedCards.CollectionChanged -= OwnedCards_CollectionChanged;
  }

  private void QueryCardsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(OwnedCards))
      OwnedCards.CollectionChanged += OwnedCards_CollectionChanged;
  }

  private void OwnedCards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
        if (QueryCards.Collection.FirstOrDefault(x => x.Info.ScryfallId == (e.NewItems[0] as DeckEditorMTGCard).Info.ScryfallId)
          is CardCollectionMTGCard existingNew)
          existingNew.IsOwned = true;
        break;
      case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
        if (QueryCards.Collection.FirstOrDefault(x => x.Info.ScryfallId == (e.OldItems[0] as DeckEditorMTGCard).Info.ScryfallId)
          is CardCollectionMTGCard existingOld)
          existingOld.IsOwned = false;
        break;
    }
  }

  private void QueryCardsCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
        if (e.NewItems[0] is CardCollectionMTGCard newCard)
          newCard.IsOwned = OwnedCards.FirstOrDefault(x => x.Info.ScryfallId == newCard.Info.ScryfallId) != null;
        break;
    }
  }
}
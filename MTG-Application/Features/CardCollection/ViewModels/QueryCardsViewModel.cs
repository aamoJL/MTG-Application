using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using MTGApplication.Features.CardCollection.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection;

public partial class QueryCardsViewModel : ObservableObject
{
  public QueryCardsViewModel(MTGCardImporter importer)
  {
    Importer = importer;
    QueryCards = new(new CardCollectionIncrementalCardSource(importer));

    QueryCards.Collection.CollectionChanged += QueryCardsCollection_CollectionChanged;
    PropertyChanging += QueryCardsViewModel_PropertyChanging;
    PropertyChanged += QueryCardsViewModel_PropertyChanged;
  }

  public MTGCardImporter Importer { get; }

  private IncrementalLoadingCardCollection<CardCollectionMTGCard> QueryCards { get; }

  [ObservableProperty] private ObservableCollection<CardCollectionMTGCard> ownedCards = [];

  public IncrementalLoadingCollection<IncrementalCardSource<CardCollectionMTGCard>, CardCollectionMTGCard> Collection => QueryCards.Collection;
  public int TotalCardCount => QueryCards.TotalCardCount;

  public async Task UpdateQueryCards(string query)
  {
    var searchResult = await new FetchCardsWithSearchQuery(Importer).Execute(query);
    QueryCards.SetCollection([.. searchResult.Found.Select(x => new CardCollectionMTGCard(x.Info))], searchResult.NextPageUri, searchResult.TotalCount);

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
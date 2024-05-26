using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models.Card;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.Features.DeckEditor;

public class MTGCardChartSeriesItem : ObservableObject
{
  public MTGCardChartSeriesItem() => Cards.CollectionChanged += Cards_CollectionChanged;

  public ObservableCollection<MTGCard> Cards { get; } = [];

  public int Count => Cards.Sum(x => x.Count);

  protected void Cards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
        (e.NewItems[0] as MTGCard).PropertyChanged += CardsItem_PropertyChanged; break;
      case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
        (e.OldItems[0] as MTGCard).PropertyChanged -= CardsItem_PropertyChanged; break;
    }

    OnPropertyChanged(nameof(Count));
  }

  protected void CardsItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(MTGCard.Count))
      OnPropertyChanged(nameof(Count));
  }
}
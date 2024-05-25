using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models.Card;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.Features.DeckEditor;
public sealed partial class ManaCurveChart
{
  partial class MTGCardCMCSeriesItem : ObservableObject
  {
    public MTGCardCMCSeriesItem(MTGCard card)
    {
      Cards.CollectionChanged += Cards_CollectionChanged;

      CMC = card.Info.CMC;

      Cards.Add(card);
    }

    public ObservableCollection<MTGCard> Cards { get; } = [];

    public int CMC { get; }
    public int Count => Cards.Sum(x => x.Count);

    private void Cards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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

    private void CardsItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(MTGCard.Count))
        OnPropertyChanged(nameof(Count));
    }
  }
}
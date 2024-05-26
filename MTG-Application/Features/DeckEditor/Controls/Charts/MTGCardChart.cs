using LiveChartsCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models.Card;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckEditor;

public abstract class MTGCardChart : UserControl
{
  public static readonly DependencyProperty CardsProperty =
      DependencyProperty.Register(nameof(Cards), typeof(ObservableCollection<MTGCard>), typeof(MTGCardChart),
        new PropertyMetadata(new ObservableCollection<MTGCard>(), CardsPropertyChanged));

  protected MTGCardChart()
  {
    Loaded += (_, _) => { UpdateTheme(); AppConfig.LocalSettings.PropertyChanged += LocalSettings_PropertyChanged; };
    Unloaded += (_, _) => { AppConfig.LocalSettings.PropertyChanged -= LocalSettings_PropertyChanged; };
  }

  public ObservableCollection<MTGCard> Cards
  {
    get => (ObservableCollection<MTGCard>)GetValue(CardsProperty);
    set => SetValue(CardsProperty, value);
  }

  protected ObservableCollection<ISeries> Series { get; set; } = [];

  protected abstract void RemoveFromSeries(MTGCard card);

  protected abstract void AddToSeries(MTGCard card);

  protected abstract ISeries AddNewSeries(object property);

  protected virtual void UpdateTheme() { }

  protected void OnCardsChanged(ObservableCollection<MTGCard> oldValue)
  {
    if (oldValue != null) oldValue.CollectionChanged -= Cards_CollectionChanged;
    if (Cards != null) Cards.CollectionChanged += Cards_CollectionChanged;

    Series.Clear();

    foreach (var card in Cards)
      AddToSeries(card);
  }

  protected void Cards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case System.Collections.Specialized.NotifyCollectionChangedAction.Add: AddToSeries(e.NewItems[0] as MTGCard); break;
      case System.Collections.Specialized.NotifyCollectionChangedAction.Remove: RemoveFromSeries(e.OldItems[0] as MTGCard); break;
      case System.Collections.Specialized.NotifyCollectionChangedAction.Reset: Series.Clear(); break;
    }
  }

  protected void LocalSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
      UpdateTheme();
  }

  protected static void CardsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    => (sender as MTGCardChart).OnCardsChanged(e.OldValue as ObservableCollection<MTGCard>);

}

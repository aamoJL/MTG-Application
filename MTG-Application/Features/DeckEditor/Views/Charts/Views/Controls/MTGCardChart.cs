using LiveChartsCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckEditor.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MTGApplication.Features.DeckEditor.Views.Charts.Views.Controls;

public abstract class MTGCardChart : UserControl
{
  public static readonly DependencyProperty CardsProperty =
      DependencyProperty.Register(nameof(Cards), typeof(ReadOnlyObservableCollection<DeckEditorMTGCard>), typeof(MTGCardChart), new PropertyMetadata(new ReadOnlyObservableCollection<DeckEditorMTGCard>([]), CardsPropertyChanged));

  public ReadOnlyObservableCollection<DeckEditorMTGCard> Cards
  {
    get => (ReadOnlyObservableCollection<DeckEditorMTGCard>)GetValue(CardsProperty);
    set => SetValue(CardsProperty, value);
  }

  protected ObservableCollection<ISeries> Series { get; set; } = [];

  protected abstract void RemoveFromSeries(DeckEditorMTGCard card);

  protected abstract void AddToSeries(DeckEditorMTGCard card);

  protected abstract ISeries? AddNewSeries(object property);

  protected virtual void ResetSeries() => Series.Clear();

  protected void OnCardsChanged(ReadOnlyObservableCollection<DeckEditorMTGCard> oldValue)
  {
    if (oldValue != null)
      (oldValue as INotifyCollectionChanged).CollectionChanged -= Cards_CollectionChanged;

    if (Cards != null)
      (Cards as INotifyCollectionChanged).CollectionChanged += Cards_CollectionChanged;

    ResetSeries();

    if (Cards != null)
    {
      foreach (var card in Cards)
        AddToSeries(card);
    }
  }

  protected void Cards_CollectionChanged(object? _, NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case NotifyCollectionChangedAction.Add:
        if (e.NewItems?[0] is DeckEditorMTGCard newCard)
          AddToSeries(newCard);
        break;
      case NotifyCollectionChangedAction.Remove:
        if (e.OldItems?[0] is DeckEditorMTGCard oldCard)
          RemoveFromSeries(oldCard);
        break;
      case NotifyCollectionChangedAction.Reset: Series.Clear(); break;
    }
  }

  protected static void CardsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    if (sender is MTGCardChart chart && e.OldValue is ReadOnlyObservableCollection<DeckEditorMTGCard> collection)
      chart.OnCardsChanged(collection);
  }
}

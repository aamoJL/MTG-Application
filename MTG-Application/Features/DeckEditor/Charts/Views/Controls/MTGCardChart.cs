using LiveChartsCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckEditor.Editor.Models;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckEditor.Charts.Views.Controls;

public abstract class MTGCardChart : UserControl
{
  public static readonly DependencyProperty CardsProperty =
      DependencyProperty.Register(nameof(Cards), typeof(ObservableCollection<DeckEditorMTGCard>), typeof(MTGCardChart),
        new PropertyMetadata(new ObservableCollection<DeckEditorMTGCard>(), CardsPropertyChanged));

  protected MTGCardChart()
  {
    Loaded += (_, _) => { UpdateTheme(); AppConfig.LocalSettings.PropertyChanged += LocalSettings_PropertyChanged; };
    Unloaded += (_, _) => { AppConfig.LocalSettings.PropertyChanged -= LocalSettings_PropertyChanged; };
  }

  public ObservableCollection<DeckEditorMTGCard> Cards
  {
    get => (ObservableCollection<DeckEditorMTGCard>)GetValue(CardsProperty);
    set => SetValue(CardsProperty, value);
  }

  protected ObservableCollection<ISeries> Series { get; set; } = [];

  protected abstract void RemoveFromSeries(DeckEditorMTGCard card);

  protected abstract void AddToSeries(DeckEditorMTGCard card);

  protected abstract ISeries? AddNewSeries(object property);

  protected virtual void ResetSeries() => Series.Clear();

  protected virtual void UpdateTheme() { }

  protected void OnCardsChanged(ObservableCollection<DeckEditorMTGCard> oldValue)
  {
    if (oldValue != null)
      oldValue.CollectionChanged -= Cards_CollectionChanged;

    if (Cards != null)
      Cards.CollectionChanged += Cards_CollectionChanged;

    ResetSeries();

    if (Cards != null)
      foreach (var card in Cards)
        AddToSeries(card);
  }

  protected void Cards_CollectionChanged(object? _, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
        if (e.NewItems?[0] is DeckEditorMTGCard newCard)
          AddToSeries(newCard);
        break;
      case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
        if (e.OldItems?[0] is DeckEditorMTGCard oldCard)
          RemoveFromSeries(oldCard);
        break;
      case System.Collections.Specialized.NotifyCollectionChangedAction.Reset: Series.Clear(); break;
    }
  }

  protected void LocalSettings_PropertyChanged(object? _, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
      UpdateTheme();
  }

  protected static void CardsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    if (sender is MTGCardChart chart && e.OldValue is ObservableCollection<DeckEditorMTGCard> collection)
      chart.OnCardsChanged(collection);
  }
}

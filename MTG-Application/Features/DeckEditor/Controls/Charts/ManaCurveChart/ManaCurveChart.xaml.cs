using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Extensions;
using MTGApplication.General.Models.Card;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Linq;
using static MTGApplication.General.Models.Card.MTGCard;

namespace MTGApplication.Features.DeckEditor;
public sealed partial class ManaCurveChart : UserControl
{
  public static readonly DependencyProperty CardsProperty =
      DependencyProperty.Register(nameof(Cards), typeof(ObservableCollection<MTGCard>), typeof(ManaCurveChart),
        new PropertyMetadata(new ObservableCollection<MTGCard>(), CardsPropertyChanged));

  public ManaCurveChart()
  {
    InitializeComponent();

    Loaded += (_, _) => { UpdateTheme(); AppConfig.LocalSettings.PropertyChanged += LocalSettings_PropertyChanged; };
    Unloaded += (_, _) => { AppConfig.LocalSettings.PropertyChanged -= LocalSettings_PropertyChanged; };
  }

  public ICartesianAxis[] XAxes { get; set; } = new Axis[]
  {
    new() {
      LabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor),
      ForceStepToMin = true,
      MinStep = 1,
    }
  };
  public ICartesianAxis[] YAxes { get; set; } = new Axis[]
  {
    new() {
      LabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor),
      ForceStepToMin = true,
      MinStep = 5,
      MinLimit = 0,
    }
  };
  public ObservableCollection<MTGCard> Cards
  {
    get => (ObservableCollection<MTGCard>)GetValue(CardsProperty);
    set => SetValue(CardsProperty, value);
  }
  public ObservableCollection<ISeries> Series { get; set; } = [];

  private void AddToSeries(MTGCard card)
  {
    // Validation: Exclude lands
    if (card.Info.SpellTypes.All(x => x == SpellType.Land))
      return;

    // Combine cards with multiple colors into a one Multicolored item
    var primaryProperties = card.Info.Colors.Length > 1 ? [ColorTypes.M] : new[] { card.Info.Colors[0] };

    foreach (var color in primaryProperties)
    {
      // Find series or create a new one
      var series = Series.FirstOrDefault(x => x.Name == color.GetFullName()) ?? AddNewSeries(card);

      // Find series item or create a new one
      if (series.Values is ObservableCollection<MTGCardCMCSeriesItem> primaryValues)
      {
        if (primaryValues.FirstOrDefault(x => x.CMC == card.Info.CMC) is MTGCardCMCSeriesItem existingSeriesItem)
          existingSeriesItem.Cards.Add(card);
        else
          primaryValues.Add(new(card));
      }
    }
  }

  private void RemoveFromSeries(MTGCard card)
  {
    if (Series.FirstOrDefault(x => x.Name == card.ColorType.GetFullName()) is not ISeries series)
      return;

    if (series.Values is ObservableCollection<MTGCardCMCSeriesItem> values &&
      values.FirstOrDefault(x => x.Cards.Contains(card)) is MTGCardCMCSeriesItem existingSeriesItem)
    {
      // Remove card from the series
      existingSeriesItem.Cards.Remove(card);

      // Remove series item if empty
      if (existingSeriesItem.Cards.Count == 0)
        values.Remove(existingSeriesItem);

      // Remove series if empty
      if (values.Count == 0)
        Series.Remove(series);
    }
  }

  private StackedColumnSeries<MTGCardCMCSeriesItem> AddNewSeries(MTGCard card)
  {
    var color = card.ColorType;

    var newSeries = new StackedColumnSeries<MTGCardCMCSeriesItem>
    {
      Name = color.GetFullName(),
      Values = new ObservableCollection<MTGCardCMCSeriesItem>(),
      Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
      Fill = color switch
      {
        ColorTypes.W => new SolidColorPaint(ChartColorPalette.White),
        ColorTypes.U => new SolidColorPaint(ChartColorPalette.Blue),
        ColorTypes.B => new SolidColorPaint(ChartColorPalette.Black),
        ColorTypes.R => new SolidColorPaint(ChartColorPalette.Red),
        ColorTypes.G => new SolidColorPaint(ChartColorPalette.Green),
        ColorTypes.M => new SolidColorPaint(ChartColorPalette.Multicolor),
        ColorTypes.C => new SolidColorPaint(ChartColorPalette.Colorless),
        _ => new SolidColorPaint(SKColors.Pink),
      },
      Padding = 0,
      MaxBarWidth = double.MaxValue,
      DataLabelsPaint = new SolidColorPaint(ChartColorPalette.LightThemeText),
      DataLabelsSize = 10,
      DataLabelsPosition = DataLabelsPosition.Middle,
      Mapping = (value, index) => new(value.CMC, value.Count)
    };

    Series.Add(newSeries);
    SortSeries();

    return newSeries;
  }

  private void SortSeries()
  {
    var tempList = Series.OrderBy(x => x.Name).ToList();

    for (var i = 0; i < tempList.Count; i++)
      Series.Move(Series.IndexOf(tempList[i]), i);
  }

  /// <summary>
  /// Sets Axis colors for the selected theme
  /// </summary>
  private void UpdateTheme()
  {
    (XAxes[0] as Axis).LabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor);
    (YAxes[0] as Axis).LabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor);
  }

  private void OnCardsChanged(ObservableCollection<MTGCard> oldValue)
  {
    if (oldValue != null) oldValue.CollectionChanged -= Cards_CollectionChanged;
    if (Cards != null) Cards.CollectionChanged += Cards_CollectionChanged;
  }

  private void Cards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case System.Collections.Specialized.NotifyCollectionChangedAction.Add: AddToSeries(e.NewItems[0] as MTGCard); break;
      case System.Collections.Specialized.NotifyCollectionChangedAction.Remove: RemoveFromSeries(e.OldItems[0] as MTGCard); break;
      case System.Collections.Specialized.NotifyCollectionChangedAction.Reset: Series.Clear(); break;
    }
  }

  private void LocalSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
      UpdateTheme();
  }

  private static void CardsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    => (sender as ManaCurveChart).OnCardsChanged(e.OldValue as ObservableCollection<MTGCard>);
}
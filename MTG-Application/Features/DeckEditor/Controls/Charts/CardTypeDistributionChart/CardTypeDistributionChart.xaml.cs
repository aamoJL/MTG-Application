using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MTGApplication.General.Models.Card;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using static MTGApplication.General.Models.Card.MTGCard;

namespace MTGApplication.Features.DeckEditor;
public sealed partial class CardTypeDistributionChart : MTGCardChart
{
  public CardTypeDistributionChart() : base() => InitializeComponent();

  protected override void AddToSeries(MTGCard card)
  {
    var spellTypes = card.Info.SpellTypes;

    foreach (var spellType in spellTypes)
    {
      // Find series or create a new one
      var series = Series.FirstOrDefault(x => x.Name == spellType.ToString()) ?? AddNewSeries(spellType);

      if (series.Values is ObservableCollection<MTGCardChartSeriesItem> seriesValues)
      {
        // Find series item or create a new one
        var valueItem = seriesValues.FirstOrDefault()
          ?? new Func<MTGCardChartSeriesItem>(() =>
          {
            var newSeriesItem = new MTGCardChartSeriesItem();
            seriesValues.Add(newSeriesItem);
            return newSeriesItem;
          }).Invoke();

        // Add card to the series item
        valueItem.Cards.Add(card);
      }
    }
  }

  protected override void RemoveFromSeries(MTGCard card)
  {
    var spellTypes = card.Info.SpellTypes;

    foreach (var spellType in spellTypes)
    {
      // Find series
      if (Series.FirstOrDefault(x => x.Name == spellType.ToString()) is not ISeries series)
        return;

      // Find value item
      if (series.Values is ObservableCollection<MTGCardChartSeriesItem> seriesValues &&
        seriesValues.FirstOrDefault() is MTGCardChartSeriesItem valueItem && valueItem.Cards.Contains(card))
      {
        // Remove card from the value item
        valueItem.Cards.Remove(card);

        // Remove value item if empty
        if (valueItem.Cards.Count == 0)
          seriesValues.Remove(valueItem);

        // Remove series if empty
        if (seriesValues.Count == 0)
          Series.Remove(series);
      }
    }
  }

  protected override ISeries AddNewSeries(object spellType)
  {
    var series = new PieSeries<MTGCardChartSeriesItem>
    {
      Name = spellType.ToString(),
      Values = new ObservableCollection<MTGCardChartSeriesItem>(),
      Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
      Fill = spellType switch
      {
        SpellType.Land => new SolidColorPaint(ChartColorPalette.White),
        SpellType.Creature => new SolidColorPaint(ChartColorPalette.Green),
        SpellType.Artifact => new SolidColorPaint(ChartColorPalette.Colorless),
        SpellType.Enchantment => new SolidColorPaint(ChartColorPalette.Multicolor),
        SpellType.Instant => new SolidColorPaint(ChartColorPalette.Blue),
        SpellType.Sorcery => new SolidColorPaint(ChartColorPalette.Red),
        SpellType.Planeswalker => new SolidColorPaint(ChartColorPalette.Black),
        _ => new SolidColorPaint(SKColors.Pink),
      },
      DataLabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor),
      DataLabelsSize = 10,
      DataLabelsPosition = PolarLabelsPosition.Outer,
      DataLabelsFormatter = p => p.Context.Series.Name,
      Mapping = (value, _) => new(double.NaN, value.Count)
    };

    Series.Add(series);
    SortSeries();

    return series;
  }

  private void SortSeries()
  {
    var tempList = Series.OrderBy(x => x.Name).ToList();

    for (var i = 0; i < tempList.Count; i++)
      Series.Move(Series.IndexOf(tempList[i]), i);
  }
}

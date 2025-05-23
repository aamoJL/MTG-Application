using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MTGApplication.Features.DeckEditor.Charts.Models;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Extensions;
using System.Collections.Generic;
using System.Linq;
using static MTGApplication.General.Models.MTGCardInfo;

namespace MTGApplication.Features.DeckEditor.Charts.Views.Controls.ColorDistributionChart;
public sealed partial class ManaDistributionChart : MTGCardChart
{
  private static readonly ColorTypes[] _colorRange = [ColorTypes.W, ColorTypes.U, ColorTypes.B, ColorTypes.R, ColorTypes.G];
  private static readonly string _costSeriesName = "Costs";
  private static readonly string _productionSeriesName = "Producers";

  public ManaDistributionChart() : base()
  {
    InitializeComponent();

    // Series are predefined because all colors will always be shown
    AddNewSeries(null);
  }

  public IPolarAxis[] AngleAxes { get; } = [new PolarAxis() { Labels = _colorRange.Select(x => x.GetFullName()).ToList() }];
  public IPolarAxis[] RadiusAxes { get; } = [new PolarAxis() { Labeler = value => value.ToString() }];

  protected override void AddToSeries(DeckEditorMTGCard card)
  {
    AddToCosts(card);
    AddToProducers(card);
  }

  protected override void RemoveFromSeries(DeckEditorMTGCard card)
  {
    RemoveFromCosts(card);
    RemoveFromProducers(card);
  }

  protected override ISeries? AddNewSeries(object? property)
  {
    Series.Add(CreateNewCostsSeries());
    Series.Add(CreateNewProducersSeries());
    return null;
  }

  protected override void ResetSeries()
  {
    base.ResetSeries();
    AddNewSeries(null);
  }

  private void AddToCosts(DeckEditorMTGCard card)
  {
    // Filter colorless costs
    var colors = card.Info.Colors.Where(color => color != ColorTypes.C);

    if (Series.FirstOrDefault(x => x.Name == _costSeriesName)?.Values
      is IEnumerable<MTGCardChartSeriesItem> seriesValues)
    {
      foreach (var color in colors)
      {
        // Find series value item with color index and add the card
        seriesValues.ElementAtOrDefault(System.Array.IndexOf(_colorRange, color))?.Cards.Add(card);
      }
    }
  }

  private void AddToProducers(DeckEditorMTGCard card)
  {
    // Filter colorless mana productions
    var producedMana = card.Info.ProducedMana.Where(color => color != ColorTypes.C);

    if (Series.FirstOrDefault(x => x.Name == _productionSeriesName)?.Values
      is IEnumerable<MTGCardChartSeriesItem> seriesValues)
    {
      foreach (var color in producedMana)
      {
        // Find series value item with color index and add the card
        seriesValues.ElementAtOrDefault(System.Array.IndexOf(_colorRange, color))?.Cards.Add(card);
      }
    }
  }

  private void RemoveFromCosts(DeckEditorMTGCard card)
  {
    // Find value item
    if (Series.FirstOrDefault(x => x.Name == _costSeriesName) is ISeries series
      && series.Values is IEnumerable<MTGCardChartSeriesItem> seriesValues)
    {
      // Remove card from the series value items
      foreach (var item in seriesValues)
        item.Cards.Remove(card);
    }
  }

  private void RemoveFromProducers(DeckEditorMTGCard card)
  {
    // Find value item
    if (Series.FirstOrDefault(x => x.Name == _productionSeriesName) is ISeries series
      && series.Values is IEnumerable<MTGCardChartSeriesItem> seriesValues)
    {
      // Remove card from the series value items
      foreach (var item in seriesValues)
        item.Cards.Remove(card);
    }
  }

  private static PolarLineSeries<MTGCardChartSeriesItem> CreateNewCostsSeries()
  {
    return new PolarLineSeries<MTGCardChartSeriesItem>
    {
      Name = _costSeriesName,
      Values = _colorRange.Select(x => new MTGCardChartSeriesItem()).ToArray(), // Series has permanent values
      LineSmoothness = 0,
      GeometrySize = 0,
      GeometryFill = new SolidColorPaint(ChartColorPalette.White),
      Fill = new SolidColorPaint(ChartColorPalette.White.WithAlpha(90)),
      GeometryStroke = new SolidColorPaint(ChartColorPalette.Black, 5),
      Stroke = new SolidColorPaint(ChartColorPalette.Black, 5),
      Mapping = (value, index) => new(index, value.Count),
      IsVisibleAtLegend = true,
    };
  }

  private static PolarLineSeries<MTGCardChartSeriesItem> CreateNewProducersSeries()
  {
    return new PolarLineSeries<MTGCardChartSeriesItem>
    {
      Name = _productionSeriesName,
      Values = _colorRange.Select(x => new MTGCardChartSeriesItem()).ToArray(), // Series has permanent values
      LineSmoothness = 0,
      GeometrySize = 0,
      GeometryFill = new SolidColorPaint(ChartColorPalette.Black),
      Fill = new SolidColorPaint(ChartColorPalette.Black.WithAlpha(90)),
      GeometryStroke = new SolidColorPaint(ChartColorPalette.White, 5),
      Stroke = new SolidColorPaint(ChartColorPalette.White, 5),
      Mapping = (value, index) => new(index, value.Count),
      IsVisibleAtLegend = true,
    };
  }
}
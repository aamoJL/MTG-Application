using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MTGApplication.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using static MTGApplication.Services.MTGService;

namespace MTGApplication.ViewModels.Charts;

public abstract class PolarChart<TPrimaryType, TModel> : CardModelChart<TPrimaryType, TModel> where TModel : ObservableObject
{
  public override bool HasSecondarySeriesItems => true;
  public override bool RemoveEmptySeries => false;

  public abstract IPolarAxis[] AngleAxes { get; set; }
  public IPolarAxis[] RadiusAxes { get; set; } = new PolarAxis[]
  {
    new() { Labeler = value => value.ToString() }
  };

  // Primary series will always be the first series
  protected override ISeries FindSeries(TModel model, TPrimaryType item) => Series.FirstOrDefault();
}

public abstract class MTGPolarChart<TPrimaryType> : PolarChart<TPrimaryType, MTGCard>
{
}

/// <summary>
/// Chart class for a polar chart that shows what colors the cards are,
/// and how many of those colored cards are there.
/// </summary>
public class MTGColorPolarChart : MTGPolarChart<ColorTypes>
{

  protected static readonly ColorTypes[] colorRange =
  [
    ColorTypes.W,
    ColorTypes.U,
    ColorTypes.B,
    ColorTypes.R,
    ColorTypes.G,
  ];

  public override IPolarAxis[] AngleAxes { get; set; } = new PolarAxis[]
  {
    new() {
      Labels = colorRange.Select(x => x.GetFullName()).ToList(),
    }
  };

  public MTGColorPolarChart()
  {
    AddAndSortSeries(CardModelSeriesItem<MTGCard>.CreatePolarLineSeries(
        secondarySeriesItems: colorRange.Select(x => new MTGCardModelCountSeriesItem()).ToArray()));
  }

  protected override ColorTypes[] GetPrimaryProperties(MTGCard model) => model.Info.Colors.Where(x => colorRange.Contains(x)).ToArray();

  // Series is already created on constructor
  protected override ISeries CreateNewSeries(ColorTypes item) => null;

  // Model needs to be any of the valid colors to be valid
  protected override bool ModelValidation(MTGCard model) => model.Info.Colors.Any(x => colorRange.Contains(x));

  // Secondary series are already created on constructor
  protected override CardModelSeriesItem<MTGCard> CreateNewSecondarySeriesItem(MTGCard model) => null;

  protected override CardModelSeriesItem<MTGCard> FindSecondarySeriesItem(ObservableCollection<CardModelSeriesItem<MTGCard>> items, ColorTypes primaryProperty, MTGCard model)
  {
    var index = Array.IndexOf(colorRange, primaryProperty);
    if (items == null || index == -1) { return null; }

    return items[index];
  }
}

/// <summary>
/// Chart class for a polar chart that shows what mana colors the cards could produce,
/// and how many producers are there.
/// </summary>
public class MTGManaProductionPolarChart : MTGColorPolarChart
{
  protected override ColorTypes[] GetPrimaryProperties(MTGCard model) => model.Info.ProducedMana.Where(x => colorRange.Contains(x)).ToArray();

  // Model needs to produce any of the valid colors to be valid
  protected override bool ModelValidation(MTGCard model) => model.Info.ProducedMana.Any(x => colorRange.Contains(x));
}

/// <summary>
/// Chart class for a polar chart that combines both 
/// <see cref="MTGColorPolarChart"/> and <see cref="MTGManaProductionPolarChart"/>
/// </summary>
public class MTGColorAndManaPolarChart
{
  public IPolarAxis[] AngleAxes => colorChart.AngleAxes;
  public IPolarAxis[] RadiusAxes => colorChart.RadiusAxes;
  public ISeries[] Series { get; }

  protected MTGColorPolarChart colorChart = new();
  protected MTGManaProductionPolarChart manaChart = new();

  public MTGColorAndManaPolarChart(ObservableCollection<MTGCard> models)
  {
    colorChart = new() { Models = models };
    manaChart = new() { Models = models };

    Series = [
      new PolarLineSeries<CardModelSeriesItem<MTGCard>>
      {
        Name = "Color",
        Values = (ObservableCollection<CardModelSeriesItem<MTGCard>>)colorChart.Series.First().Values,
        LineSmoothness = 0,
        GeometrySize = 0,
        GeometryFill = new SolidColorPaint(ChartColorPalette.White),
        Fill = new SolidColorPaint(ChartColorPalette.White.WithAlpha(90)),
        GeometryStroke = new SolidColorPaint(ChartColorPalette.Black, 5),
        Stroke = new SolidColorPaint(ChartColorPalette.Black, 5),
        Mapping = (value, index) => new(index, value.PrimaryValue),
        IsVisibleAtLegend = true,
      },
      new PolarLineSeries<CardModelSeriesItem<MTGCard>>
      {
        Name = "Mana",
        Values = (ObservableCollection<CardModelSeriesItem<MTGCard>>)manaChart.Series.First().Values,
        LineSmoothness = 0,
        GeometrySize = 0,
        GeometryFill = new SolidColorPaint(ChartColorPalette.Black),
        Fill = new SolidColorPaint(ChartColorPalette.Black.WithAlpha(90)),
        GeometryStroke = new SolidColorPaint(ChartColorPalette.White, 5),
        Stroke = new SolidColorPaint(ChartColorPalette.White, 5),
        Mapping = (value, index) => new(index, value.PrimaryValue),
        IsVisibleAtLegend = true,
      }
    ];
  }
}

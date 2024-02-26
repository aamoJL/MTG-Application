using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MTGApplication.Models;
using System.Collections.ObjectModel;
using System.Linq;
using static MTGApplication.Services.MTGService;

namespace MTGApplication.ViewModels.Charts;

/// <summary>
/// Base class for MTG card stacked column charts
/// </summary>
public abstract class StackedColumnChart<TPrimaryType, TModel> : CardModelChart<TPrimaryType, TModel> where TModel : ObservableObject
{
  public StackedColumnChart() { }

  public override bool HasSecondarySeriesItems => true;

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

  /// <summary>
  /// Sets Axis colors for the selected theme
  /// </summary>
  public void UpdateTheme()
  {
    (XAxes[0] as Axis).LabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor);
    (YAxes[0] as Axis).LabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor);
  }
}

/// <summary>
/// Stacked column chart that shows MTG cardlist CMC distribution, separated by card color
/// </summary>
public class MTGCMCStackedColumnChart : StackedColumnChart<ColorTypes, MTGCard>
{
  public MTGCMCStackedColumnChart() { }

  protected override bool ModelValidation(MTGCard model) => !model.Info.SpellTypes.Contains(SpellType.Land); // Don't count Lands as a color

  protected override StackedColumnSeries<CardModelSeriesItem<MTGCard>> FindSeries(MTGCard model, ColorTypes item)
  {
    if (Series.FirstOrDefault(x => x.Name == item.GetFullName()) is StackedColumnSeries<CardModelSeriesItem<MTGCard>> series)
    {
      return series;
    }
    else { return null; }
  }

  protected override ColorTypes[] GetPrimaryProperties(MTGCard model) => model.Info.Colors.Length > 1 ? new[] { ColorTypes.M } : new[] { model.Info.Colors[0] };

  protected override StackedColumnSeries<CardModelSeriesItem<MTGCard>> CreateNewSeries(ColorTypes item) => CardModelSeriesItem<MTGCard>.CreateStackedColumnSeriesByColor(item);

  protected override CardModelSeriesItem<MTGCard> CreateNewSecondarySeriesItem(MTGCard model) => new MTGCardModelCMCSeriesItem(model); // Create new cmc object

  protected override CardModelSeriesItem<MTGCard> FindSecondarySeriesItem(ObservableCollection<CardModelSeriesItem<MTGCard>> items, ColorTypes primaryProperty, MTGCard model)
    => items?.FirstOrDefault(x => x.SecondaryValue == model.Info.CMC, null); // Find series' cmc object
}
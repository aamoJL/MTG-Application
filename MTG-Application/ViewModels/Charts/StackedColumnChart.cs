using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MTGApplication.Models;
using System.Linq;
using static MTGApplication.Services.MTGService;

namespace MTGApplication.ViewModels.Charts;

/// <summary>
/// Base class for MTG card stacked column charts
/// </summary>
public abstract class StackedColumnChart<TPrimaryType, TModel> : CardModelChart<TPrimaryType, TModel> where TModel : ObservableObject
{
  public StackedColumnChart() { }

  public override bool HasSecondaryValues => true;

  public ICartesianAxis[] XAxes { get; set; } = new Axis[]
  {
    new Axis
    {
      LabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor),
      ForceStepToMin = true,
      MinStep = 1,
    }
  };
  public ICartesianAxis[] YAxes { get; set; } = new Axis[]
  {
    new Axis
    {
      LabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor),
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

  protected override StackedColumnSeries<CardModelSeries<MTGCard>> FindPrimarySeries(MTGCard model, ColorTypes item)
  {
    if (Series.FirstOrDefault(x => x.Name == item.GetFullName()) is StackedColumnSeries<CardModelSeries<MTGCard>> series)
    {
      return series;
    }
    else { return null; }
  }

  protected override ColorTypes[] GetPrimaryItems(MTGCard model) => model.Info.Colors.Length > 1 ? new ColorTypes[] { ColorTypes.M } : new ColorTypes[] { model.Info.Colors[0] };

  protected override StackedColumnSeries<CardModelSeries<MTGCard>> CreateNewSeries(ColorTypes item) => CardModelSeries<MTGCard>.CreateStackedColumnSeriesByColor(item);

  protected override CardModelSeries<MTGCard> FindSecondaryItem(ISeries series, MTGCard model)
    => ((StackedColumnSeries<CardModelSeries<MTGCard>>)series).Values.FirstOrDefault(x => x.SecondaryValue == model.Info.CMC); // Find series' cmc object

  protected override CardModelSeries<MTGCard> CreateNewSecondarySeries(MTGCard model) => new MTGCardModelCMCSeries(model); // Create new cmc object
}
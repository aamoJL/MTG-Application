using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
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

public class MTGColorDevotionPolarChart : MTGPolarChart<ColorTypes>
{
  private static readonly ColorTypes[] validColors =
  [
    ColorTypes.W,
    ColorTypes.U,
    ColorTypes.B,
    ColorTypes.R,
    ColorTypes.G,
  ];

  public override IPolarAxis[] AngleAxes { get; set; } = new PolarAxis[]
  {
    new() { Labels = validColors.Select(x => x.ToString()).ToList() }
  };

  protected override ColorTypes[] GetPrimaryProperties(MTGCard model)
    => model.Info.Colors.Where(x => validColors.Contains(x)).ToArray();

  protected override ISeries CreateNewSeries(ColorTypes item)
    => CardModelSeriesItem<MTGCard>.CreatePolarLineSeries(
      secondarySeriesItems: validColors.Select(x => new MTGCardModelCountSeriesItem()).ToArray());

  // Model needs to have any of the valid colors to be valid
  protected override bool ModelValidation(MTGCard model)
    => model.Info.Colors.Any(x => validColors.Contains(x));

  // Secondary series are already created on [CreateNewSeries]
  protected override CardModelSeriesItem<MTGCard> CreateNewSecondarySeriesItem(MTGCard model) => null;

  protected override CardModelSeriesItem<MTGCard> FindSecondarySeriesItem(ObservableCollection<CardModelSeriesItem<MTGCard>> items, ColorTypes primaryProperty, MTGCard model)
  {
    var index = Array.IndexOf(validColors, primaryProperty);
    if (items == null || index == -1) { return null; }

    return items[index];
  }
}
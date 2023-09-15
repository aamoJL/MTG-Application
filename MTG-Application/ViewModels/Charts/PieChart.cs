using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using MTGApplication.Models;
using System.Linq;
using static MTGApplication.Services.MTGService;

namespace MTGApplication.ViewModels.Charts;

/// <summary>
/// Base class for pie charts
/// </summary>
public abstract class PieChart<TPrimaryType, TModel> : CardModelChart<TPrimaryType, TModel> where TModel : ObservableObject
{
  public override bool HasSecondaryValues => false;
}

/// <summary>
/// Base class for MTG pie charts
/// </summary>
public abstract class MTGPieChart<TPrimaryType> : PieChart<TPrimaryType, MTGCard>
{
  protected override CardModelSeries<MTGCard> CreateNewSecondarySeries(MTGCard model) => new MTGCardModelCountSeries(model);
}

/// <summary>
/// Pie chart that shows MTG cardlist spell type distribution
/// </summary>
public class MTGSpellTypePieChart : MTGPieChart<SpellType>
{
  public MTGSpellTypePieChart() { }

  protected override CardModelSeries<MTGCard> FindSecondaryItem(ISeries series, MTGCard model) => null;

  protected override ISeries CreateNewSeries(SpellType item) => CardModelSeries<MTGCard>.CreateSeriesGroupBySpellType(item);

  protected override ISeries FindPrimarySeries(MTGCard model, SpellType item)
  {
    if (Series.FirstOrDefault(x => x.Name == item.ToString()) is PieSeries<CardModelSeries<MTGCard>> series)
    {
      return series;
    }
    else { return null; }
  }

  protected override SpellType[] GetPrimaryItems(MTGCard model) => model.Info.SpellTypes;
}

/// <summary>
/// Pie chart that shows MTG cardlist mana production color distribution
/// </summary>
public class MTGManaProductionPieChart : MTGPieChart<ColorTypes>
{
  public MTGManaProductionPieChart() { }

  protected override CardModelSeries<MTGCard> FindSecondaryItem(ISeries series, MTGCard model) => null;

  protected override ISeries CreateNewSeries(ColorTypes item)
  {
    var series = CardModelSeries<MTGCard>.CreatePieSeriesByColor(item);
    series.DataLabelsSize = 0;
    series.HoverPushout = 0;
    return series;
  }

  protected override ISeries FindPrimarySeries(MTGCard model, ColorTypes item)
  {
    if (Series.FirstOrDefault(x => x.Name == item.GetFullName()) is PieSeries<CardModelSeries<MTGCard>> series) { return series; }
    else { return null; }
  }

  protected override ColorTypes[] GetPrimaryItems(MTGCard model)
  {
    // Combine together mana producers that can produce all colors
    var producedMana = model.Info.ProducedMana;
    if (producedMana.Length == 6 || (producedMana.Length == 5 && !producedMana.Contains(ColorTypes.C))) { return new ColorTypes[] { ColorTypes.M }; }
    else { return producedMana; }
  }
}

/// <summary>
/// Pie chart that shows MTG cardlist color distribution
/// </summary>
public class MTGColorPieChart : MTGPieChart<ColorTypes>
{
  public MTGColorPieChart(int innerRadius = 0) => InnerRadius = innerRadius;

  public int InnerRadius { get; }

  protected override CardModelSeries<MTGCard> FindSecondaryItem(ISeries series, MTGCard model) => null;

  protected override bool ModelValidation(MTGCard model) => !model.Info.SpellTypes.Contains(SpellType.Land); // Exclude lands

  protected override ISeries FindPrimarySeries(MTGCard model, ColorTypes item)
  {
    if (Series.FirstOrDefault(x => x.Name == item.GetFullName()) is PieSeries<CardModelSeries<MTGCard>> series) { return series; }
    else { return null; }
  }

  protected override ColorTypes[] GetPrimaryItems(MTGCard model) => model.Info.Colors;

  protected override ISeries CreateNewSeries(ColorTypes item)
  {
    var series = CardModelSeries<MTGCard>.CreatePieSeriesByColor(item);
    series.InnerRadius = InnerRadius;
    series.DataLabelsSize = 0;
    series.HoverPushout = 0;
    return series;
  }
}
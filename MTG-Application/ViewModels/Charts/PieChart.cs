using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using MTGApplication.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static MTGApplication.Services.MTGService;

namespace MTGApplication.ViewModels.Charts;

/// <summary>
/// Base class for pie charts
/// </summary>
public abstract class PieChart<TPrimaryType, TModel> : CardModelChart<TPrimaryType, TModel> where TModel : ObservableObject
{
  public override bool HasSecondarySeriesItems => false;
}

/// <summary>
/// Base class for MTG pie charts
/// </summary>
public abstract class MTGPieChart<TPrimaryType> : PieChart<TPrimaryType, MTGCard>
{
  protected override CardModelSeriesItem<MTGCard> CreateNewSecondarySeriesItem(MTGCard model) => new MTGCardModelCountSeriesItem(model);
}

/// <summary>
/// Pie chart that shows MTG cardlist spell type distribution
/// </summary>
public class MTGSpellTypePieChart : MTGPieChart<SpellType>
{
  public MTGSpellTypePieChart() { }

  protected override ISeries CreateNewSeries(SpellType item) => CardModelSeriesItem<MTGCard>.CreatePieSeriesBySpellType(item);

  protected override ISeries FindSeries(MTGCard model, SpellType item)
  {
    if (Series.FirstOrDefault(x => x.Name == item.ToString()) is PieSeries<CardModelSeriesItem<MTGCard>> series)
    {
      return series;
    }
    else { return null; }
  }

  protected override SpellType[] GetPrimaryProperties(MTGCard model) => model.Info.SpellTypes;

  protected override CardModelSeriesItem<MTGCard> FindSecondarySeriesItem(ObservableCollection<CardModelSeriesItem<MTGCard>> items, SpellType primaryProperty, MTGCard model)
    => null;
}

/// <summary>
/// Pie chart that shows MTG cardlist mana production color distribution
/// </summary>
public class MTGManaProductionPieChart : MTGPieChart<ColorTypes>
{
  public MTGManaProductionPieChart() { }

  protected override ISeries CreateNewSeries(ColorTypes item)
  {
    var series = CardModelSeriesItem<MTGCard>.CreatePieSeriesByColor(item);
    series.DataLabelsSize = 0;
    series.HoverPushout = 0;
    return series;
  }

  protected override ISeries FindSeries(MTGCard model, ColorTypes item)
  {
    if (Series.FirstOrDefault(x => x.Name == item.GetFullName()) is PieSeries<CardModelSeriesItem<MTGCard>> series) { return series; }
    else { return null; }
  }

  protected override ColorTypes[] GetPrimaryProperties(MTGCard model)
  {
    // Combine together mana producers that can produce all colors
    var producedMana = model.Info.ProducedMana;
    if (producedMana.Length == 6 || (producedMana.Length == 5 && !producedMana.Contains(ColorTypes.C))) { return new ColorTypes[] { ColorTypes.M }; }
    else { return producedMana; }
  }

  protected override CardModelSeriesItem<MTGCard> FindSecondarySeriesItem(ObservableCollection<CardModelSeriesItem<MTGCard>> items, ColorTypes primaryProperty, MTGCard model)
    => null;
}

/// <summary>
/// Pie chart that shows MTG cardlist color distribution
/// </summary>
public class MTGColorPieChart : MTGPieChart<ColorTypes>
{
  public MTGColorPieChart(int innerRadius = 0) => InnerRadius = innerRadius;

  public int InnerRadius { get; }

  protected override bool ModelValidation(MTGCard model) => !model.Info.SpellTypes.Contains(SpellType.Land); // Exclude lands

  protected override ISeries FindSeries(MTGCard model, ColorTypes item)
  {
    if (Series.FirstOrDefault(x => x.Name == item.GetFullName()) is PieSeries<CardModelSeriesItem<MTGCard>> series) { return series; }
    else { return null; }
  }

  protected override ColorTypes[] GetPrimaryProperties(MTGCard model) => model.Info.Colors;

  protected override ISeries CreateNewSeries(ColorTypes item)
  {
    var series = CardModelSeriesItem<MTGCard>.CreatePieSeriesByColor(item);
    series.InnerRadius = InnerRadius;
    series.DataLabelsSize = 0;
    series.HoverPushout = 0;
    return series;
  }

  protected override CardModelSeriesItem<MTGCard> FindSecondarySeriesItem(ObservableCollection<CardModelSeriesItem<MTGCard>> items, ColorTypes primaryProperty, MTGCard model)
    => null;
}
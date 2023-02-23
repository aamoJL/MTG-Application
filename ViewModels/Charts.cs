﻿using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.ComponentModel;
using System.Collections.Specialized;
using static MTGApplication.Models.MTGCard;
using MTGApplication.Models;
using System;
using System.Xml.Linq;

namespace MTGApplication.ViewModels
{
  public static class ColorPalette
  {
    public static readonly SKColor White = new(235, 235, 235, 100);
    public static readonly SKColor Blue = new(80, 130, 186, 100);
    public static readonly SKColor Black = new(80, 80, 80, 100);
    public static readonly SKColor Red = new(186, 80, 80, 100);
    public static readonly SKColor Green = new(80, 186, 80, 100);
    public static readonly SKColor Multicolor = new(186, 186, 80, 100);
    public static readonly SKColor Colorless = new(186, 186, 186, 100);
  }

  #region Series
  public abstract class MTGCardModelSeries : ViewModelBase
  {
    protected ObservableCollection<MTGCard> Models { get; }
    protected double primaryValue;

    public virtual double PrimaryValue
    {
      get => primaryValue;
      set
      {
        primaryValue = value;
        OnPropertyChanged(nameof(PrimaryValue));
      }
    }
    public virtual double SecondaryValue { get; protected set; }

    public MTGCardModelSeries(MTGCard model)
    {
      Models = new();
      Models.CollectionChanged += Models_CollectionChanged;

      AddItem(model);
    }

    public virtual void AddItem(MTGCard model)
    {
      if (Models.Contains(model)) { return; }
      Models.Add(model);

      model.PropertyChanged += Model_PropertyChanged;
    }
    public virtual void RemoveItem(MTGCard model)
    {
      if (!Models.Contains(model)) { return; }
      Models.Remove(model);

      model.PropertyChanged -= Model_PropertyChanged;
    }

    protected virtual void Model_PropertyChanged(object sender, PropertyChangedEventArgs e) { }
    protected virtual void Models_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }

    public static StackedColumnSeries<MTGCardModelSeries> CreateStackedColumnSeriesByColor(ColorTypes color)
    {
      return new StackedColumnSeries<MTGCardModelSeries>
      {
        Name = GetColorTypeName(color),
        Values = new ObservableCollection<MTGCardModelSeries>(),
        Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
        Fill = color switch
        {
          ColorTypes.W => new SolidColorPaint(ColorPalette.White),
          ColorTypes.U => new SolidColorPaint(ColorPalette.Blue),
          ColorTypes.B => new SolidColorPaint(ColorPalette.Black),
          ColorTypes.R => new SolidColorPaint(ColorPalette.Red),
          ColorTypes.G => new SolidColorPaint(ColorPalette.Green),
          ColorTypes.M => new SolidColorPaint(ColorPalette.Multicolor),
          ColorTypes.C => new SolidColorPaint(ColorPalette.Colorless),
          _ => new SolidColorPaint(SKColors.Pink),
        },
        Padding = 0,
        MaxBarWidth = double.MaxValue,
        DataLabelsPaint = new SolidColorPaint(new(45, 45, 45)),
        DataLabelsSize = 10,
        DataLabelsPosition = DataLabelsPosition.Middle,
        Mapping = (value, point) =>
        {
          point.PrimaryValue = value.PrimaryValue;
          point.SecondaryValue = value.SecondaryValue;
        }
      };
    }
    public static PieSeries<MTGCardModelSeries> CreatePieSeriesByColor(ColorTypes color)
    {
      return new PieSeries<MTGCardModelSeries>
      {
        Name = GetColorTypeName(color),
        Values = new ObservableCollection<MTGCardModelSeries>(),
        Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
        Fill = color switch
        {
          ColorTypes.W => new SolidColorPaint(ColorPalette.White),
          ColorTypes.U => new SolidColorPaint(ColorPalette.Blue),
          ColorTypes.B => new SolidColorPaint(ColorPalette.Black),
          ColorTypes.R => new SolidColorPaint(ColorPalette.Red),
          ColorTypes.G => new SolidColorPaint(ColorPalette.Green),
          ColorTypes.C => new SolidColorPaint(ColorPalette.Colorless),
          ColorTypes.M => new SolidColorPaint(ColorPalette.Multicolor),
          _ => new SolidColorPaint(SKColors.Pink),
        },
        DataLabelsPaint = new SolidColorPaint(new(45, 45, 45)),
        DataLabelsSize = 10,
        DataLabelsPosition = PolarLabelsPosition.Outer,
        DataLabelsFormatter = p => p.Context.Series.Name,
        Mapping = (value, point) =>
        {
          point.PrimaryValue = value.PrimaryValue;
          point.SecondaryValue = value.SecondaryValue;
        }
      };
    }
    public static PieSeries<MTGCardModelSeries> CreateSeriesGroupBySpellType(SpellType spellType)
    {
      return new PieSeries<MTGCardModelSeries>
      {
        Name = spellType.ToString(),
        Values = new ObservableCollection<MTGCardModelSeries>(),
        Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
        Fill = spellType switch
        {
          SpellType.Land => new SolidColorPaint(ColorPalette.White),
          SpellType.Creature => new SolidColorPaint(ColorPalette.Green),
          SpellType.Artifact => new SolidColorPaint(ColorPalette.Colorless),
          SpellType.Enchantment => new SolidColorPaint(ColorPalette.Multicolor),
          SpellType.Instant => new SolidColorPaint(ColorPalette.Blue),
          SpellType.Sorcery => new SolidColorPaint(ColorPalette.Red),
          SpellType.Planeswalker => new SolidColorPaint(ColorPalette.Black),
          _ => new SolidColorPaint(SKColors.Pink),
        },
        DataLabelsPaint = new SolidColorPaint(new(45, 45, 45)),
        DataLabelsSize = 10,
        DataLabelsPosition = PolarLabelsPosition.Outer,
        DataLabelsFormatter = p => p.Context.Series.Name,
        Mapping = (value, point) =>
        {
          point.PrimaryValue = value.PrimaryValue;
          point.SecondaryValue = value.SecondaryValue;
        }
      };
    }
  }

  public class MTGCardModelCountSeries : MTGCardModelSeries
  {
    public MTGCardModelCountSeries(MTGCard model) : base(model) { }

    protected override void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.Model_PropertyChanged(sender, e);
      UpdateCardCount();
    }
    protected override void Models_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      base.Models_CollectionChanged(sender, e);
      UpdateCardCount();
    }

    protected void UpdateCardCount() => PrimaryValue = Models.Sum(x => x.Count);
  }

  public class MTGCardModelCMCSeries : MTGCardModelCountSeries
  {
    public MTGCardModelCMCSeries(MTGCard model) : base(model)
    {
      SecondaryValue = model.Info.CMC;
    }
  }
  #endregion

  #region Charts
  public abstract class MTGCardModelChart
  {
    public ObservableCollection<ISeries> Series { get; } = new();

    public MTGCardModelChart(ObservableCollection<MTGCard> models)
    {
      foreach (var model in models)
      {
        AddToChartSeries(model);
      }

      models.CollectionChanged += Models_CollectionChanged;
    }

    protected virtual void Models_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add: AddToChartSeries(e.NewItems[0] as MTGCard); break;
        case NotifyCollectionChangedAction.Remove: RemoveFromChartSeries(e.OldItems[0] as MTGCard); break;
        case NotifyCollectionChangedAction.Reset: Series.Clear(); break;
        default: break;
      }
    }

    public abstract void AddToChartSeries(MTGCard model);
    public abstract void RemoveFromChartSeries(MTGCard model);
    public virtual void ClearChart()
    {
      Series.Clear();
    }
  }
  
  public class MTGCMCStackedColumnChart : MTGCardModelChart
  {
    public MTGCMCStackedColumnChart(ObservableCollection<MTGCard> models) : base(models) { }

    public override void AddToChartSeries(MTGCard model)
    {
      // Don't count Lands as a color
      if (model.Info.SpellTypes.Contains(SpellType.Land)) { return; }

      // Find Color series
      ColorTypes color = model.Info.Colors.Length > 1 ? ColorTypes.M : model.Info.Colors[0];
      if (Series.FirstOrDefault(x => x.Name == GetColorTypeName(color)) is not StackedColumnSeries<MTGCardModelSeries> colorSeries)
      {
        // Create new series group if it does not exist
        colorSeries = MTGCardModelSeries.CreateStackedColumnSeriesByColor(color);
        Series.Add(colorSeries);
      }

      // Find series' cmc object
      MTGCardModelSeries cmcObject = colorSeries.Values.FirstOrDefault(x => x.SecondaryValue == model.Info.CMC);
      if (cmcObject != null)
      {
        // Add to existing object
        cmcObject.AddItem(model);
      }
      else
      {
        // Create new cmc object
        cmcObject = new MTGCardModelCMCSeries(model);
        (colorSeries.Values as ObservableCollection<MTGCardModelSeries>).Add(cmcObject);
      }
    }
    public override void RemoveFromChartSeries(MTGCard model)
    {
      // Don't count Lands as a color
      if (model.Info.SpellTypes.Contains(SpellType.Land)) { return; }

      // Find Color series
      ColorTypes color = model.Info.Colors.Length > 1 ? ColorTypes.M : model.Info.Colors[0];
      if (Series.FirstOrDefault(x => x.Name == GetColorTypeName(color)) is not StackedColumnSeries<MTGCardModelSeries> colorSeries)
      {
        return; // Returns if color series does not exist
      }

      // Find series' cmc object
      if (colorSeries.Values.FirstOrDefault(x => x.SecondaryValue == model.Info.CMC) is MTGCardModelSeries valueObject)
      {
        valueObject.RemoveItem(model);
        if (valueObject.PrimaryValue == 0) { (colorSeries.Values as ObservableCollection<MTGCardModelSeries>).Remove(valueObject); } // Remove cmcObject if its count is zero
      }
    }
  }
  
  public class MTGSpellTypePieChart : MTGCardModelChart
  {
    public MTGSpellTypePieChart(ObservableCollection<MTGCard> models) : base(models) { }

    public override void AddToChartSeries(MTGCard model)
    {
      foreach (var spellType in model.Info.SpellTypes)
      {
        // Find series
        if (Series.FirstOrDefault(x => x.Name == spellType.ToString()) is not PieSeries<MTGCardModelSeries> typeSeries)
        {
          // Create new series group if it does not exist
          typeSeries = MTGCardModelSeries.CreateSeriesGroupBySpellType(spellType);
          Series.Add(typeSeries);

          // Create new value object to the series
          (typeSeries.Values as ObservableCollection<MTGCardModelSeries>).Add(new MTGCardModelCountSeries(model));
        }
        else
        {
          // Add model to the value object
          MTGCardModelSeries valueObject = (typeSeries.Values as ObservableCollection<MTGCardModelSeries>)[0];
          valueObject.AddItem(model);
        }
      }
    }
    public override void RemoveFromChartSeries(MTGCard model)
    {
      foreach (var spellType in model.Info.SpellTypes)
      {
        // Find series
        if (Series.FirstOrDefault(x => x.Name == spellType.ToString()) is not PieSeries<MTGCardModelSeries> typeSeries)
        {
          return; // Returns if color series does not exist
        }
        var valueObject = (typeSeries.Values as ObservableCollection<MTGCardModelSeries>)[0];
        valueObject.RemoveItem(model);

        if (valueObject.PrimaryValue == 0) { Series.Remove(typeSeries); }
      }
    }
  }

  public class MTGManaProductionPieChart : MTGCardModelChart
  {
    public MTGManaProductionPieChart(ObservableCollection<MTGCard> models) : base(models) { }

    public override void AddToChartSeries(MTGCard model)
    {
      // Combine together mana producers that can produce all colors
      var producedManas = model.Info.ProducedMana.Length == 5 ? new ColorTypes[] { ColorTypes.M } : model.Info.ProducedMana;

      foreach (var producedMana in producedManas)
      {
        // Find series
        if (Series.FirstOrDefault(x => x.Name == GetColorTypeName(producedMana)) is not PieSeries<MTGCardModelSeries> manaSeries)
        {
          // Create new series group if it does not exist
          manaSeries = MTGCardModelSeries.CreatePieSeriesByColor(producedMana);
          Series.Add(manaSeries);

          // Create new value object to the series
          (manaSeries.Values as ObservableCollection<MTGCardModelSeries>).Add(new MTGCardModelCountSeries(model));
        }
        else
        {
          // Add model to the value object
          MTGCardModelSeries valueObject = (manaSeries.Values as ObservableCollection<MTGCardModelSeries>)[0];
          valueObject.AddItem(model);
        }
      }
    }
    public override void RemoveFromChartSeries(MTGCard model)
    {
      // Combine together mana producers that can produce all colors
      var producedManas = model.Info.ProducedMana.Length == 5 ? new ColorTypes[] { ColorTypes.M } : model.Info.ProducedMana;

      foreach (var colorType in producedManas)
      {
        // Find series
        if (Series.FirstOrDefault(x => x.Name == GetColorTypeName(colorType)) is not PieSeries<MTGCardModelSeries> typeSeries)
        {
          return; // Returns if color series does not exist
        }
        var valueObject = (typeSeries.Values as ObservableCollection<MTGCardModelSeries>)[0];
        valueObject.RemoveItem(model);

        if (valueObject.PrimaryValue == 0) { Series.Remove(typeSeries); }
      }
    }
  }

  public class MTGColorPieChart : MTGCardModelChart
  {
    public MTGColorPieChart(ObservableCollection<MTGCard> models) : base(models) { }

    public override void AddToChartSeries(MTGCard model)
    {
      // Exclude lands
      if(model.Info.SpellTypes.Contains(SpellType.Land)) { return; }

      foreach (var colorType in model.Info.Colors)
      {
        // Find series
        if (Series.FirstOrDefault(x => x.Name == GetColorTypeName(colorType)) is not PieSeries<MTGCardModelSeries> manaSeries)
        {
          // Create new series group if it does not exist
          manaSeries = MTGCardModelSeries.CreatePieSeriesByColor(colorType);
          Series.Add(manaSeries);

          // Create new value object to the series
          (manaSeries.Values as ObservableCollection<MTGCardModelSeries>).Add(new MTGCardModelCountSeries(model));
        }
        else
        {
          // Add model to the value object
          MTGCardModelSeries valueObject = (manaSeries.Values as ObservableCollection<MTGCardModelSeries>)[0];
          valueObject.AddItem(model);
        }
      }
    }
    public override void RemoveFromChartSeries(MTGCard model)
    {
      foreach (var colorType in model.Info.Colors)
      {
        // Find series
        if (Series.FirstOrDefault(x => x.Name == GetColorTypeName(colorType)) is not PieSeries<MTGCardModelSeries> typeSeries)
        {
          return; // Returns if color series does not exist
        }
        var valueObject = (typeSeries.Values as ObservableCollection<MTGCardModelSeries>)[0];
        valueObject.RemoveItem(model);

        if (valueObject.PrimaryValue == 0) { Series.Remove(typeSeries); }
      }
    }
  }
  #endregion
}
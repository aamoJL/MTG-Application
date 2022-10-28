using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.ComponentModel;
using System.Collections.Specialized;
using MTGApplication.ViewModels;
using static MTGApplication.Models.MTGCardModel;
using MTGApplication.Models;

namespace MTGApplication.Charts
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
    protected ObservableCollection<MTGCardModel> Models { get; }
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
    public virtual double SecondaryValue { get; }

    public MTGCardModelSeries(MTGCardModel model)
    {
      Models = new();
      Models.CollectionChanged += Models_CollectionChanged;

      AddItem(model);
    }

    public virtual void AddItem(MTGCardModel model)
    {
      if (Models.Contains(model)) { return; }
      Models.Add(model);

      model.PropertyChanged += Model_PropertyChanged;
    }
    public virtual void RemoveItem(MTGCardModel model)
    {
      if (!Models.Contains(model)) { return; }
      Models.Remove(model);

      model.PropertyChanged -= Model_PropertyChanged;
    }

    protected virtual void Model_PropertyChanged(object sender, PropertyChangedEventArgs e) { }
    protected virtual void Models_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }

    public static StackedColumnSeries<MTGCardModelSeries> CreateSeriesGroupByColor(ColorTypes color)
    {
      return new StackedColumnSeries<MTGCardModelSeries>
      {
        Name = color switch
        {
          ColorTypes.W => "White",
          ColorTypes.U => "Blue",
          ColorTypes.B => "Black",
          ColorTypes.R => "Red",
          ColorTypes.G => "Green",
          ColorTypes.M => "Multicolor",
          ColorTypes.C => "Colorless",
          _ => "Other",
        },
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
  public class MTGCardModelCMCSeries : MTGCardModelSeries
  {
    public MTGCardModelCMCSeries(MTGCardModel model) : base(model) { }

    public override double PrimaryValue { get => base.PrimaryValue; set => base.PrimaryValue = value; }
    public override double SecondaryValue => Models[0].Info.CMC;

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

    private void UpdateCardCount()
    {
      PrimaryValue = Models.Sum(x => x.Count);
    }
  }
  public class MTGCardModelSpellTypeSeries : MTGCardModelSeries
  {
    public MTGCardModelSpellTypeSeries(MTGCardModel model) : base(model) { }

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

    private void UpdateCardCount()
    {
      PrimaryValue = Models.Sum(x => x.Count);
    }
  }
  #endregion

  #region Charts
  public abstract class MTGCardModelChart
  {
    public ObservableCollection<ISeries> Series { get; }

    public MTGCardModelChart(ObservableCollection<MTGCardModel> models)
    {
      Series = new();
      models.CollectionChanged += Models_CollectionChanged;
    }

    protected virtual void Models_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          AddToChartSeries(e.NewItems[0] as MTGCardModel);
          break;
        case NotifyCollectionChangedAction.Remove:
          RemoveFromChartSeries(e.OldItems[0] as MTGCardModel);
          break;
        case NotifyCollectionChangedAction.Reset:
          Series.Clear();
          break;
        default:
          break;
      }
    }

    public abstract void AddToChartSeries(MTGCardModel model);
    public abstract void RemoveFromChartSeries(MTGCardModel model);
    public virtual void ClearChart()
    {
      Series.Clear();
    }
  }
  public class CMCChart : MTGCardModelChart
  {
    public CMCChart(ObservableCollection<MTGCardModel> models) : base(models) { }

    public override void AddToChartSeries(MTGCardModel model)
    {
      // Don't count Lands as a color
      if (model.SpellTypes.Contains(SpellType.Land)) { return; }

      // Find Color series
      if (Series.FirstOrDefault(x => x.Name == GetColorTypeName(model.ColorType)) is not StackedColumnSeries<MTGCardModelSeries> colorSeries)
      {
        // Create new series group if it does not exist
        colorSeries = MTGCardModelSeries.CreateSeriesGroupByColor(model.ColorType);
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
    public override void RemoveFromChartSeries(MTGCardModel model)
    {
      // Don't count Lands as a color
      if (model.SpellTypes.Contains(SpellType.Land)) { return; }

      // Find Color series
      if (Series.FirstOrDefault(x => x.Name == GetColorTypeName(model.ColorType)) is not StackedColumnSeries<MTGCardModelSeries> colorSeries)
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
  public class SpellTypeChart : MTGCardModelChart
  {
    public SpellTypeChart(ObservableCollection<MTGCardModel> models) : base(models) { }

    public override void AddToChartSeries(MTGCardModel model)
    {
      foreach (var spellType in model.SpellTypes)
      {
        // Find series
        if (Series.FirstOrDefault(x => x.Name == spellType.ToString()) is not PieSeries<MTGCardModelSeries> typeSeries)
        {
          // Create new series group if it does not exist
          typeSeries = MTGCardModelSeries.CreateSeriesGroupBySpellType(spellType);
          Series.Add(typeSeries);

          // Create new value object to the series
          (typeSeries.Values as ObservableCollection<MTGCardModelSeries>).Add(new MTGCardModelSpellTypeSeries(model));
        }
        else
        {
          // Add viewModel to the value object
          MTGCardModelSeries valueObject = (typeSeries.Values as ObservableCollection<MTGCardModelSeries>)[0];
          valueObject.AddItem(model);
        }
      }
    }
    public override void RemoveFromChartSeries(MTGCardModel model)
    {
      foreach (var spellType in model.SpellTypes)
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
  #endregion
}
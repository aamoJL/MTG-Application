using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MTGApplication.Models;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using static MTGApplication.Services.MTGService;

namespace MTGApplication.ViewModels.Charts;

/// <summary>
/// Base class for card model series
/// </summary>
public abstract class CardModelSeries<TModel> : ViewModelBase where TModel : ObservableObject
{
  public CardModelSeries(TModel model)
  {
    Models = new();
    Models.CollectionChanged += Models_CollectionChanged;

    AddItem(model);
  }

  protected double primaryValue;

  #region Properties
  protected ObservableCollection<TModel> Models { get; }
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
  #endregion

  #region OnPropertyChanged events
  protected virtual void Model_PropertyChanged(object sender, PropertyChangedEventArgs e) { }

  protected virtual void Models_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }
  #endregion

  /// <summary>
  /// Adds <paramref name="item"/> to the series
  /// </summary>
  public virtual void AddItem(TModel item)
  {
    if (Models.Contains(item)) { return; }
    Models.Add(item);

    item.PropertyChanged += Model_PropertyChanged;
  }

  /// <summary>
  /// Removes <paramref name="item"/> from the series
  /// </summary>
  public virtual void RemoveItem(TModel item)
  {
    if (!Models.Contains(item)) { return; }
    Models.Remove(item);

    item.PropertyChanged -= Model_PropertyChanged;
  }

  /// <summary>
  /// Returns StackedColumnSeries using the given <paramref name="color"/>
  /// </summary>
  public static StackedColumnSeries<CardModelSeries<TModel>> CreateStackedColumnSeriesByColor(ColorTypes color)
  {
    return new StackedColumnSeries<CardModelSeries<TModel>>
    {
      Name = color.GetFullName(),
      Values = new ObservableCollection<CardModelSeries<TModel>>(),
      Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
      Fill = color switch
      {
        ColorTypes.W => new SolidColorPaint(ChartColorPalette.White),
        ColorTypes.U => new SolidColorPaint(ChartColorPalette.Blue),
        ColorTypes.B => new SolidColorPaint(ChartColorPalette.Black),
        ColorTypes.R => new SolidColorPaint(ChartColorPalette.Red),
        ColorTypes.G => new SolidColorPaint(ChartColorPalette.Green),
        ColorTypes.M => new SolidColorPaint(ChartColorPalette.Multicolor),
        ColorTypes.C => new SolidColorPaint(ChartColorPalette.Colorless),
        _ => new SolidColorPaint(SKColors.Pink),
      },
      Padding = 0,
      MaxBarWidth = double.MaxValue,
      DataLabelsPaint = new SolidColorPaint(ChartColorPalette.LightThemeText),
      DataLabelsSize = 10,
      DataLabelsPosition = DataLabelsPosition.Middle,
      Mapping = (value, point) =>
      {
        point.Coordinate = new(value.SecondaryValue, value.PrimaryValue);
      }
    };
  }

  /// <summary>
  /// Returns PieSeries using the given <paramref name="color"/>
  /// </summary>
  public static PieSeries<CardModelSeries<TModel>> CreatePieSeriesByColor(ColorTypes color)
  {
    return new PieSeries<CardModelSeries<TModel>>
    {
      Name = color.GetFullName(),
      Values = new ObservableCollection<CardModelSeries<TModel>>(),
      Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
      Fill = color switch
      {
        ColorTypes.W => new SolidColorPaint(ChartColorPalette.White),
        ColorTypes.U => new SolidColorPaint(ChartColorPalette.Blue),
        ColorTypes.B => new SolidColorPaint(ChartColorPalette.Black),
        ColorTypes.R => new SolidColorPaint(ChartColorPalette.Red),
        ColorTypes.G => new SolidColorPaint(ChartColorPalette.Green),
        ColorTypes.C => new SolidColorPaint(ChartColorPalette.Colorless),
        ColorTypes.M => new SolidColorPaint(ChartColorPalette.Multicolor),
        _ => new SolidColorPaint(SKColors.Pink),
      },
      DataLabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor),
      DataLabelsSize = 10,
      DataLabelsPosition = PolarLabelsPosition.Outer,
      DataLabelsFormatter = p => p.Context.Series.Name,
      Mapping = (value, point) =>
      {
        point.Coordinate = new(value.SecondaryValue, value.PrimaryValue);
      },
    };
  }

  /// <summary>
  /// Returns PieSeries using the given <paramref name="spellType"/>
  /// </summary>
  public static PieSeries<CardModelSeries<TModel>> CreateSeriesGroupBySpellType(SpellType spellType)
  {
    return new PieSeries<CardModelSeries<TModel>>
    {
      Name = spellType.ToString(),
      Values = new ObservableCollection<CardModelSeries<TModel>>(),
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
      Mapping = (value, point) =>
      {
        point.Coordinate = new(value.SecondaryValue, value.PrimaryValue);
      }
    };
  }
}

/// <summary>
/// MTG card model series that has the sum of the card counts as the primary value
/// </summary>
public class MTGCardModelCountSeries : CardModelSeries<MTGCard>
{
  public MTGCardModelCountSeries(MTGCard model) : base(model) { }

  #region OnPropertyChanged events
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
  #endregion

  /// <summary>
  /// Updates the primary value
  /// </summary>
  protected void UpdateCardCount() => PrimaryValue = Models.Sum(x => x.Count);
}

/// <summary>
/// MTG card model series that has the sum of the card counts as the primary value
/// and model CMC as the secondary value
/// </summary>
public class MTGCardModelCMCSeries : MTGCardModelCountSeries
{
  public MTGCardModelCMCSeries(MTGCard model) : base(model) => SecondaryValue = model.Info.CMC;
}
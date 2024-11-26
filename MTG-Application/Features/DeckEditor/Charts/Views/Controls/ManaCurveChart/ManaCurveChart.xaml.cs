using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MTGApplication.Features.DeckEditor.Charts.Models;
using MTGApplication.Features.DeckEditor.Charts.Views.Controls;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Extensions;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using static MTGApplication.General.Models.MTGCardInfo;

namespace MTGApplication.Features.DeckEditor;
public sealed partial class ManaCurveChart : MTGCardChart
{
  public ManaCurveChart() : base() => InitializeComponent();

  public ICartesianAxis[] XAxes { get; set; } =
  [
    new Axis() {
      LabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor),
      ForceStepToMin = true,
      MinStep = 1,
    }
  ];
  public ICartesianAxis[] YAxes { get; set; } =
  [
    new Axis() {
      LabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor),
      ForceStepToMin = true,
      MinStep = 5,
      MinLimit = 0,
    }
  ];

  protected override void AddToSeries(DeckEditorMTGCard card)
  {
    // Validation: Exclude cards that are only Lands
    if (card.Info.SpellTypes.All(x => x == SpellType.Land))
      return;

    // Cards with multiple colors will be shown as a Multicolored
    var color = card.Info.Colors.Length > 1 ? ColorTypes.M : card.Info.Colors[0];

    // Find series or create a new one
    var series = Series.FirstOrDefault(x => x.Name == color.GetFullName()) ?? AddNewSeries(color);

    if (series.Values is ObservableCollection<MTGCardCMCSeriesItem> seriesValues)
    {
      // Find series item or create a new one
      var valueItem = seriesValues.FirstOrDefault(x => x.CMC == card.Info.CMC)
        ?? new Func<MTGCardCMCSeriesItem>(() =>
        {
          var newSeriesItem = new MTGCardCMCSeriesItem(card.Info.CMC);
          seriesValues.Add(newSeriesItem);
          return newSeriesItem;
        }).Invoke();

      // Add card to the series item
      valueItem.Cards.Add(card);
    }
  }

  protected override void RemoveFromSeries(DeckEditorMTGCard card)
  {
    // Cards with multiple colors will be shown as a Multicolored
    var color = card.Info.Colors.Length > 1 ? ColorTypes.M : card.Info.Colors[0];

    // Find value item
    if (Series.FirstOrDefault(x => x.Name == color.GetFullName()) is ISeries series
      && series.Values is ObservableCollection<MTGCardCMCSeriesItem> seriesValues
      && seriesValues.FirstOrDefault(x => x.Cards.Contains(card)) is MTGCardCMCSeriesItem valueItem)
    {
      // Remove card from the value item
      valueItem.Cards.Remove(card);

      // Remove value item if empty
      if (valueItem.Cards.Count == 0)
        seriesValues.Remove(valueItem);

      // Remove series if empty
      if (seriesValues.Count == 0)
        Series.Remove(series);
    }
  }

  protected override ISeries AddNewSeries(object color)
  {
    var series = new StackedColumnSeries<MTGCardCMCSeriesItem>
    {
      Name = ((ColorTypes)color).GetFullName(),
      Values = new ObservableCollection<MTGCardCMCSeriesItem>(),
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
      Mapping = (value, _) => new(value.CMC, value.Count)
    };

    Series.Add(series);
    SortSeries();

    return series;
  }

  private void SortSeries()
  {
    var tempList = Series.OrderBy(x => x.Name).ToList();

    for (var i = 0; i < tempList.Count; i++)
      Series.Move(Series.IndexOf(tempList[i]), i);
  }

  /// <summary>
  /// Sets Axis colors for the selected theme
  /// </summary>
  protected override void UpdateTheme()
  {
    if (XAxes[0] is Axis x)
      x.LabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor);
    if (YAxes[0] is Axis y)
      y.LabelsPaint = new SolidColorPaint(ChartColorPalette.ForegroundColor);
  }
}
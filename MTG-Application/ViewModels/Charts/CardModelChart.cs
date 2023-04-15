using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace MTGApplication.ViewModels.Charts;

/// <summary>
/// Base class for MTG card charts
/// </summary>
public abstract class CardModelChart<TPrimaryType, TModel> where TModel : ObservableObject
{
  protected ObservableCollection<TModel> models = new();

  public ObservableCollection<ISeries> Series { get; } = new();
  public ObservableCollection<TModel> Models
  {
    get => models;
    init
    {
      models = value;
      foreach (var model in models)
      {
        AddToChartSeries(model);
      }

      models.CollectionChanged += Models_CollectionChanged;
    }
  }
  public abstract bool HasSecondaryValues { get; }

  public CardModelChart() { }

  protected virtual void Models_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case NotifyCollectionChangedAction.Add:
        AddToChartSeries(e.NewItems[0] as TModel);
        break;
      case NotifyCollectionChangedAction.Remove:
        RemoveFromChartSeries(e.OldItems[0] as TModel);
        break;
      case NotifyCollectionChangedAction.Reset:
        Series.Clear();
        break;
      default:
        break;
    }
  }

  /// <summary>
  /// Adds series to the chart and sorts all the series
  /// </summary>
  protected void AddAndSortSeries(ISeries series)
  {
    Series.Add(series);
    SortSeries();
  }

  /// <summary>
  /// Adds model to the chart
  /// </summary>
  protected virtual void AddToChartSeries(TModel model)
  {
    if (!ModelValidation(model))
    { return; }

    foreach (var primaryItem in GetPrimaryItems(model))
    {
      var series = FindPrimarySeries(model, primaryItem);

      if (series == null)
      {
        series = CreateNewSeries(primaryItem);
        AddAndSortSeries(series);
      }

      if (HasSecondaryValues)
      {
        if (FindSecondaryItem(series, model) is CardModelSeries<TModel> secondaryItem)
        {
          // Add to existing item
          secondaryItem.AddItem(model);
        }
        else
        {
          (series.Values as ObservableCollection<CardModelSeries<TModel>>).Add(CreateNewSecondarySeries(model));
        }
      }
      else
      {
        if (series.Values is ObservableCollection<CardModelSeries<TModel>> values)
        {
          if (values.Count == 0)
          {
            values.Add(CreateNewSecondarySeries(model));
          }
          else
          {
            // Add model to the value object
            values[0].AddItem(model);
          }
        }
      }
    }
  }

  /// <summary>
  /// Clears series
  /// </summary>
  protected virtual void ClearChart() => Series.Clear();

  /// <summary>
  /// Sorts series by name
  /// </summary>
  protected void SortSeries()
  {
    var tempList = Series.OrderBy(x => x.Name).ToList();

    for (var i = 0; i < tempList.Count; i++)
    {
      Series.Move(Series.IndexOf(tempList[i]), i);
    }
  }

  /// <summary>
  /// Returns <see langword="true"/>, if the model is valid for the chart
  /// </summary>
  protected virtual bool ModelValidation(TModel model) => true;

  /// <summary>
  /// Returns existing <see cref="ISeries"/> object
  /// </summary>
  protected abstract ISeries FindPrimarySeries(TModel model, TPrimaryType item);

  /// <summary>
  /// Returns array of properties from <paramref name="model"/> that will be used to populate the chart series
  /// </summary>
  protected abstract TPrimaryType[] GetPrimaryItems(TModel model);

  /// <summary>
  /// Returns new <see cref="ISeries"/>
  /// </summary>
  protected abstract ISeries CreateNewSeries(TPrimaryType item);

  /// <summary>
  /// Returns secondary series from the <paramref name="series"/> object's values.
  /// </summary>
  protected abstract CardModelSeries<TModel> FindSecondaryItem(ISeries series, TModel model);

  /// <summary>
  /// Returns new secondary series
  /// </summary>
  protected abstract CardModelSeries<TModel> CreateNewSecondarySeries(TModel model);

  /// <summary>
  /// Removes model from the chart
  /// </summary>
  protected virtual void RemoveFromChartSeries(TModel model)
  {
    if (!ModelValidation(model))
    { return; }

    foreach (var primaryItem in GetPrimaryItems(model))
    {
      var series = FindPrimarySeries(model, primaryItem);
      if (series == null)
      { return; }

      if (HasSecondaryValues)
      {
        if (FindSecondaryItem(series, model) is CardModelSeries<TModel> secondaryItem)
        {
          secondaryItem.RemoveItem(model);
          if (secondaryItem.PrimaryValue == 0)
          { (series.Values as ObservableCollection<CardModelSeries<TModel>>).Remove(secondaryItem); } // remove secondary item if its count is zero
        }

        if ((series.Values as ObservableCollection<CardModelSeries<TModel>>).Count == 0)
        {
          Series.Remove(series);
        }
      }
      else
      {
        var valueObject = (series.Values as ObservableCollection<CardModelSeries<TModel>>)[0];
        valueObject.RemoveItem(model);
        if (valueObject.PrimaryValue == 0)
        { Series.Remove(series); }
      }
    }
  }
}
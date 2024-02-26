using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace MTGApplication.ViewModels.Charts;

/// <summary>
/// Base class for card charts
/// </summary>
public abstract class CardModelChart<TPrimaryType, TModel> where TModel : ObservableObject
{
  public CardModelChart() { }

  protected ObservableCollection<TModel> models = new();

  #region Properties
  public ObservableCollection<ISeries> Series { get; } = new();
  public virtual ObservableCollection<TModel> Models
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
  public abstract bool HasSecondarySeriesItems { get; }
  public virtual bool RemoveEmptySeries { get; } = true;
  #endregion

  #region OnPropertyChanged events
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
  #endregion

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

    var primaryProperties = GetPrimaryProperties(model);
    foreach (var primaryProperty in primaryProperties)
    {
      var series = FindSeries(model, primaryProperty);

      if (series == null)
      {
        series = CreateNewSeries(primaryProperty);
        AddAndSortSeries(series);
      }

      if (HasSecondarySeriesItems)
      {
        if (series.Values is ObservableCollection<CardModelSeriesItem<TModel>> primaryValues && FindSecondarySeriesItem(primaryValues, primaryProperty, model)
          is CardModelSeriesItem<TModel> secondaryItem)
        {
          // Add to existing item
          secondaryItem.AddItem(model);
        }
        else
        {
          (series.Values as ObservableCollection<CardModelSeriesItem<TModel>>).Add(CreateNewSecondarySeriesItem(model));
        }
      }
      else
      {
        if (series.Values is ObservableCollection<CardModelSeriesItem<TModel>> values)
        {
          if (values.Count == 0)
          {
            var secondarySeries = CreateNewSecondarySeriesItem(model);
            if (secondarySeries != null) values.Add(secondarySeries);
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
  /// Returns array of properties from <paramref name="model"/> that will be used to populate the chart series
  /// </summary>
  protected abstract TPrimaryType[] GetPrimaryProperties(TModel model);

  /// <summary>
  /// Returns existing <see cref="ISeries"/> object
  /// </summary>
  protected abstract ISeries FindSeries(TModel model, TPrimaryType item);

  /// <summary>
  /// Returns new <see cref="ISeries"/>
  /// </summary>
  protected abstract ISeries CreateNewSeries(TPrimaryType item);

  /// <summary>
  /// Returns secondary series from the <paramref name="series"/> object's values.
  /// </summary>
  protected abstract CardModelSeriesItem<TModel> FindSecondarySeriesItem(ObservableCollection<CardModelSeriesItem<TModel>> items, TPrimaryType primaryProperty, TModel model);

  /// <summary>
  /// Returns new secondary series
  /// </summary>
  protected abstract CardModelSeriesItem<TModel> CreateNewSecondarySeriesItem(TModel model);

  /// <summary>
  /// Removes model from the chart
  /// </summary>
  protected virtual void RemoveFromChartSeries(TModel model)
  {
    if (!ModelValidation(model)) { return; }

    foreach (var primaryProperty in GetPrimaryProperties(model))
    {
      var series = FindSeries(model, primaryProperty);
      if (series == null) { return; }

      if (HasSecondarySeriesItems)
      {
        if (series.Values is ObservableCollection<CardModelSeriesItem<TModel>> primaryValues && FindSecondarySeriesItem(primaryValues, primaryProperty, model)
          is CardModelSeriesItem<TModel> secondaryItem)
        {
          secondaryItem.RemoveItem(model);
          if (RemoveEmptySeries && secondaryItem.PrimaryValue == 0)
          {
            // remove secondary item if its count is zero
            (series.Values as ObservableCollection<CardModelSeriesItem<TModel>>).Remove(secondaryItem);
          }
        }

        if (RemoveEmptySeries && (series.Values as ObservableCollection<CardModelSeriesItem<TModel>>).Count == 0)
        {
          Series.Remove(series);
        }
      }
      else
      {
        var valueObject = (series.Values as ObservableCollection<CardModelSeriesItem<TModel>>)[0];
        valueObject.RemoveItem(model);
        if (RemoveEmptySeries && valueObject.PrimaryValue == 0) { Series.Remove(series); }
      }
    }
  }
}
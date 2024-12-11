// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Linq;

namespace MTGApplication.General.Views.Controls.AdaptiveGridView;

internal partial class AdaptiveHeightValueConverter : IValueConverter
{
  private Thickness thickness = new(0, 0, 4, 4);

  public Thickness DefaultItemMargin
  {
    get => thickness;
    set => thickness = value;
  }

  public object Convert(object value, Type targetType, object parameter, string language)
  {
    if (value != null)
    {
      var gridView = (GridView)parameter;
      if (gridView == null)
      {
        return value;
      }

      _ = double.TryParse(value.ToString(), out var height);

      var padding = gridView.Padding;
      var margin = GetItemMargin(gridView, DefaultItemMargin);
      height = height + margin.Top + margin.Bottom + padding.Top + padding.Bottom;

      return height;
    }

    return double.NaN;
  }

  public object ConvertBack(object value, Type targetType, object parameter, string language)
    => throw new NotImplementedException();

  internal static Thickness GetItemMargin(GridView view, Thickness fallback = default)
  {
    var setter = view.ItemContainerStyle?.Setters.OfType<Setter>().FirstOrDefault(s => s.Property == FrameworkElement.MarginProperty);
    if (setter != null)
    {
      return (Thickness)setter.Value;
    }
    else
    {
      if (view.Items.Count > 0)
      {
        var container = (GridViewItem)view.ContainerFromIndex(0);
        if (container != null)
        {
          return container.Margin;
        }
      }

      // Use the default thickness for a GridViewItem
      return fallback;
    }
  }
}
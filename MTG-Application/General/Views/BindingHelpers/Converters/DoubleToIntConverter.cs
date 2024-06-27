using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.General.Views.BindingHelpers.Converters;

/// <summary>
/// Converts <see cref="double"/> to <see cref="int"/>
/// </summary>
public class DoubleToIntConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language) => (int)(double)value;

  public object ConvertBack(object value, Type targetType, object parameter, string language) => (double)value;
}

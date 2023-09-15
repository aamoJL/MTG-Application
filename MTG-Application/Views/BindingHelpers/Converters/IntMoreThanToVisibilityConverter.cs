using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.Views.BindingHelpers.Converters;

public class IntMoreThanToVisibilityConverter : IValueConverter
{
  /// <summary>
  /// Returns <see cref="Visibility"/> depending on the <paramref name="value"/> being more than the <paramref name="parameter"/>
  /// </summary>
  /// <param name="parameter">value being on the right side of the More than comparison</param>
  public object Convert(object value, Type targetType, object parameter, string language)
  {
    if (double.TryParse((string)parameter, out var comparer))
    {
      return value is not null and int number && number > comparer ? Visibility.Visible : Visibility.Collapsed;
    }
    return Visibility.Collapsed;
  }

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.Views.BindingHelpers.Converters;

public class BoolToIntConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language)
  {
    if (parameter != null && int.TryParse((string)parameter, out var angle))
    {
      return (bool)value ? angle : 0;
    }
    return 0;
  }

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
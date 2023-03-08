using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.Views.BindingHelpers.Converters
{
  public class DoubleToIntConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      return (int)(double)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      return (double)value;
    }
  }
}

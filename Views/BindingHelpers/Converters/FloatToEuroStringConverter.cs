using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.Views.BindingHelpers.Converters
{
  public class FloatToEuroStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      return Format.EuroToString((float)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}

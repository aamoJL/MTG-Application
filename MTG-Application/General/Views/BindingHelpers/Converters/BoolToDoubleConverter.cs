using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.General.Views.BindingHelpers.Converters;

public partial class BoolToDoubleConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language)
    => parameter != null && double.TryParse((string)parameter, out var angle) ? (bool)value ? angle : 0 : 0;

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
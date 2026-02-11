using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.General.Views.BindingHelpers.Converters;

public partial class NullToBooleanConverter : IValueConverter
{
  public bool Invert { get; set; } = false;

  public object Convert(object value, Type targetType, object parameter, string language)
  {
    return Invert
      ? value != null
      : value == null;
  }

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}

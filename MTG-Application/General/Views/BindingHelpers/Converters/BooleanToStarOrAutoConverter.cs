using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.General.Views.BindingHelpers.Converters;

public partial class BooleanToStarOrAutoConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language)
    => value is true ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
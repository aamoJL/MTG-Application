using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.General.Views.BindingHelpers;

public class EnumToVisibilityConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language) 
    => value.ToString() == parameter.ToString() ? Visibility.Visible : Visibility.Collapsed;

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
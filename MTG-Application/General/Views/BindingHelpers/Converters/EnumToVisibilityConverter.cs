using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.General.Views.BindingHelpers.Converters;

public partial class EnumToVisibilityConverter : IValueConverter
{
  public bool Invert { get; set; } = false;

  public object Convert(object value, Type targetType, object parameter, string language)
  {
    return Invert
      ? value?.ToString() != parameter?.ToString() ? Visibility.Visible : Visibility.Collapsed
      : value?.ToString() == parameter?.ToString() ? Visibility.Visible : Visibility.Collapsed;
  }

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
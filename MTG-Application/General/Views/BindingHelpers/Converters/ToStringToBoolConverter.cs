using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.General.Views.BindingHelpers.Converters;

public partial class ToStringToBoolConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language) => value?.ToString() == parameter.ToString();

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
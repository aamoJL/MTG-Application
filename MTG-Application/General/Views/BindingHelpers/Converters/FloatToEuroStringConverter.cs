using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.General.Views.BindingHelpers.Converters;

/// <summary>
/// Converts <see cref="float"/> to euro currency formatter <see cref="string"/>
/// </summary>
public partial class FloatToEuroStringConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language) => Format.EuroToString((float)value);

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}

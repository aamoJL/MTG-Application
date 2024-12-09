using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace MTGApplication.General.Views.BindingHelpers.Converters;

/// <summary>
/// Converts <see cref="string"/> uri to <see cref="BitmapImage"/>
/// </summary>
public partial class StringToBitmapConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language)
  {
    if (value is string uri)
      return new BitmapImage(new Uri(uri));
    else
      return new BitmapImage();
  }

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
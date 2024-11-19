using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.Features.DeckEditor.Editor.Views.Converters;

public partial class StringOrDefaultConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language)
    => (value is not string text || string.IsNullOrEmpty(text)) ? parameter : value;

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}

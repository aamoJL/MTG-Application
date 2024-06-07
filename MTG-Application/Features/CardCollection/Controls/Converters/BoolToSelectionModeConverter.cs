using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.Features.CardCollection.Controls;

public class BoolToSelectionModeConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language)
  {
    if (value is true) return SelectionMode.SingleTap;
    return SelectionMode.DoubleTap;
  }

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}

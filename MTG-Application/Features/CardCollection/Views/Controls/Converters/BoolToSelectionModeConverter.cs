using Microsoft.UI.Xaml.Data;
using MTGApplication.Features.CardCollection.Views.Controls;
using System;

namespace MTGApplication.Features.CardCollection.Views.Controls.Converters;

public class BoolToSelectionModeConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language)
    => value is true ? SelectionMode.SingleTap : SelectionMode.DoubleTap;

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.Views.Controls.Converters;

public partial class BoolToSelectionModeConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language)
    => value is true ? SelectionMode.SingleTap : SelectionMode.DoubleTap;

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using static MTGApplication.ViewModels.MTGCardCollectionViewModel;

namespace MTGApplication.Converters
{
  internal class DisplayTypeToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      if(value.GetType() != typeof(DisplayTypes)) { return DependencyProperty.UnsetValue; }
      
      return parameter.ToString() == value.ToString() ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}

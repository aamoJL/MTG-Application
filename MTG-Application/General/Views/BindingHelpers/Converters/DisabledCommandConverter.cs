using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.General.Views.BindingHelpers.Converters;

public partial class DisabledCommandConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language)
  {
    if (value == null)
      return new RelayCommand(
        execute: () => throw new NotImplementedException(),
        canExecute: () => false);
    else
      return value;
  }

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}

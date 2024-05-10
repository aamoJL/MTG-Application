﻿using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.General.Views.BindingHelpers;

public class NotNullToBooleanConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language) => value is not null;

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}

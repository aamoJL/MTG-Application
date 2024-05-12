﻿using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.General.Views.BindingHelpers;

public class PlusCounterConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language)
    => value is not null and int count ? $"+{count}" : "";

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
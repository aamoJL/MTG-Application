using Microsoft.UI.Xaml.Data;
using System;
using static MTGApplication.Features.DeckEditor.CardList.Services.CardFilters;

namespace MTGApplication.Features.DeckEditor.Editor.Views.Converters;

public partial class CardColorGroupToBoolConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language)
  {
    if (value is not ColorGroups group || parameter is not string targetGroup)
      return false;

    return group.ToString() == targetGroup;
  }

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}

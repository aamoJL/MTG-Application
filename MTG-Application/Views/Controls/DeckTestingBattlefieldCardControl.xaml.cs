using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using MTGApplication.ViewModels;
using System;

namespace MTGApplication.Views.Controls;
public sealed partial class DeckTestingBattlefieldCardControl : UserControl
{
  public DeckTestingBattlefieldCardControl() => InitializeComponent();

  public float CardWidth
  {
    get => (float)GetValue(CardWidthProperty);
    set => SetValue(CardWidthProperty, value);
  }

  public float CardHeight
  {
    get => (float)GetValue(CardHeightProperty);
    set => SetValue(CardHeightProperty, value);
  }

  public static readonly DependencyProperty CardWidthProperty =
      DependencyProperty.Register(nameof(CardWidth), typeof(float), typeof(DeckTestingBattlefieldCardControl), new PropertyMetadata(0));
  
  public static readonly DependencyProperty CardHeightProperty =
      DependencyProperty.Register(nameof(CardHeight), typeof(float), typeof(DeckTestingBattlefieldCardControl), new PropertyMetadata(0));

  private void Root_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
  {
    if(DataContext is DeckTestingMTGCardViewModel card and not null)
    {
      card.IsTapped = !card.IsTapped;
    }
  }
}

#region Value Converters
public class PlusCounterConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language) 
    => value is not null and int count ? $"+{count}" : "";

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}

public class IntMoreThanToVisibilityConverter : IValueConverter
{
  /// <summary>
  /// Returns <see cref="Visibility"/> depending on the <paramref name="value"/> being more than the <paramref name="parameter"/>
  /// </summary>
  /// <param name="parameter">value being on the right side of the More than comparison</param>
  public object Convert(object value, Type targetType, object parameter, string language)
  {
    if (double.TryParse((string)parameter, out var comparer))
    {
      return value is not null and int number && number > comparer ? Visibility.Visible : Visibility.Collapsed;
    }
    return Visibility.Collapsed;
  }

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}

public class BoolToIntConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language)
  {
    if (parameter != null && int.TryParse((string)parameter, out var angle))
    {
      return (bool)value ? angle : 0;
    }
    return 0;
  }

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
#endregion
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace MTGApplication.Features.DeckEditor;
public sealed partial class DeckEditorCardTextView : DeckEditorCardViewBase
{
  public static readonly DependencyProperty SetIconVisibleProperty =
      DependencyProperty.Register(nameof(SetIconVisible), typeof(bool), typeof(DeckEditorCardTextView), new PropertyMetadata(true));

  public static readonly DependencyProperty TypeLineVisibleProperty =
      DependencyProperty.Register(nameof(TypeLineVisible), typeof(bool), typeof(DeckEditorCardTextView), new PropertyMetadata(true));

  public DeckEditorCardTextView() => InitializeComponent();

  public bool SetIconVisible
  {
    get => (bool)GetValue(SetIconVisibleProperty);
    set => SetValue(SetIconVisibleProperty, value);
  }
  public bool TypeLineVisible
  {
    get => (bool)GetValue(TypeLineVisibleProperty);
    set => SetValue(TypeLineVisibleProperty, value);
  }
}

public class BooleanToStarOrAutoConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, string language)
    => value is true ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;

  public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}

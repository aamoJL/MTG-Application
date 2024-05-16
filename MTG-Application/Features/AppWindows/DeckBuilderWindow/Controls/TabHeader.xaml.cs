using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow.Controls;
public sealed partial class TabHeader : UserControl
{
  public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register(nameof(Text), typeof(string), typeof(TabHeader), new PropertyMetadata(string.Empty));

  public static readonly DependencyProperty UnsavedIndicatorProperty =
      DependencyProperty.Register(nameof(UnsavedIndicator), typeof(bool), typeof(TabHeader), new PropertyMetadata(false));

  public TabHeader() => InitializeComponent();

  public string Text
  {
    get => (string)GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }

  public bool UnsavedIndicator
  {
    get => (bool)GetValue(UnsavedIndicatorProperty);
    set => SetValue(UnsavedIndicatorProperty, value);
  }
}

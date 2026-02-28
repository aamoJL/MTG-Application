using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.Features.DeckEditor.Views.Controls;

public sealed partial class CardGroupViewGroupHeader : UserControl
{
  public static readonly DependencyProperty IsExpandedProperty =
      DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(CardGroupViewGroupHeader), new PropertyMetadata(true, OnExpandedChanged));

  public CardGroupViewGroupHeader() => InitializeComponent();

  public bool IsExpanded
  {
    get => (bool)GetValue(IsExpandedProperty);
    set => SetValue(IsExpandedProperty, value);
  }

  private void ExpandedChanged() => ChevronStoryboard.Begin();

  private static void OnExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs _)
    => (d as CardGroupViewGroupHeader)?.ExpandedChanged();
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls;

public sealed partial class CardGroupViewGroupHeader : UserControl
{
  public static readonly DependencyProperty PrimaryHeaderProperty =
      DependencyProperty.Register(nameof(PrimaryHeader), typeof(string), typeof(CardGroupViewGroupHeader), new PropertyMetadata(string.Empty));

  public static readonly DependencyProperty SecondaryHeaderProperty =
      DependencyProperty.Register(nameof(SecondaryHeader), typeof(string), typeof(CardGroupViewGroupHeader), new PropertyMetadata(string.Empty));

  public static readonly DependencyProperty IsExpandedProperty =
      DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(CardGroupViewGroupHeader), new PropertyMetadata(true, OnExpandedChanged));

  public CardGroupViewGroupHeader() => InitializeComponent();

  public string PrimaryHeader
  {
    get => (string)GetValue(PrimaryHeaderProperty);
    set => SetValue(PrimaryHeaderProperty, value);
  }

  public string SecondaryHeader
  {
    get => (string)GetValue(SecondaryHeaderProperty);
    set => SetValue(SecondaryHeaderProperty, value);
  }

  public bool IsExpanded
  {
    get => (bool)GetValue(IsExpandedProperty);
    set => SetValue(IsExpandedProperty, value);
  }

  private void ExpandedChanged()
    => ChevronStoryboard.Begin();

  private static void OnExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    => (d as CardGroupViewGroupHeader)?.ExpandedChanged();
}

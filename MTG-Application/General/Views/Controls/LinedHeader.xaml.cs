using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.General.Views.Controls;

public sealed partial class LinedHeader : UserControl
{
  public static readonly DependencyProperty HeaderProperty =
      DependencyProperty.Register(nameof(Header), typeof(TextBlock), typeof(LinedHeader), new PropertyMetadata(null));

  public LinedHeader() => InitializeComponent();

  public TextBlock Header
  {
    get => (TextBlock)GetValue(HeaderProperty);
    set => SetValue(HeaderProperty, value);
  }
}

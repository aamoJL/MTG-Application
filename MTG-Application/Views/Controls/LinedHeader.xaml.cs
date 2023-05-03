using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.Views.Controls;
public sealed partial class LinedHeader : UserControl
{
  public LinedHeader() => InitializeComponent();

  public TextBlock Header
  {
    get => (TextBlock)GetValue(HeaderProperty);
    set => SetValue(HeaderProperty, value);
  }

  // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
  public static readonly DependencyProperty HeaderProperty =
      DependencyProperty.Register("Header", typeof(TextBlock), typeof(LinedHeader), new PropertyMetadata(null));
}

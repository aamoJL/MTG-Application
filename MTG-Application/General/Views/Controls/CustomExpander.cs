using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.General.Views.Controls;

public partial class CustomExpander : Expander
{
  public CustomExpander()
  {
    RegisterPropertyChangedCallback(IsExpandedProperty, (d, e) =>
    {
      var state = IsExpanded ? "ExpandDown" : "CollapseUp";
      VisualStateManager.GoToState(this, state, true);
    });

    Loaded += CustomExpander_Loaded;
  }

  private bool PointerPressing { get; set; } = false;

  private void CustomExpander_Loaded(object sender, RoutedEventArgs e)
  {
    Loaded -= CustomExpander_Loaded;

    if (Header is FrameworkElement header)
    {
      header.PointerPressed += (_, e) => PointerPressing = e.GetCurrentPoint(null).Properties.IsLeftButtonPressed;
      header.PointerReleased += (_, _) =>
      {
        if (PointerPressing)
          IsExpanded = !IsExpanded;
      };
      header.PointerExited += (_, _) => PointerPressing = false;
    }
  }
}

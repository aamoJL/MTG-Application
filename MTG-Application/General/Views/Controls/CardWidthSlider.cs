using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace MTGApplication.General.Views.Controls;

/// <summary>
/// Slider that uses the default card witdh value from LocalSettings
/// </summary>
public partial class CardWidthSlider : Slider
{
  public CardWidthSlider()
  {
    Minimum = 140;
    Maximum = 350;
    Value = AppConfig.LocalSettings.DefaultCardImageWidth;
    TickFrequency = 10;
  }

  protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
  {
    base.OnPointerCaptureLost(e);

    e.Handled = true;

    AppConfig.LocalSettings.DefaultCardImageWidth = (ushort)Value;
  }
}

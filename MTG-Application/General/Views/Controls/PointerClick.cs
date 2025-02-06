using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;

namespace MTGApplication.General.Views.Controls;

public class PointerClick
{
  private object? PressedElement { get; set; }

  public event EventHandler<PointerRoutedEventArgs>? Clicked;

  public void Register(UIElement element)
  {
    element.PointerExited += Element_PointerExited;
    element.PointerPressed += Element_PointerPressed;
    element.PointerReleased += Element_PointerReleased;
  }

  public void Unregister(UIElement element)
  {
    element.PointerExited -= Element_PointerExited;
    element.PointerPressed -= Element_PointerPressed;
    element.PointerReleased -= Element_PointerReleased;
  }

  private void Element_PointerExited(object sender, PointerRoutedEventArgs e)
  {
    if (sender == PressedElement)
      PressedElement = null;
  }

  private void Element_PointerPressed(object sender, PointerRoutedEventArgs e)
    => PressedElement = sender;

  private void Element_PointerReleased(object sender, PointerRoutedEventArgs e)
  {
    if (PressedElement == sender)
      Clicked?.Invoke(sender, e);

    PressedElement = null;

    e.Handled = true;
  }
}

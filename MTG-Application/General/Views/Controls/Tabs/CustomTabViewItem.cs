using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace MTGApplication.General.Views.Controls;

/// <summary>
/// Wrapper for frame and header for <see cref="TabViewItem"/> so the item content can be styled on template
/// </summary>
public class CustomTabViewItem
{
  public FrameworkElement Header = new TextBlock() { Text = "New tab" };
  public Frame Frame { get; set; }

  public event EventHandler OnClosed;

  public void Close() => OnClosed?.Invoke(this, EventArgs.Empty);
}

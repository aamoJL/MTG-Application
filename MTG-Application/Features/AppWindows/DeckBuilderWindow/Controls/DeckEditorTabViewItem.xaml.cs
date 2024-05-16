using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.Features.AppWindows.DeckBuilderWindow.Controls;
public sealed partial class DeckEditorTabViewItem : TabViewItem
{
  public DeckEditorTabViewItem() => InitializeComponent();

  public Frame Frame => ContentFrame;

  public new TabHeader Header { get; set; } = new TabHeader() { Text = "New tab" };
}

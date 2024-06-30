using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MTGApplication.General.Views.Extensions;
using System;

namespace MTGApplication.General.Views.Dialogs.Controls;
public sealed partial class GridViewDialog : ObjectDialog
{
  public GridViewDialog(string title, object[] items, DataTemplate itemTemplate) : base(title)
  {
    InitializeComponent();

    Items = items;
    ItemTemplate = itemTemplate;

    SecondaryButtonText = string.Empty;
  }

  public object[] Items { get; }
  public DataTemplate ItemTemplate { get; }
  public object Selection { get; set; }
  public bool CanDragItems { get; set; } = false;

  public Action<DragItemsStartingEventArgs> OnItemDragStarting { get; set; }

  protected override object ProcessResult(ContentDialogResult result)
    => result switch
    {
      ContentDialogResult.Primary => Selection,
      _ => null
    };

  private void GridView_DoubleTapped(object _, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
  {
    var root = VisualTreeHelper.GetParent(this);
    var primaryButton = root.FindChildByName("PrimaryButton") as Button;

    if (FrameworkElementAutomationPeer.FromElement(primaryButton) is ButtonAutomationPeer primaryFeap)
      primaryFeap?.Invoke(); // Click the primary button
    else
      Hide(); // If primary button is not available, close the dialog
  }

  private void GridView_DragItemsStarting(object _, DragItemsStartingEventArgs e)
  {
    Hide();

    OnItemDragStarting?.Invoke(e);
  }
}

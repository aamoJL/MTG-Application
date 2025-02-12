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

    PointerExited += GridViewDialog_PointerExited;
  }

  public object[] Items { get; }
  public DataTemplate ItemTemplate { get; }
  public object? Selection { get; set; }
  public bool CanDragItems { get; set; } = false;
  public bool CanSelectItems { get; set; } = true;

  private ListViewSelectionMode SelectionMode => CanSelectItems ? ListViewSelectionMode.Single : ListViewSelectionMode.None;

  public Action<DragItemsStartingEventArgs>? OnItemDragStarting { get; set; }

  private bool IsDraggingItem { get; set; } = false;

  protected override object? ProcessResult(ContentDialogResult result)
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
    OnItemDragStarting?.Invoke(e);

    // Hiding the dialog here sometime causes Access Violation Exception to be thrown that can't be caught,
    //  so the dragging needs to be tracked with the draggingItem variable and hide the dialog on pointerExited.
    IsDraggingItem = true;
  }

  private void GridViewDialog_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (IsDraggingItem)
      Hide();
  }
}

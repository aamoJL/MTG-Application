using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.Views.Controls;
using MTGApplication.Features.DeckTesting.Views.Controls.CardView;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.Features.DeckTesting.Services;

public class DeckTestingPointerEvents
{
  public void Droppable_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs _)
  {
    if (DeckTestingCardDrag.IsDragging && sender is FrameworkElement element)
      DragCardPreview.Change(this, new(element.XamlRoot) { Opacity = DragCardPreview.DroppableOpacity });
  }

  public void Droppable_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs _)
  {
    if (DeckTestingCardDrag.IsDragging && sender is FrameworkElement element)
      DragCardPreview.Change(this, new(element.XamlRoot) { Opacity = DragCardPreview.UndroppableOpacity });
  }

  public void DroppableListView_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (!DeckTestingCardDrag.IsDragging || DeckTestingCardDrag.Item is not DeckTestingMTGCard item)
      return;

    // Tokens will not be added to the list, but they will be removed from the battlefield
    if (item.IsToken)
      DeckTestingCardDrag.Complete();
    else if (sender is ListViewBase list)
      DropAndReorder(item: item, target: list, e: e);
  }

  public void LibraryTop_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs _)
  {
    if (!DeckTestingCardDrag.IsDragging || DeckTestingCardDrag.Item is not DeckTestingMTGCard item)
      return;

    // Tokens will not be added to the list, but they will be removed from the battlefield
    if (!item.IsToken && sender is FrameworkElement element)
      (element.DataContext as ObservableCollection<DeckTestingMTGCard>)?.Insert(0, item);

    DeckTestingCardDrag.Complete();
  }

  public void LibraryBottom_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs _)
  {
    if (!DeckTestingCardDrag.IsDragging || DeckTestingCardDrag.Item is not DeckTestingMTGCard item)
      return;

    // Tokens will not be added to the list, but they will be removed from the battlefield
    if (!item.IsToken && sender is FrameworkElement element)
      (element.DataContext as ObservableCollection<DeckTestingMTGCard>)?.Add(item);

    DeckTestingCardDrag.Complete();
  }

  public void BattlefieldCanvas_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs _)
  {
    if (DeckTestingCardDrag.IsDragging && sender is FrameworkElement element)
      DragCardPreview.Change(this, new(element.XamlRoot) { Opacity = DragCardPreview.BattlefieldOpacity });
  }

  public void BattlefieldCanvas_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (!DeckTestingCardDrag.IsDragging)
      return;

    if (sender is Canvas canvas && DeckTestingCardDrag.Item is DeckTestingMTGCard item)
    {
      var pos = e.GetCurrentPoint(canvas).Position;

      if (canvas.Children.FirstOrDefault(x => (x as DeckTestingBattlefieldCardView)?.Model == item) is UIElement existingElement)
      {
        // Card is already on the canvas, so move it to the dragged position
        // TODO: z-index (check if card are under the pointer)
        Canvas.SetLeft(existingElement, pos.X + DragCardPreview.CurrentOffset.X);
        Canvas.SetTop(existingElement, pos.Y + DragCardPreview.CurrentOffset.Y);
        DeckTestingCardDrag.Cancel();
      }
      else
      {
        // Add card to the canvas
        var cardElement = new DeckTestingBattlefieldCardView()
        {
          Model = new(item.Info),
          Height = DragCardPreview.ImageY,
          Width = DragCardPreview.ImageX,
        };

        if (item.IsToken)
          cardElement.CountCounterVisibility = Visibility.Visible;

        Canvas.SetLeft(cardElement, pos.X + DragCardPreview.CurrentOffset.X);
        Canvas.SetTop(cardElement, pos.Y + DragCardPreview.CurrentOffset.Y);

        canvas.Children.Add(cardElement);

        DeckTestingCardDrag.Complete();
      }
    }

    DeckTestingCardDrag.Cancel();
  }

  // Drop & Reorder from: https://github.com/microsoft/WinUI-Gallery/blob/main/WinUIGallery/ControlPages/ListViewPage.xaml.cs
  private static void DropAndReorder(DeckTestingMTGCard item, ListViewBase target, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    // Find the insertion index:
    var pos = e.GetCurrentPoint(target.ItemsPanelRoot).Position;

    // If the target ListView has items in it, use the height of the first item
    //      to find the insertion index.
    var insertIndex = 0;
    if (target.Items.Count != 0)
    {
      // Get a reference to the first item in the ListView
      var sampleItem = (ListViewItem)target.ContainerFromIndex(0);

      if (target.ItemsPanelRoot is ItemsStackPanel panel)
      {
        if (panel.Orientation == Orientation.Horizontal)
        {
          // Adjust itemWidth for margins
          var itemWidth = sampleItem.ActualWidth + sampleItem.Margin.Right + sampleItem.Margin.Left;

          // Find index based on dividing number of items by width of each item
          insertIndex = Math.Min(target.Items.Count - 1, (int)(pos.X / itemWidth));

          // Find the item being dropped on top of.
          var targetItem = (ListViewItem)target.ContainerFromIndex(insertIndex);

          // If the drop position is more than half-way down the item being dropped on
          //      top of, increment the insertion index so the dropped item is inserted
          //      right instead of left of the item being dropped on top of.
          var positionInItem = e.GetCurrentPoint(targetItem).Position;
          if (positionInItem.X > itemWidth / 2)
          {
            insertIndex++;
          }

          // Don't go out of bounds
          insertIndex = Math.Min(target.Items.Count, insertIndex);
        }
        else
        {
          // Adjust itemHeight for margins
          var itemHeight = sampleItem.ActualHeight + sampleItem.Margin.Top + sampleItem.Margin.Bottom;

          // Find index based on dividing number of items by height of each item
          insertIndex = Math.Min(target.Items.Count - 1, (int)(pos.Y / itemHeight));

          // Find the item being dropped on top of.
          var targetItem = (ListViewItem)target.ContainerFromIndex(insertIndex);

          // If the drop position is more than half-way down the item being dropped on
          //      top of, increment the insertion index so the dropped item is inserted
          //      below instead of above the item being dropped on top of.
          var positionInItem = e.GetCurrentPoint(targetItem).Position;
          if (positionInItem.Y > itemHeight / 2)
          {
            insertIndex++;
          }

          // Don't go out of bounds
          insertIndex = Math.Min(target.Items.Count, insertIndex);
        }
      }
    }
    // Only other case is if the target ListView has no items (the dropped item will be
    //      the first). In that case, the insertion index will remain zero.

    // Insert the item into the target's ItemsSource
    if (target.ItemsSource is ObservableCollection<DeckTestingMTGCard> targetSource)
    {
      var oldIndex = targetSource.IndexOf(item);

      if (oldIndex == -1 || oldIndex != insertIndex && oldIndex + 1 != insertIndex)
      {
        if (oldIndex != -1 && oldIndex < insertIndex)
          insertIndex--; // Removing the old item will move other items back

        DeckTestingCardDrag.Complete(); // Removes old item from the original collection (might be the same as targetSource)

        // using targetSource.Move() will make the list UI flicker, so the item needs to be removed and inserted
        targetSource.Insert(Math.Clamp(insertIndex, 0, targetSource.Count), item);
      }
    }

    DeckTestingCardDrag.Cancel();
  }
}
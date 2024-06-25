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
  public void Droppable_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (DeckTestingCardDrag.IsDragging)
      DragCardPreview.Change(this, new((sender as FrameworkElement).XamlRoot) { Opacity = DragCardPreview.DroppableOpacity });
  }

  public void Droppable_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (DeckTestingCardDrag.IsDragging)
      DragCardPreview.Change(this, new((sender as FrameworkElement).XamlRoot) { Opacity = DragCardPreview.UndroppableOpacity });
  }

  public void DroppableListView_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (!DeckTestingCardDrag.IsDragging) return;

    var item = DeckTestingCardDrag.Item;

    // Tokens will not be added to the list, but they will be removed from the battlefield
    if (item.IsToken)
      DeckTestingCardDrag.Complete();
    else
      DropAndReorder(item: item, target: sender as ListViewBase, e: e);
  }

  public void LibraryTop_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (!DeckTestingCardDrag.IsDragging) return;

    var item = DeckTestingCardDrag.Item;

    // Tokens will not be added to the list, but they will be removed from the battlefield
    if (!item.IsToken)
      ((sender as FrameworkElement).DataContext as ObservableCollection<DeckTestingMTGCard>)
        ?.Insert(0, item);

    DeckTestingCardDrag.Complete();
  }

  public void LibraryBottom_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (!DeckTestingCardDrag.IsDragging) return;

    var item = DeckTestingCardDrag.Item;

    // Tokens will not be added to the list, but they will be removed from the battlefield
    if (!item.IsToken)
      ((sender as FrameworkElement).DataContext as ObservableCollection<DeckTestingMTGCard>)
        ?.Add(item);

    DeckTestingCardDrag.Complete();
  }

  public void BattlefieldCanvas_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (DeckTestingCardDrag.IsDragging)
      DragCardPreview.Change(this, new((sender as FrameworkElement).XamlRoot) { Opacity = DragCardPreview.BattlefieldOpacity });
  }

  public void BattlefieldCanvas_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (!DeckTestingCardDrag.IsDragging) return;

    if (sender is Canvas canvas)
    {
      var pos = e.GetCurrentPoint(canvas).Position;
      var item = DeckTestingCardDrag.Item;

      if (canvas.Children.FirstOrDefault(x => ((FrameworkElement)x).DataContext == item) is UIElement existingElement)
      {
        // Card is already on the canvas, so move it to the dragged position
        // TODO: z-index (check if card are under the pointer)
        //Canvas.SetLeft(existingElement, pos.X + DeckTestingCardDrag.DragOffset.X);
        //Canvas.SetTop(existingElement, pos.Y + DeckTestingCardDrag.DragOffset.Y);
        Canvas.SetLeft(existingElement, pos.X);
        Canvas.SetTop(existingElement, pos.Y);
        DeckTestingCardDrag.Cancel();
      }
      else
      {
        // Add card to the canvas
        var cardImg = new DeckTestingCardImageView()
        {
          Model = item,
          Height = DragCardPreview.ImageY,
          Width = DragCardPreview.ImageX,
          //DataContext = new DeckTestingMTGCardViewModel(new(card.Model.Info))
          //{
          //  SelectedFaceSide = card.SelectedFaceSide,
          //  IsToken = card.IsToken,
          //},
          //CardHeight = BattlefieldCardSize.Y,
          //CardWidth = BattlefieldCardSize.X,
        };

        //cardImg.PointerEntered += BattlefieldCard_PointerEntered;
        //cardImg.PointerPressed += BattlefieldCard_PointerPressed;

        // TODO: Item repeater for canvas items?
        // CanvasView: https://dev.azure.com/dotnet/CommunityToolkit/_artifacts/feed/CommunityToolkit-Labs/NuGet/CommunityToolkit.Labs.WinUI.CanvasView/overview/0.1.230830
        // Events needs to be unsubscribed on unload so the Page could be destroyerd on GC
        //canvas.Unloaded += (s, e) =>
        //{
        //  if (cardImg != null)
        //  {
        //    cardImg.PointerEntered -= BattlefieldCard_PointerEntered;
        //    cardImg.PointerPressed -= BattlefieldCard_PointerPressed;
        //  }
        //};

        //Canvas.SetLeft(cardImg, pos.X + DragArgs.DragOffset.X);
        //Canvas.SetTop(cardImg, pos.Y + DragArgs.DragOffset.Y);

        Canvas.SetLeft(cardImg, pos.X);
        Canvas.SetTop(cardImg, pos.Y);

        canvas.Children.Add(cardImg);

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
    var targetSource = target.ItemsSource as ObservableCollection<DeckTestingMTGCard>;
    var oldIndex = targetSource.IndexOf(item);

    if (oldIndex == -1 || oldIndex != insertIndex && oldIndex + 1 != insertIndex)
    {
      if (oldIndex != -1 && oldIndex < insertIndex)
        insertIndex--; // Removing the old item will move other items back

      DeckTestingCardDrag.Complete(); // Removes old item from the original collection (might be the same as targetSource)

      // using targetSource.Move() will make the list UI flicker, so the item needs to be removed and inserted
      targetSource.Insert(Math.Clamp(insertIndex, 0, targetSource.Count), item);
    }
    DeckTestingCardDrag.Cancel();
  }
}

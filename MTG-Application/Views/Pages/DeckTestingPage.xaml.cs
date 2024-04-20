using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using MTGApplication.General.Extensions;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using MTGApplication.Views.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using static MTGApplication.Views.Controls.MTGCardPreviewControl;

namespace MTGApplication.Views.Pages;

/// <summary>
/// Page that can be used to playtest selected MTG deck
/// </summary>
[ObservableObject]
public sealed partial class DeckTestingPage : Page
{
  public DeckTestingPage(MTGCardDeck cardDeck, DeckTestingMTGCardViewModel[] tokens = null)
  {
    InitializeComponent();
    PointerMoved += Root_PointerMoved;
    PointerReleased += Root_PointerReleased;

    MTGDeckTestingViewModel = new MTGDeckTestingViewModel(cardDeck, tokens);

    Loaded += DeckTestingPage_Loaded;
    Unloaded += DeckTestingPage_Unloaded;
  }

  private CustomDragArgs<DeckTestingMTGCardViewModel> dragArgs;

  #region Properties
  [ObservableProperty] private Visibility libraryVisibility = Visibility.Collapsed;
  [ObservableProperty] private CardPreviewProperties cardPreviewProperties = new();

  public MTGDeckTestingViewModel MTGDeckTestingViewModel { get; }
  public Vector2 BattlefieldCardSize { get; } = new(215, 300);
  public Vector2 CardPreviewSize { get; } = new(251, 350);
  private CustomDragArgs<DeckTestingMTGCardViewModel> DragArgs
  {
    get => dragArgs;
    set
    {
      dragArgs = value;

      if (dragArgs != null)
      {
        dragArgs.Completed += DragArgs_DragEnded;
        dragArgs.Canceled += DragArgs_DragEnded;
      }
    }
  }
  #endregion

  #region Event Methods
  private void DeckTestingPage_Loaded(object sender, RoutedEventArgs e)
  {
    MTGDeckTestingViewModel.NewGameStarted += MTGDeckTestingViewModel_NewGameStarted;
    MTGDeckTestingViewModel.NewTurnStarted += MTGDeckTestingViewModel_NewTurnStarted;
    MTGDeckTestingViewModel.NewGame(); // Start new game when page loads
  }

  private void DeckTestingPage_Unloaded(object sender, RoutedEventArgs e)
  {
    MTGDeckTestingViewModel.NewGameStarted -= MTGDeckTestingViewModel_NewGameStarted;
    MTGDeckTestingViewModel.NewTurnStarted -= MTGDeckTestingViewModel_NewTurnStarted;
  }

  private void MTGDeckTestingViewModel_NewTurnStarted()
  {
    foreach (var child in BattlefieldCanvas.Children)
    {
      if ((child as FrameworkElement).DataContext is DeckTestingMTGCardViewModel cardVM)
      {
        cardVM.IsTapped = false;
      }
    }
  }

  private void MTGDeckTestingViewModel_NewGameStarted() => BattlefieldCanvas.Children.Clear();

  private void DragArgs_DragEnded(DeckTestingMTGCardViewModel obj)
  {
    DragPreviewImage.Visibility = Visibility.Collapsed;
    DragArgs = null;
  }
  #endregion

  [RelayCommand]
  public void LibraryVisibilitySwitch() => LibraryVisibility = LibraryVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

  #region Pointer Events
  // ---------------- Item Pointer Movement -------------------

  private void CardListViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
  {
    // Change card preview image to hovered item
    if (sender is FrameworkElement { DataContext: DeckTestingMTGCardViewModel cardVM })
    {
      CardPreviewProperties = new()
      {
        CardViewModel = cardVM,
        XMirror = true,
        Offset = new(175, 100)
      };
    }
  }

  private void CardListViewItem_PointerMoved(object sender, PointerRoutedEventArgs e)
    // Move card preview image to mouse position when hovering over on list view item.
    => CardPreviewProperties.Coordinates = e.GetCurrentPoint(null).Position.ToVector2();

  private void CardListViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
    // Change placeholder image to the old hovered card's image so the placeholder won't flicker
    => CardPreviewProperties.CardViewModel = null;

  private void HandCard_PointerEntered(object sender, PointerRoutedEventArgs e)
  {
    // Change card preview image to hovered item
    if (sender is FrameworkElement { DataContext: DeckTestingMTGCardViewModel cardVM })
    {
      CardPreviewProperties = new()
      {
        CardViewModel = cardVM,
        Coordinates = ((sender as FrameworkElement).TransformToVisual(null).TransformPoint(new()).ToVector2()) + new Vector2(CardPreviewSize.X / 4),
      };
    }
  }

  private void HandCard_PointerMoved(object sender, PointerRoutedEventArgs e)
  {
    if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
    {
      // Dont show preview image if the user is dragging a card
      CardPreviewProperties.CardViewModel = null;
    }
  }

  private void HandCard_PointerExited(object sender, PointerRoutedEventArgs e)
    // Change placeholder image to the old hovered card's image so the placeholder won't flicker
    => CardPreviewProperties.CardViewModel = null;

  // ------------------ Item Drag & Drop ----------------------

  private void Root_PointerMoved(object sender, PointerRoutedEventArgs e)
  {
    if (DragArgs != null)
    {
      if (e.GetCurrentPoint(null).Properties.IsRightButtonPressed || !e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
      {
        DragArgs?.Cancel();
      }
      else
      {
        var pointerPosition = e.GetCurrentPoint(null).Position;
        DragPreviewImage.SetValue(Canvas.LeftProperty, pointerPosition.X + DragArgs.DragOffset.X);
        DragPreviewImage.SetValue(Canvas.TopProperty, pointerPosition.Y + DragArgs.DragOffset.Y);
        DragPreviewImage.Visibility = Visibility.Visible;
        DragArgs.Start();
      }
    }
  }

  // This event does not invoke when the pointer releases on the same element that started the dragging,
  // unless the pointer has moved outside the element before releasing.
  // If this event does not invoke, the draggable element will invoke the PointerReleased event and end the drag.
  private void Root_PointerReleased(object sender, PointerRoutedEventArgs e) => DragArgs?.Cancel();

  private void Droppable_PointerEntered(object sender, PointerRoutedEventArgs e) => DragPreviewImage.Opacity = CustomDragArgs<DeckTestingMTGCardViewModel>.DroppableOpacity;

  private void Droppable_PointerExited(object sender, PointerRoutedEventArgs e) => DragPreviewImage.Opacity = CustomDragArgs<DeckTestingMTGCardViewModel>.UndroppableOpacity;

  private void DraggableListItem_PointerPressed(object sender, PointerRoutedEventArgs e)
  {
    if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
    {
      DragArgs = new((sender as FrameworkElement).DataContext as DeckTestingMTGCardViewModel, new(-BattlefieldCardSize.X / 2, -BattlefieldCardSize.Y / 2));

      DragArgs.Completed += (item) =>
      {
        ((sender as FrameworkElement).FindParentByType<ListViewBase>().ItemsSource as ObservableCollection<DeckTestingMTGCardViewModel>).Remove(item);
      };

      var pointerPosition = e.GetCurrentPoint(null).Position;
      DragPreviewImage.Source = DragArgs?.Item.SelectedFaceUri;
      DragPreviewImage.SetValue(Canvas.LeftProperty, pointerPosition.X + DragArgs.DragOffset.X);
      DragPreviewImage.SetValue(Canvas.TopProperty, pointerPosition.Y + DragArgs.DragOffset.Y);

      (DragPreviewImage.RenderTransform as RotateTransform).Angle = 0;
    }
  }

  private void TokenListView_PointerPressed(object sender, PointerRoutedEventArgs e)
  {
    if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
    {
      DragArgs = new((sender as FrameworkElement).DataContext as DeckTestingMTGCardViewModel, new(-BattlefieldCardSize.X / 2, -BattlefieldCardSize.Y / 2));

      var pointerPosition = e.GetCurrentPoint(null).Position;
      DragPreviewImage.Source = DragArgs?.Item.SelectedFaceUri;
      DragPreviewImage.SetValue(Canvas.LeftProperty, pointerPosition.X + DragArgs.DragOffset.X);
      DragPreviewImage.SetValue(Canvas.TopProperty, pointerPosition.Y + DragArgs.DragOffset.Y);

      (DragPreviewImage.RenderTransform as RotateTransform).Angle = 0;
    }
  }

  private void DroppableList_PointerReleased(object sender, PointerRoutedEventArgs e)
  {
    if (DragArgs?.Item is var item && item != null)
    {
      // Tokens will not be added to the list, but they will be removed from the original place
      if (item.IsToken) { DragArgs.Complete(); return; }

      var target = sender as ListViewBase;

      if (target != null)
      {
        // Drop & Reorder from: https://github.com/microsoft/WinUI-Gallery/blob/main/WinUIGallery/ControlPages/ListViewPage.xaml.cs

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
        var targetSource = target.ItemsSource as ObservableCollection<DeckTestingMTGCardViewModel>;
        var oldIndex = targetSource.IndexOf(item);

        if (oldIndex == -1 || (oldIndex != insertIndex && oldIndex + 1 != insertIndex))
        {
          if (oldIndex != -1 && oldIndex < insertIndex)
          {
            insertIndex--; // Removing the old item will move other items back
          }

          DragArgs.Complete(); // Removes old item from the original collection (might be the same as targetSource)

          // using targetSource.Move() will make the list UI flicker, so the item needs to be removed and inserted
          targetSource.Insert(Math.Clamp(insertIndex, 0, targetSource.Count), item);
        }
      }
    }

    DragArgs?.Cancel();
  }

  private void BattlefieldCanvas_PointerEntered(object sender, PointerRoutedEventArgs e) => DragPreviewImage.Opacity = 1;

  private void BattlefieldCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
  {
    if (DragArgs?.Item != null)
    {
      if (sender as Canvas is var canvas && canvas != null)
      {
        var pos = e.GetCurrentPoint(canvas).Position;
        var card = DragArgs.Item;

        if (canvas.Children.FirstOrDefault(x => ((FrameworkElement)x).DataContext == card) is var existingElement && existingElement != null)
        {
          // Card is already on the canvas, so move it to the dragged position
          // TODO: z-index (check if card are under the pointer)
          Canvas.SetLeft(existingElement, pos.X + DragArgs.DragOffset.X);
          Canvas.SetTop(existingElement, pos.Y + DragArgs.DragOffset.Y);
          DragArgs?.Cancel();
        }
        else
        {
          // Add card to the canvas
          var cardImg = new DeckTestingBattlefieldCardControl()
          {
            DataContext = new DeckTestingMTGCardViewModel(new(card.Model.Info))
            {
              SelectedFaceSide = card.SelectedFaceSide,
              IsToken = card.IsToken,
            },
            CardHeight = BattlefieldCardSize.Y,
            CardWidth = BattlefieldCardSize.X,
          };

          cardImg.PointerEntered += BattlefieldCard_PointerEntered;
          cardImg.PointerPressed += BattlefieldCard_PointerPressed;

          // TODO: Item repeater for canvas items?
          // CanvasView: https://dev.azure.com/dotnet/CommunityToolkit/_artifacts/feed/CommunityToolkit-Labs/NuGet/CommunityToolkit.Labs.WinUI.CanvasView/overview/0.1.230830
          // Events needs to be unsubscribed on unload so the Page could be destroyerd on GC
          canvas.Unloaded += (s, e) =>
          {
            if (cardImg != null)
            {
              cardImg.PointerEntered -= BattlefieldCard_PointerEntered;
              cardImg.PointerPressed -= BattlefieldCard_PointerPressed;
            }
          };

          Canvas.SetLeft(cardImg, pos.X + DragArgs.DragOffset.X);
          Canvas.SetTop(cardImg, pos.Y + DragArgs.DragOffset.Y);

          canvas.Children.Add(cardImg);

          DragArgs?.Complete();
        }
      }
    }

    DragArgs?.Cancel();
  }

  private void BattlefieldCard_PointerEntered(object sender, PointerRoutedEventArgs e)
  {
    if (DragArgs == null)
    {
      // Settings the preview image's source before dragging reduces flickering when staring the drag
      DragPreviewImage.Source = ((sender as FrameworkElement).DataContext as DeckTestingMTGCardViewModel).SelectedFaceUri;
    }
  }

  private void BattlefieldCard_PointerPressed(object sender, PointerRoutedEventArgs e)
  {
    // Drag and drop
    var img = sender as FrameworkElement;
    var canvas = BattlefieldCanvas;
    var pointerPoint = e.GetCurrentPoint(img);

    if (pointerPoint.Properties.IsLeftButtonPressed)
    {
      var imgPos = new Point(Canvas.GetLeft(img), Canvas.GetTop(img));
      var pointetPos = e.GetCurrentPoint(canvas).Position;
      var offset = new Point()
      {
        X = imgPos.X - pointetPos.X,
        Y = imgPos.Y - pointetPos.Y,
      };

      DragArgs = new((sender as FrameworkElement).DataContext as DeckTestingMTGCardViewModel, offset);

      DragArgs.Started += (item) =>
      {
        img.Opacity = 0;
      };

      DragArgs.Completed += (item) =>
      {
        canvas.Children.Remove(canvas.Children.FirstOrDefault(x => ((FrameworkElement)x).DataContext as DeckTestingMTGCardViewModel == item));
      };

      DragArgs.Canceled += (item) =>
      {
        img.Opacity = 1;
      };

      var pointerWindowPosition = e.GetCurrentPoint(null).Position;
      DragPreviewImage.Source = DragArgs.Item.SelectedFaceUri;

      DragPreviewImage.SetValue(Canvas.LeftProperty, pointerWindowPosition.X + DragArgs.DragOffset.X);
      DragPreviewImage.SetValue(Canvas.TopProperty, pointerWindowPosition.Y + DragArgs.DragOffset.Y);

      // Preview card rotation
      (DragPreviewImage.RenderTransform as RotateTransform).Angle = (img.DataContext as DeckTestingMTGCardViewModel).IsTapped ? 90 : 0;
    }
  }

  private void LibraryTop_PointerReleased(object sender, PointerRoutedEventArgs e)
  {
    if (DragArgs?.Item != null)
    {
      // Tokens will not be added to the list, but they will be removed from the original place
      if (DragArgs.Item.IsToken) { DragArgs.Complete(); return; }

      MTGDeckTestingViewModel.LibraryAddTop(DragArgs.Item);
      DragArgs.Complete();
    }
  }

  private void LibraryBottom_PointerReleased(object sender, PointerRoutedEventArgs e)
  {
    if (DragArgs?.Item != null)
    {
      // Tokens will not be added to the list, but they will be removed from the original place
      if (DragArgs.Item.IsToken) { DragArgs.Complete(); return; }

      MTGDeckTestingViewModel.LibraryAddBottom(DragArgs.Item);
      DragArgs.Complete();
    }
  }
  #endregion
}

// Drag args
public sealed partial class DeckTestingPage
{
  public class CustomDragArgs<T>
  {
    public CustomDragArgs(T item, Point dragOffset)
    {
      Item = item;
      DragOffset = dragOffset;
    }

    public T Item { get; }
    public Point DragOffset { get; }
    public event Action<T> Started;
    public event Action<T> Completed;
    public event Action<T> Canceled;
    public bool IsDragging { get; private set; } = false;
    public bool IsCompleted { get; private set; } = false;

    public static float UndroppableOpacity { get; } = .3f;
    public static float DroppableOpacity { get; } = .8f;

    public void Start()
    {
      IsDragging = true;
      Started?.Invoke(Item);
    }

    public void Complete()
    {
      IsCompleted = true;
      IsDragging = false;
      Completed?.Invoke(Item);
    }

    public void Cancel()
    {
      if (!IsCompleted || IsDragging) Canceled?.Invoke(Item);
      IsDragging = false;
    }
  }
}
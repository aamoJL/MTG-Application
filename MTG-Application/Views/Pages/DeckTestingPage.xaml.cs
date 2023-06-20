using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using MTGApplication.Extensions;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using Windows.Foundation;

namespace MTGApplication.Views.Pages;

[ObservableObject]
public sealed partial class DeckTestingPage : Page
{
  public DeckTestingPage()
  {
    InitializeComponent();
    PointerMoved += Root_PointerMoved;
    PointerReleased += Root_PointerReleased;

    MTGDeckTestingViewModel.NewGameStarted += MTGDeckTestingViewModel_NewGameStarted;

    LibraryVisibilitySwitchCommand = new RelayCommand(SwitchLibraryVisibility);
  }

  private void MTGDeckTestingViewModel_NewGameStarted() => BattlefieldCanvas.Children.Clear();

  public MTGCard[] DeckCards
  {
    get => (MTGCard[])GetValue(DeckCardsProperty);
    set
    {
      SetValue(DeckCardsProperty, value);
      MTGDeckTestingViewModel.DeckCards = value;
    }
  }
  public Vector2 BattlefieldCardDimensions { get; } = new(179 * 1.2f, 250 * 1.2f);
  [ObservableProperty]
  public Visibility libraryVisibility = Visibility.Collapsed;

  public ICommand LibraryVisibilitySwitchCommand;

  public static readonly DependencyProperty DeckCardsProperty =
      DependencyProperty.Register(nameof(DeckCards), typeof(MTGCard[]), typeof(DeckTestingPage), new PropertyMetadata(Array.Empty<MTGCard>()));

  public MTGDeckTestingViewModel MTGDeckTestingViewModel { get; } = new();

  public void SwitchLibraryVisibility() => LibraryVisibility = LibraryVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

  #region Pointer Events

  public class CustomDragArgs<T>
  {
    public CustomDragArgs(T item, Point dragOffset)
    {
      Item = item;
      DragOffset = dragOffset;
    }

    public T Item { get; }
    public Point DragOffset { get; }
    public event Action<T> Completed;
    public event Action<T> Canceled;
    public bool IsCompleted { get; private set; } = false;

    public static float UndroppableOpacity { get; } = .3f;
    public static float DroppableOpacity { get; } = .8f;

    public void Complete()
    {
      IsCompleted = true;
      Completed?.Invoke(Item);
    }

    public void Cancel()
    {
      if (!IsCompleted) Canceled?.Invoke(Item);
    }
  }

  private CustomDragArgs<MTGCardViewModel> cardDrag;
  private CustomDragArgs<MTGCardViewModel> CardDrag => cardDrag;

  private void SetCardDragArgs(MTGCardViewModel item, Point? offset)
  {
    offset ??= new(BattlefieldCardDimensions.X / 2, BattlefieldCardDimensions.Y / 2);
    cardDrag = new(item, (Point)offset);
    DragPreviewImage.Visibility = Visibility.Visible;

    CardDrag.Completed += (item) =>
    {
      DragPreviewImage.Visibility = Visibility.Collapsed;
      cardDrag = null;
    };
    CardDrag.Canceled += (item) =>
    {
      DragPreviewImage.Visibility = Visibility.Collapsed;
      cardDrag = null;
    };
  }

  // ---------------- Item Pointer Movement -------------------

  private void CardListViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
  {
    // Change card preview image to hovered item
    if (sender is FrameworkElement { DataContext: MTGCardViewModel cardVM })
    {
      PreviewImage.Visibility = Visibility.Visible;
      PreviewImage.Source = new BitmapImage(new(cardVM.SelectedFaceUri));
    }
  }

  private void CardListViewItem_PointerMoved(object sender, PointerRoutedEventArgs e)
  {
    // Move card preview image to mouse position when hovering over on list view item.
    // The position is clamped to element size
    var windowBounds = ActualSize;
    var pointerPosition = e.GetCurrentPoint(null).Position;

    pointerPosition.X -= ActualOffset.X; // Apply element offset
    pointerPosition.Y -= ActualOffset.Y;

    var xOffsetFromPointer = (windowBounds.X - pointerPosition.X) > PreviewImage.ActualWidth ? 50 : -50 - PreviewImage.ActualWidth;
    var yOffsetFromPointer = -100;

    PreviewImage.SetValue(Canvas.LeftProperty, Math.Max(Math.Clamp(pointerPosition.X + xOffsetFromPointer, 0, Math.Max(ActualSize.X - PreviewImage.ActualWidth, 0)), 0));
    PreviewImage.SetValue(Canvas.TopProperty, Math.Max(Math.Clamp(pointerPosition.Y + yOffsetFromPointer, 0, Math.Max(ActualSize.Y - PreviewImage.ActualHeight, 0)), 0));
  }

  private void CardListViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
  {
    PreviewImage.Visibility = Visibility.Collapsed;
    // Change placeholder image to the old hovered card's image so the placeholder won't flicker
    PreviewImage.PlaceholderSource = PreviewImage.Source as ImageSource;
  }

  private void HandCard_PointerMoved(object sender, PointerRoutedEventArgs e)
  {
    var pointerPoint = e.GetCurrentPoint(null);
    if (!pointerPoint.Properties.IsLeftButtonPressed)
    {
      PreviewImage.Visibility = Visibility.Visible;
      // Move card preview image to mouse position when hovering over on hand card.
      // The position is clamped to element size
      var element = (sender as FrameworkElement);
      var elementPosition = element.TransformToVisual(null).TransformPoint(new());

      var xOffsetFromPointer = -(PreviewImage.ActualWidth / 2) + (element.ActualWidth / 2);
      var yOffsetFromPointer = 0;

      PreviewImage.SetValue(Canvas.LeftProperty, Math.Max(Math.Clamp(elementPosition.X + xOffsetFromPointer, 0, Math.Max(ActualSize.X - PreviewImage.ActualWidth, 0)), 0));
      PreviewImage.SetValue(Canvas.TopProperty, Math.Max(Math.Clamp(elementPosition.Y + yOffsetFromPointer, 0, Math.Max(ActualSize.Y - PreviewImage.ActualHeight, 0)), 0));
    }
    else
    {
      // Dont show preview image if the user is dragging a card
      PreviewImage.Visibility = Visibility.Collapsed;
    }
  }

  private void HandCard_PointerExited(object sender, PointerRoutedEventArgs e)
  {
    PreviewImage.Visibility = Visibility.Collapsed;
    // Change placeholder image to the old hovered card's image so the placeholder won't flicker
    PreviewImage.PlaceholderSource = null;
  }

  // ------------------ Item Drag & Drop ----------------------

  private void Root_PointerMoved(object sender, PointerRoutedEventArgs e)
  {
    if (CardDrag != null)
    {
      if (e.GetCurrentPoint(null).Properties.IsRightButtonPressed || !e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
      {
        CardDrag?.Cancel();
      }
      else
      {
        var pointerPosition = e.GetCurrentPoint(null).Position;
        DragPreviewImage.SetValue(Canvas.LeftProperty, pointerPosition.X + CardDrag.DragOffset.X);
        DragPreviewImage.SetValue(Canvas.TopProperty, pointerPosition.Y + CardDrag.DragOffset.Y);
      }
    }
  }

  // This event does not invoke when the pointer releases on the same element that started the dragging,
  // unless the pointer has moved outside the element before releasing.
  // If this event does not invoke, the draggable element will invoke the PointerReleased event and end the drag.
  private void Root_PointerReleased(object sender, PointerRoutedEventArgs e) => CardDrag?.Cancel();

  private void Droppable_PointerEntered(object sender, PointerRoutedEventArgs e) => DragPreviewImage.Opacity = CustomDragArgs<MTGCardViewModel>.DroppableOpacity;

  private void Droppable_PointerExited(object sender, PointerRoutedEventArgs e) => DragPreviewImage.Opacity = CustomDragArgs<MTGCardViewModel>.UndroppableOpacity;

  private void DraggableListItem_PointerPressed(object sender, PointerRoutedEventArgs e)
  {
    if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
    {
      SetCardDragArgs((sender as FrameworkElement).DataContext as MTGCardViewModel, new(-BattlefieldCardDimensions.X / 2, -BattlefieldCardDimensions.Y / 2));

      CardDrag.Completed += (item) =>
      {
        ((sender as FrameworkElement).FindParentByType<ListViewBase>().ItemsSource as ObservableCollection<MTGCardViewModel>).Remove(item);
      };

      var pointerPosition = e.GetCurrentPoint(null).Position;
      DragPreviewImage.Source = CardDrag?.Item.SelectedFaceUri;
      DragPreviewImage.SetValue(Canvas.LeftProperty, pointerPosition.X + CardDrag.DragOffset.X);
      DragPreviewImage.SetValue(Canvas.TopProperty, pointerPosition.Y + CardDrag.DragOffset.Y);

      (DragPreviewImage.RenderTransform as RotateTransform).Angle = 0;
    }
  }

  private void DroppableList_PointerReleased(object sender, PointerRoutedEventArgs e)
  {
    if (CardDrag?.Item is var item && item != null)
    {
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
        var targetSource = target.ItemsSource as ObservableCollection<MTGCardViewModel>;
        var oldIndex = targetSource.IndexOf(item);

        if (oldIndex == -1 || (oldIndex != insertIndex && oldIndex + 1 != insertIndex))
        {
          if (oldIndex != -1 && oldIndex < insertIndex)
          {
            insertIndex--; // Removing the old item will move other items back
          }

          CardDrag.Complete(); // Removes old item from the original collection (might be the same as targetSource)

          // using targetSource.Move() will make the list UI flicker, so the item needs to be removed and inserted
          targetSource.Insert(Math.Clamp(insertIndex, 0, targetSource.Count), item);
        }
      }
    }

    CardDrag?.Cancel();
  }

  private void BattlefieldCanvas_PointerEntered(object sender, PointerRoutedEventArgs e) => DragPreviewImage.Opacity = 1;

  private void BattlefieldCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
  {
    if (CardDrag?.Item != null)
    {
      if (sender as Canvas is var canvas && canvas != null)
      {
        var pos = e.GetCurrentPoint(canvas).Position;
        var card = CardDrag.Item;

        if (canvas.Children.FirstOrDefault(x => ((FrameworkElement)x).DataContext == card) is var existingElement && existingElement != null)
        {
          // Card is already on the canvas, so move it the dragged position
          // TODO: z-index (check if card are under the pointer)
          Canvas.SetLeft(existingElement, pos.X + CardDrag.DragOffset.X);
          Canvas.SetTop(existingElement, pos.Y + CardDrag.DragOffset.Y);
          CardDrag?.Cancel();
        }
        else
        {
          // Add card to the canvas
          Application.Current.Resources.TryGetValue("PreviewImagePlaceholderStyle", out var style);

          // TODO: flyouts
          var canvasImg = new ImageEx()
          {
            DataContext = card,
            Height = BattlefieldCardDimensions.Y,
            Width = BattlefieldCardDimensions.X,
            CornerRadius = new(12),
            Style = (Style)style,
            RenderTransformOrigin = new Point(0.5, 0.5),
            RenderTransform = new RotateTransform(),
            Source = card.SelectedFaceUri,
          };

          //// Settings the binding to the canvasImg did not update the image, why?
          //canvasImg.SetBinding(ImageEx.SourceProperty, new Binding()
          //{
          //  Mode = BindingMode.OneWay,
          //  Source = card.SelectedFaceUri,
          //});

          // Instead using property change event to change the image
          card.PropertyChanged += (s,e) =>
          {
            if(e.PropertyName == nameof(MTGCardViewModel.SelectedFaceUri))
            {
              canvasImg.Source = card.SelectedFaceUri;
            }
          };

          SetBattlefieldImage_Flyouts(canvasImg);
          SetBattlefieldImage_PointerEvents(canvasImg, canvas);

          Canvas.SetLeft(canvasImg, pos.X + CardDrag.DragOffset.X);
          Canvas.SetTop(canvasImg, pos.Y + CardDrag.DragOffset.Y);

          canvas.Children.Add(canvasImg);

          CardDrag?.Complete();
        }
      }
    }

    CardDrag?.Cancel();
  }

  private void SetBattlefieldImage_Flyouts(FrameworkElement img)
  {
    var card = img.DataContext as MTGCardViewModel;
    var flyout = new MenuFlyout
    {
      AreOpenCloseAnimationsEnabled = false
    };

    flyout.Items.Add(new MenuFlyoutItem()
    {
      Command = card.FlipCardCommand,
      Icon = new FontIcon()
      {
        FontFamily = new FontFamily("Segoe MDL2 Assets"),
        Glyph = "\xE117"
      },
      Text = "Flip"
    });

    img.ContextFlyout = flyout;
  }

  private void SetBattlefieldImage_PointerEvents(FrameworkElement img, Canvas canvas)
  {
    img.PointerEntered += (sender, e) =>
    {
      if (CardDrag == null)
      {
        // Settings the preview image's source before dragging reduces flickering when staring the drag
        DragPreviewImage.Source = (img.DataContext as MTGCardViewModel).SelectedFaceUri;
      }
    };

    img.PointerPressed += (sender, e) =>
    {
      var pointerPoint = e.GetCurrentPoint(sender as FrameworkElement);

      if (pointerPoint.Properties.IsLeftButtonPressed)
      {
        var imgPos = new Point(Canvas.GetLeft(img), Canvas.GetTop(img));
        var pointetPos = e.GetCurrentPoint(canvas).Position;
        var offset = new Point()
        {
          X = imgPos.X - pointetPos.X,
          Y = imgPos.Y - pointetPos.Y,
        };

        SetCardDragArgs((sender as FrameworkElement).DataContext as MTGCardViewModel, offset);

        CardDrag.Completed += (item) =>
        {
          canvas.Children.Remove(canvas.Children.FirstOrDefault(x => ((FrameworkElement)x).DataContext as MTGCardViewModel == item));
        };

        CardDrag.Canceled += (item) =>
        {
          img.Opacity = 1;
        };

        var pointerWindowPosition = e.GetCurrentPoint(null).Position;
        DragPreviewImage.Source = CardDrag.Item.SelectedFaceUri;

        DragPreviewImage.SetValue(Canvas.LeftProperty, pointerWindowPosition.X + CardDrag.DragOffset.X);
        DragPreviewImage.SetValue(Canvas.TopProperty, pointerWindowPosition.Y + CardDrag.DragOffset.Y);

        var previewRenderTransform = (DragPreviewImage.RenderTransform as RotateTransform);
        previewRenderTransform.Angle = (img.RenderTransform as RotateTransform).Angle;

        img.Opacity = 0;
      }
    };

    img.DoubleTapped += (sender, e) =>
    {
      var oldTransform = img.RenderTransform as RotateTransform;
      (img.RenderTransform as RotateTransform).Angle = oldTransform.Angle == 90 ? 0 : 90;

      var previewRenderTransform = (DragPreviewImage.RenderTransform as RotateTransform);
      previewRenderTransform.Angle = (img.RenderTransform as RotateTransform).Angle;
    };
  }

  private void LibraryTop_PointerReleased(object sender, PointerRoutedEventArgs e)
  {
    if (CardDrag?.Item != null)
    {
      MTGDeckTestingViewModel.LibraryAddTop(CardDrag.Item);
      CardDrag.Complete();
    }
  }

  private void LibraryBottom_PointerReleased(object sender, PointerRoutedEventArgs e)
  {
    if (CardDrag?.Item != null)
    {
      MTGDeckTestingViewModel.LibraryAddBottom(CardDrag.Item);
      CardDrag.Complete();
    }
  }

  #endregion
}
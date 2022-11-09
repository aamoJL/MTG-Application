using LiveChartsCore.SkiaSharpView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Pages
{
  /// <summary>
  /// Main Page
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPageViewModel ViewModel = new();

    public MainPage()
    {
      this.InitializeComponent();

      CollectionCmcChart.XAxes = new List<Axis>
      {
        new Axis
        {
          MinStep = 1,
          TextSize = 10
        }
      };
      CollectionCmcChart.YAxes = new List<Axis>
      {
        new Axis
        {
          MinStep = 1,
          TextSize = 10
        }
      };

      Notifications.OnCopied += Notifications_OnCopied;
      Notifications.OnError += Notifications_OnError;
    }

    #region //-----Notifications-----//
    private readonly int notificationDuration = 3000;

    private void Notifications_OnError(object sender, string error)
    {
      PopupAppNotification.Show(error, notificationDuration);
    }
    private void Notifications_OnCopied(object sender, string e)
    {
      PopupAppNotification.Show("Copied", notificationDuration);
    }
    #endregion

    #region //-----Card pointer events-----//
    // -----------List view
    private void CollectionListViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
      // Change card preview image to hovered item
      if ((sender as FrameworkElement)?.DataContext is MTGCardViewModel cardVM)
      {
        PreviewImage.Visibility = Visibility.Visible;
        var imgSource = new BitmapImage(new(cardVM.SelectedFaceUri));
        PreviewImage.Source = imgSource;
      }
    }
    private void CollecitonListViewItem_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
      // Move card preview image to mouse position when hovering over on list view item.
      // The position is clamped to window size
      var windowBounds = ActualSize;
      var pointerPosition = e.GetCurrentPoint(null).Position;

      // Offsets from pointer
      var xOffset = (windowBounds.X - pointerPosition.X) > PreviewImage.ActualWidth ? 50 : -50 - PreviewImage.ActualWidth;
      var yOffset = -100;

      PreviewImage.SetValue(Canvas.LeftProperty, Math.Clamp(pointerPosition.X + xOffset, 0, windowBounds.X - PreviewImage.ActualWidth));
      PreviewImage.SetValue(Canvas.TopProperty, Math.Clamp(pointerPosition.Y + yOffset, 0, windowBounds.Y - PreviewImage.ActualHeight));
    }
    private void CollectionListViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
    {
      PreviewImage.Visibility = Visibility.Collapsed;
      PreviewImage.PlaceholderSource = PreviewImage.Source as ImageSource; // Reduces flickering
    }
    // -----------Grid view
    private void CollectionGridViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
      if ((sender as FrameworkElement)?.DataContext is MTGCardViewModel cardVM)
      {
        cardVM.ControlsVisible = true;
      }
    }
    private void CollectionGridViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
    {
      if ((sender as FrameworkElement)?.DataContext is MTGCardViewModel cardVM)
      {
        cardVM.ControlsVisible = false;
      }
    }
    #endregion

    #region //-----Drag & Drop-----//
    public object draggedElement;

    private void CollectionView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
      if (e.Items[0] is MTGCardViewModel viewModel)
      {
        e.Data.SetText(viewModel.ModelToJSON());
        e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
        draggedElement = sender;
      }
    }
    private void CollectionView_DragOver(object sender, DragEventArgs e)
    {
      if (e.DataView.Contains(StandardDataFormats.Text) && !sender.Equals(draggedElement))
      {
        // Change operation to 'Move' if the shift key is down
        e.AcceptedOperation =
          (e.Modifiers & Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift) == Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift
          ? DataPackageOperation.Move : DataPackageOperation.Copy;
      }
    }
    private async void CollectionView_Drop(object sender, DragEventArgs e)
    {
      if (e.DataView.Contains(StandardDataFormats.Text) && (sender as FrameworkElement).DataContext is MTGCardCollectionViewModel collectionVM)
      {
        DragOperationDeferral def = e.GetDeferral();
        string data = await e.DataView.GetTextAsync();

        try
        {
          var model = JsonSerializer.Deserialize<MTGCardModel>(data);
          if (model.Info.Id == string.Empty || model.Info.Id == null) { throw new Exception(); }
          collectionVM.AddModel(model);
        }
        catch (Exception) { }

        def.Complete();
      }
      else { throw new Exception("DataContext is not MTGCardCollectionViewModel"); }
    }
    private void CollectionView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
      // Remove original item if the operation was 'Move'
      if ((args.DropResult & DataPackageOperation.Move) == DataPackageOperation.Move
        && args.Items[0] is MTGCardViewModel cardVM &&
        sender.DataContext is MTGCardCollectionViewModel collectionVM)
      {
        collectionVM.RemoveViewModel(cardVM);
      }
    }
    #endregion

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
      // Select the search box text so the user doesn't need to click the search box again to write the next search.
      ScryfallSearchBox.Focus(FocusState.Programmatic);
      ScryfallSearchBox.SelectAll();
    }
  }
}

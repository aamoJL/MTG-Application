using LiveChartsCore.SkiaSharpView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;

namespace MTGApplication.Pages
{
  /// <summary>
  /// Main Page
  /// </summary>
  public sealed partial class MainPage : Page
  {
    private int notificationDuration = 3000;

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

      PopupAppNotification.XamlRoot = this.XamlRoot;
      Notifications.OnCopied += Notifications_OnCopied;
      Notifications.OnError += Notifications_OnError;
    }


    #region Notifications
    private void Notifications_OnError(object sender, string error)
    {
      PopupAppNotification.Show(error, notificationDuration);
    }
    private void Notifications_OnCopied(object sender, string e)
    {
      PopupAppNotification.Show("Copied", notificationDuration);
    }
    #endregion

    #region //----------------Card pointer events---------------//
    // -----------List view
    private void CollectionListViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
      // Change card preview image to hovered item
      if((sender as FrameworkElement)?.DataContext is MTGCardViewModel cardVM)
      {
        PreviewImage.Visibility = Visibility.Visible;
        PreviewImage.Source = new BitmapImage(new(cardVM.SelectedFaceUri));
      }
    }
    private void CollecitonListViewItem_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
      // Move card preview image to mouse position when hovering over on list view item.
      // The position is clamped to window size
      var offset = new Point(50, -100);
      var pointerPosition = e.GetCurrentPoint(null).Position;
      var windowBounds = ActualSize;
      PreviewImage.SetValue(Canvas.LeftProperty, Math.Clamp(pointerPosition.X + offset.X, 0, windowBounds.X - PreviewImage.ActualWidth));
      PreviewImage.SetValue(Canvas.TopProperty, Math.Clamp(pointerPosition.Y + offset.Y, 0, windowBounds.Y - PreviewImage.ActualHeight));
    }
    private void CollectionListViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
    {
      PreviewImage.Visibility = Visibility.Collapsed;
    }
    // -----------Grid view
    private void CollectionGridViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
      if((sender as FrameworkElement)?.DataContext is MTGCardViewModel cardVM)
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

    #region //--------------------- Drag & Drop -----------------------------//
    private void CollectionView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
      if (e.Items[0] is MTGCardViewModel viewModel)
      {
        e.Data.SetText(viewModel.ModelToJSON());
        e.Data.RequestedOperation = DataPackageOperation.Copy;
      }
    }
    private void CollectionView_DragOver(object sender, DragEventArgs e)
    {
      if (e.DataView.Contains(StandardDataFormats.Text))
      {
        e.AcceptedOperation = DataPackageOperation.Copy;
      }
    }
    private async void CollectionView_Drop(object sender, DragEventArgs e)
    {
      if (e.DataView.Contains(StandardDataFormats.Text))
      {
        DragOperationDeferral def = e.GetDeferral();
        string data = await e.DataView.GetTextAsync();

        try
        {
          var model = JsonSerializer.Deserialize<MTGCardModel>(data);
          if (model.Info.Id == string.Empty || model.Info.Id == null) { throw new Exception(); }
          ViewModel.CollectionViewModel.AddAndCombineAndSort(model);
        }
        catch (Exception) { }

        def.Complete();
      }
    }
    #endregion

  }
}

using LiveChartsCore.SkiaSharpView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MTGApplication.Pages
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainWindowViewModel ViewModel = new();

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
    }

    // TODO: show increase/decrease only on pointer hover

    #region //----------------Card pointer events---------------//
    private void ScryfallCardName_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
      // Change card preview image to hovered item
      PreviewImage.Visibility = Visibility.Visible;
      FrameworkElement ListElement = sender as FrameworkElement;
      if (ListElement != null)
      {
        MTGCardViewModel cardViewModel = ListElement.DataContext as MTGCardViewModel;
        ViewModel.PreviewCardViewModel = cardViewModel;
      }
    }
    private void CardListView_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
      // Move card preview image to mouse position when hovering over on list view item.
      // Position is clamped to window size
      Point offset = new(50, -100);
      Point pointerPosition = e.GetCurrentPoint(null).Position;
      //Rect windowBounds = this.Bounds;
      //PreviewImage.SetValue(Canvas.LeftProperty, Math.Clamp(pointerPosition.X + offset.X, 0, windowBounds.Width - PreviewImage.ActualWidth));
      //PreviewImage.SetValue(Canvas.TopProperty, Math.Clamp(pointerPosition.Y + offset.Y, 0, windowBounds.Height - PreviewImage.ActualHeight));
    }
    private void CardListViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
    {
      PreviewImage.Visibility = Visibility.Collapsed;
    }
    private void ScryfallCard_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
      //pointerOverCard = true;
    }
    private void ScryfallCard_PointerExited(object sender, PointerRoutedEventArgs e)
    {
      //pointerOverCard = false;
    }
    #endregion

    #region //--------------------- Drag & Drop -----------------------------//
    private void CardGridView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
      MTGCardViewModel viewModel = (e.Items[0] as MTGCardViewModel);
      if (viewModel != null)
      {
        e.Data.SetText(viewModel.ModelToJSON());
        e.Data.RequestedOperation = DataPackageOperation.Copy;
      }
    }

    private void CollectionListView_DragOver(object sender, DragEventArgs e)
    {
      if (e.DataView.Contains(StandardDataFormats.Text))
      {
        e.AcceptedOperation = DataPackageOperation.Copy;
      }
    }

    private async void CollectionListView_Drop(object sender, DragEventArgs e)
    {
      if (e.DataView.Contains(StandardDataFormats.Text))
      {
        DragOperationDeferral def = e.GetDeferral();
        string data = await e.DataView.GetTextAsync();

        try
        {
          var model = JsonSerializer.Deserialize<MTGCardModel>(data);
          if (model.Info.Id == string.Empty || model.Info.Id == null) { throw new Exception(); }
          ViewModel.CollectionViewModel.AddAndSort(model);
        }
        catch (Exception) { }

        e.AcceptedOperation = DataPackageOperation.Copy;
        def.Complete();
      }
    }
    #endregion
  }
}

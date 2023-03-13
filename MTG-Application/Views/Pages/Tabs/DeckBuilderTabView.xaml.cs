using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using MTGApplication.ViewModels;
using System;
using Windows.ApplicationModel.DataTransfer;
using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Models;
using System.Text.Json;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.API;
using MTGApplication.Database.Repositories;
using CommunityToolkit.WinUI.UI;

namespace MTGApplication.Views
{
  [ObservableObject]
  public sealed partial class DeckBuilderTabView : UserControl
  {
    public DeckBuilderTabView()
    {
      this.InitializeComponent();
      App.MainWindow.Closed += MainWindow_Closed;
    }

    private async void MainWindow_Closed(object sender, WindowEventArgs args)
    {
      if (DeckBuilderViewModel.HasUnsavedChanges)
      {
        args.Handled = true;
        if (await DeckBuilderViewModel.ShowUnsavedDialogs())
        {
          // UnsavedChanges needs to be set to false, otherwise the window would not close
          // until the used saves the deck.
          DeckBuilderViewModel.HasUnsavedChanges = false;
          App.MainWindow.Close();
        }
      }
    }

    public MTGSearchViewModel SearchViewModel = new(new ScryfallAPI());
    public DeckBuilderViewModel DeckBuilderViewModel = new(new ScryfallAPI(), new SQLiteMTGDeckRepository(new ScryfallAPI(), new Database.CardDbContextFactory()));

    [ObservableProperty]
    private double searchDesiredItemWidth = 250;
    [ObservableProperty]
    private bool searchPanelOpen = false;
    [ObservableProperty]
    private double deckDesiredItemWidth = 250;

    [RelayCommand]
    public void SwitchSearchPanel() => SearchPanelOpen = !SearchPanelOpen;

    #region Pointer Events
    // Prevents dropping to the same element as the element that started the dragging
    private object draggedElement;

    private void CardListViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
      // Change card preview image to hovered item
      if ((sender as FrameworkElement)?.DataContext is MTGCardViewModel cardVM)
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
    private void CardView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
      if (e.Items[0] is MTGCardViewModel vm)
      {
        e.Data.SetText(vm.Model.ToJSON());
        e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
        draggedElement = sender;
      }
    }
    private void CardView_DragOver(object sender, DragEventArgs e)
    {
      if (e.DataView.Contains(StandardDataFormats.Text) && !sender.Equals(draggedElement))
      {
        // Change operation to 'Move' if the shift key is down
        e.AcceptedOperation =
          (e.Modifiers & Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift) == Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift
          ? DataPackageOperation.Move : DataPackageOperation.Copy;
      }
    }
    private async void CardView_Drop(object sender, DragEventArgs e)
    {
      if (e.DataView.Contains(StandardDataFormats.Text))
      {
        DragOperationDeferral def = e.GetDeferral();
        string data = await e.DataView.GetTextAsync();

        if ((sender as FrameworkElement)?.DataContext is DeckBuilderViewModel.Cardlist cardlist)
        {
          var card = new Func<MTGCard>(() =>
          {
            // Try to import from JSON
            try
            {
              var card = JsonSerializer.Deserialize<MTGCard>(data);
              if (string.IsNullOrEmpty(card?.Info.Name)) { throw new Exception("Card does not have name"); }
              return card;
            }
            catch { return null; }
          })();

          if (card != null)
          {
            // Card was dragged from MTGApplication
            if (cardlist.AddToCardlistCommand.CanExecute(card))
            {
              cardlist.AddToCardlistCommand.Execute(card);
            }
          }
          else
          {
            // Try to import from EDHREC URL
            if (Uri.TryCreate(data, UriKind.Absolute, out Uri uri) && uri.Host == "edhrec.com")
            {
              var cardName = uri.Segments[^1];
              await cardlist.ImportCards(cardName);
            }
            else
            {
              // Try to import from string
              await cardlist.ImportCards(data);
            }
          }
        }
        def.Complete();
      }
    }
    private void CardView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
      // Remove original item if the operation is 'Move'
      if ((args.DropResult & DataPackageOperation.Move) == DataPackageOperation.Move
        && args.Items[0] is MTGCardViewModel cardVM
        && (sender as FrameworkElement)?.DataContext is DeckBuilderViewModel.Cardlist cardlist)
      {
        if (cardlist.RemoveFromCardlistCommand.CanExecute(cardVM.Model))
        {
          cardlist.RemoveFromCardlistCommand.Execute(cardVM.Model);
        }
      }
      draggedElement = null;
    }
    #endregion

    private void CardView_KeyDown(object sender, KeyRoutedEventArgs e)
    {
      if (e.Key == Windows.System.VirtualKey.Delete && sender is ListViewBase element && element.SelectedItem is MTGCardViewModel cardVM)
      {
        if (cardVM.DeleteCardCommand.CanExecute(null)) { cardVM.DeleteCardCommand.Execute(cardVM.Model); }
      }
    }
    private void CardView_LosingFocus(object sender, RoutedEventArgs e)
    {
      if(sender is ListViewBase element) { element.DeselectAll(); }
    }
  }
}
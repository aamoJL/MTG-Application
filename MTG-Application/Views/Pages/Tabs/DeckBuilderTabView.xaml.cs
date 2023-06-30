using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using MTGApplication.API;
using MTGApplication.Database.Repositories;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using System;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.ViewModels.DeckBuilderViewModel;

namespace MTGApplication.Views.Pages.Tabs;

/// <summary>
/// Code behind for DeckBuilder Tab
/// </summary>
[ObservableObject]
public sealed partial class DeckBuilderTabView : UserControl
{
  public DeckBuilderTabView()
  {
    this.InitializeComponent();
    App.Closing += MainWindow_Closed;

    var cardAPI = new ScryfallAPI();
    DeckBuilderViewModel = new(cardAPI, new SQLiteMTGDeckRepository(cardAPI, cardDbContextFactory: new()));
    SearchViewModel = new(cardAPI);
  }

  private void MainWindow_Closed(object sender, App.WindowClosingEventArgs args) => args.ClosingTasks.Add(DeckBuilderViewModel);

  public DeckBuilderAPISearchViewModel SearchViewModel { get; }
  public DeckBuilderViewModel DeckBuilderViewModel { get; }

  [ObservableProperty]
  private double searchDesiredItemWidth = 250;
  [ObservableProperty]
  private bool searchPanelOpen = false;
  [ObservableProperty]
  private double deckDesiredItemWidth = 250;

  /// <summary>
  /// Opens and closes search panel
  /// </summary>
  [RelayCommand]
  public void SwitchSearchPanel() => SearchPanelOpen = !SearchPanelOpen;

  #region Pointer Events

  /// <summary>
  /// Custom drag and drop args.
  /// This class is used for card drag and drop actions because the default drag and drop system did not work well with async methods.
  /// </summary>
  private class DragArgs
  {
    public DragArgs(object dragStartElement, Cardlist dragOrigin, MTGCard dragItem)
    {
      DragStartElement = dragStartElement;
      DragOriginList = dragOrigin;
      DragItem = dragItem;
    }

    public object DragStartElement { get; set; }
    public Cardlist DragOriginList { get; set; }
    public MTGCard DragItem { get; set; }
  }

  private DragArgs dragArgs;

  private void PreviewableCard_PointerEntered(object sender, PointerRoutedEventArgs e)
  {
    // Change card preview image to hovered item
    if (sender is FrameworkElement { DataContext: MTGCardViewModel cardVM })
    {
      PreviewImage.Visibility = Visibility.Visible;
      PreviewImage.Source = new BitmapImage(new(cardVM.SelectedFaceUri));
    }
  }

  private void PreviewableCard_PointerMoved(object sender, PointerRoutedEventArgs e)
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

  private void PreviewableCard_PointerExited(object sender, PointerRoutedEventArgs e)
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
      var originList = (sender as FrameworkElement).DataContext as Cardlist;
      dragArgs = new(sender, originList, vm.Model);
    }
  }

  private void CardView_DragOver(object sender, DragEventArgs e)
  {
    if (e.DataView.Contains(StandardDataFormats.Text) && !sender.Equals(dragArgs?.DragStartElement))
    {
      // Change operation to 'Move' if the shift key is down
      e.AcceptedOperation =
        (e.Modifiers & global::Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift)
        == global::Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift
        ? DataPackageOperation.Move : DataPackageOperation.Copy;
    }
  }

  private async void CardView_Drop(object sender, DragEventArgs e)
  {
    var def = e.GetDeferral();
    var data = await e.DataView.GetTextAsync();
    var operation = e.AcceptedOperation;
    var targetList = (sender as FrameworkElement).DataContext as Cardlist;

    if (!string.IsNullOrEmpty(data))
    {
      if (dragArgs?.DragOriginList != null && dragArgs?.DragItem != null)
      {
        // Dragged from cardlist
        if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
        {
          await targetList?.AddToCardlist(dragArgs.DragItem);
        }
        else if ((operation & DataPackageOperation.Move) == DataPackageOperation.Move)
        {
          await targetList?.MoveToCardlist(dragArgs.DragItem, dragArgs.DragOriginList);
        }
      }
      else if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy
        || (operation & DataPackageOperation.Move) == DataPackageOperation.Move
        && !string.IsNullOrEmpty(data))
      {
        var card = new Func<MTGCard>(() =>
        {
          // Try to import from JSON
          try
          {
            var card = JsonSerializer.Deserialize<MTGCard>(data);
            if (string.IsNullOrEmpty(card?.Info.Name))
            { throw new Exception("Card does not have name"); }
            return card;
          }
          catch { return null; }
        })();

        if (card != null)
        {
          await targetList?.AddToCardlist(card);
        }
        else
        {
          if (Uri.TryCreate(data, UriKind.Absolute, out var uri) && uri.Host == "edhrec.com")
          {
            // Try to import from EDHREC URL
            var cardName = uri.Segments[^1];
            await targetList?.ImportCards(cardName);
          }
          else
          {
            // Try to import from string
            await targetList?.ImportCards(data);
          }
        }
      }
    }

    dragArgs = null;
    def.Complete();
  }

  private void CommanderView_DragOver(object sender, DragEventArgs e)
  {
    if (e.DataView.Contains(StandardDataFormats.Text) && !sender.Equals(dragArgs?.DragStartElement))
    {
      // Change operation to 'Move' if the shift key is down
      e.AcceptedOperation =
        (e.Modifiers & global::Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift)
        == global::Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift
        ? DataPackageOperation.Move : DataPackageOperation.Copy;

      if (e.AcceptedOperation == DataPackageOperation.Move) { e.DragUIOverride.Caption = "Move to Commander"; }
      else if (e.AcceptedOperation == DataPackageOperation.Copy) { e.DragUIOverride.Caption = "Copy to Commander"; }

      e.DragUIOverride.IsContentVisible = false;
    }
  }

  private async void CommanderView_Drop(object sender, DragEventArgs e)
  {
    var def = e.GetDeferral();
    var data = await e.DataView.GetTextAsync();
    var operation = e.AcceptedOperation;

    if (!string.IsNullOrEmpty(data))
    {
      if (dragArgs?.DragOriginList != null && dragArgs?.DragItem != null)
      {
        // Dragged from cardlist
        if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
        {
          DeckBuilderViewModel.SetCommander(dragArgs.DragItem);
        }
        else if ((operation & DataPackageOperation.Move) == DataPackageOperation.Move)
        {
          DeckBuilderViewModel.MoveToCommander(dragArgs.DragItem, dragArgs.DragOriginList);
        }
      }
      else if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy
        || (operation & DataPackageOperation.Move) == DataPackageOperation.Move
        && !string.IsNullOrEmpty(data))
      {
        var card = new Func<MTGCard>(() =>
        {
          // Try to import from JSON
          try
          {
            var card = JsonSerializer.Deserialize<MTGCard>(data);
            if (string.IsNullOrEmpty(card?.Info.Name))
            { throw new Exception("Card does not have name"); }
            return card;
          }
          catch { return null; }
        })();

        if (card != null)
        {
          DeckBuilderViewModel.SetCommander(card);
        }
      }
    }
    def.Complete();
  }

  private void CommanderPartnerView_DragOver(object sender, DragEventArgs e)
  {
    if (e.DataView.Contains(StandardDataFormats.Text) && !sender.Equals(dragArgs?.DragStartElement))
    {
      // Change operation to 'Move' if the shift key is down
      e.AcceptedOperation =
        (e.Modifiers & global::Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift)
        == global::Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift
        ? DataPackageOperation.Move : DataPackageOperation.Copy;

      if (e.AcceptedOperation == DataPackageOperation.Move) { e.DragUIOverride.Caption = "Move to Partner"; }
      else if (e.AcceptedOperation == DataPackageOperation.Copy) { e.DragUIOverride.Caption = "Copy to Partner"; }

      e.DragUIOverride.IsContentVisible = false;
    }
  }

  private async void CommanderPartnerView_Drop(object sender, DragEventArgs e)
  {
    var def = e.GetDeferral();
    var data = await e.DataView.GetTextAsync();
    var operation = e.AcceptedOperation;

    if (!string.IsNullOrEmpty(data))
    {
      if (dragArgs?.DragOriginList != null && dragArgs?.DragItem != null)
      {
        // Dragged from cardlist
        if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
        {
          DeckBuilderViewModel.SetCommanderPartner(dragArgs.DragItem);
        }
        else if ((operation & DataPackageOperation.Move) == DataPackageOperation.Move)
        {
          DeckBuilderViewModel.MoveToCommanderPartner(dragArgs.DragItem, dragArgs.DragOriginList);
        }
      }
      else if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy
        || (operation & DataPackageOperation.Move) == DataPackageOperation.Move
        && !string.IsNullOrEmpty(data))
      {
        var card = new Func<MTGCard>(() =>
        {
          // Try to import from JSON
          try
          {
            var card = JsonSerializer.Deserialize<MTGCard>(data);
            if (string.IsNullOrEmpty(card?.Info.Name))
            { throw new Exception("Card does not have name"); }
            return card;
          }
          catch { return null; }
        })();

        if (card != null)
        {
          DeckBuilderViewModel.SetCommanderPartner(card);
        }
      }
    }
    def.Complete();
  }

  #endregion

  private void CardView_KeyDown(object sender, KeyRoutedEventArgs e)
  {
    if (e.Key == global::Windows.System.VirtualKey.Delete && sender is ListViewBase element && element.SelectedItem is MTGCardViewModel cardVM)
    {
      if (cardVM.DeleteCardCommand.CanExecute(null)) { cardVM.DeleteCardCommand.Execute(cardVM.Model); }
    }
  }

  private void CardView_LosingFocus(object sender, RoutedEventArgs e)
  {
    if (sender is ListViewBase element)
    { element.DeselectAll(); }
  }

  private void Root_KeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) => DeckBuilderViewModel.CardFilters.Reset();

  private void CommanderView_DragStarting(UIElement sender, DragStartingEventArgs args)
  {
    if ((sender as FrameworkElement).DataContext is MTGCardViewModel vm && vm != null)
    {
      args.Data.SetText(vm.Model.ToJSON());
      args.Data.RequestedOperation = DataPackageOperation.Copy;
      dragArgs = new(sender, null, vm.Model);
    }
    else { args.Cancel = true; }
  }
}
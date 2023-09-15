using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTGApplication.API;
using MTGApplication.Database.Repositories;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Services;
using MTGApplication.ViewModels;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.Views.Controls.MTGCardPreviewControl;

namespace MTGApplication.Views.Pages.Tabs;

/// <summary>
/// Code behind for DeckBuilder Tab
/// </summary>
[ObservableObject]
public sealed partial class DeckBuilderTabView : UserControl, ISavable
{
  public DeckBuilderTabView(CardPreviewProperties previewProperties)
  {
    InitializeComponent();
    CardPreviewProperties = previewProperties;

    DeckBuilderViewModel = new(CardAPI, new SQLiteMTGDeckRepository(CardAPI, cardDbContextFactory: new()), new());
    DeckBuilderViewModel.OnNotification += (s, args) => NotificationService.RaiseNotification(XamlRoot, args);
    DeckBuilderViewModel.OnGetDialogWrapper += (s, args) => DialogService.ShowAsync(XamlRoot, args);
  }

  private DragArgs dragArgs;

  #region Properties
  [ObservableProperty] private double deckDesiredItemWidth = 250;
  [ObservableProperty] private bool isClosable = true;

  public DeckBuilderViewModel DeckBuilderViewModel { get; }
  public ICardAPI<MTGCard> CardAPI { get; } = new ScryfallAPI();
  public CardPreviewProperties CardPreviewProperties { get; }
  #endregion

  #region ISavable Implementation
  public bool HasUnsavedChanges { get => DeckBuilderViewModel.HasUnsavedChanges; set => DeckBuilderViewModel.HasUnsavedChanges = value; }

  public async Task<bool> SaveUnsavedChanges() => await DeckBuilderViewModel.SaveUnsavedChanges();
  #endregion

  public async Task<bool> TabCloseRequested() => !DeckBuilderViewModel.HasUnsavedChanges || await DeckBuilderViewModel.SaveUnsavedChanges();

  #region Pointer Events
  // Preview
  private void PreviewableCard_PointerEntered(object sender, PointerRoutedEventArgs e)
    // Change card preview image to hovered item
    => CardPreviewProperties.CardViewModel = (sender as FrameworkElement)?.DataContext as MTGCardViewModel;

  private void PreviewableCard_PointerMoved(object sender, PointerRoutedEventArgs e)
  {
    // Move card preview image to mouse position when hovering over on list view item.
    // The position is clamped to element size
    var point = e.GetCurrentPoint(null).Position;
    CardPreviewProperties.Coordinates = new((float)point.X, (float)point.Y);
  }

  private void PreviewableCard_PointerExited(object sender, PointerRoutedEventArgs e)
    => CardPreviewProperties.CardViewModel = null;

  // Drag and drop
  private void CardView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
  {
    if (e.Items[0] is MTGCardViewModel vm)
    {
      e.Data.SetText(vm.Model.ToJSON());
      e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
      var originList = (sender as FrameworkElement).DataContext as DeckCardlistViewModel;
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
    var targetList = (sender as FrameworkElement).DataContext as DeckCardlistViewModel;

    var card = dragArgs?.DragItem ?? new Func<MTGCard>(() =>
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

    if (!string.IsNullOrEmpty(data))
    {
      if (card != null)
      {
        // Dragged from this application
        if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy || dragArgs?.DragStartElement == null)
        {
          await targetList?.Add(new(card.Info, card.Count));
        }
        else if ((operation & DataPackageOperation.Move) == DataPackageOperation.Move)
        {
          await targetList?.Move(card, dragArgs.DragOriginList);
        }
      }
      else if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy
        || (operation & DataPackageOperation.Move) == DataPackageOperation.Move
        && !string.IsNullOrEmpty(data))
      {
        await targetList?.Import(data);
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
      if (dragArgs?.DragStartElement != null && dragArgs?.DragItem != null)
      {
        // Dragged from cardlist
        if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
        {
          DeckBuilderViewModel.SetCommander(new(dragArgs.DragItem.Info));
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
        await DeckBuilderViewModel.ImportCommander(data);
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
      if (dragArgs?.DragStartElement != null && dragArgs?.DragItem != null)
      {
        // Dragged from cardlist
        if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
        {
          DeckBuilderViewModel.SetCommanderPartner(new(dragArgs.DragItem.Info));
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
        await DeckBuilderViewModel.ImportCommanderPartner(data);
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
    if (sender is ListViewBase element) { element.DeselectAll(); }
  }

  private void Root_KeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    => DeckBuilderViewModel.CardFilters.Reset();

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

// Drag args
public sealed partial class DeckBuilderTabView
{
  /// <summary>
  /// Custom drag and drop args.
  /// This class is used for card drag and drop actions because the default drag and drop system did not work well with async methods.
  /// </summary>
  public class DragArgs
  {
    public DragArgs(object dragStartElement, DeckCardlistViewModel dragOrigin, MTGCard dragItem)
    {
      DragStartElement = dragStartElement;
      DragOriginList = dragOrigin;
      DragItem = dragItem;
    }

    public object DragStartElement { get; set; }
    public DeckCardlistViewModel DragOriginList { get; set; }
    public MTGCard DragItem { get; set; }
  }
}
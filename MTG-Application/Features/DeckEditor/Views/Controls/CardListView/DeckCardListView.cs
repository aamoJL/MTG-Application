using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using MTGApplication.General.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.Views.DragAndDrop;
using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Features.DeckEditor.Views.Controls.CardListView;

public partial class DeckCardListView : ListView, CardDragArgs.IMoveOrigin
{
  public DeckCardListView()
  {
    DragItemsStarting += ListView_DragItemsStarting;
    LosingFocus += ListView_LosingFocus;

    var deleteAccelerator = new KeyboardAccelerator() { Key = Windows.System.VirtualKey.Delete };
    deleteAccelerator.Invoked += DeleteAccelerator_Invoked;
    KeyboardAccelerators.Add(deleteAccelerator);
  }

  public IAsyncRelayCommand<DeckEditorMTGCard>? OnDropCopy { get; set; }
  public IAsyncRelayCommand<string>? OnDropImport { get; set; }
  public IRelayCommand<DeckEditorMTGCard>? OnDropBeginMoveFrom { get; set; }
  public IAsyncRelayCommand<DeckEditorMTGCard>? OnDropBeginMoveTo { get; set; }
  public IRelayCommand? OnDropExecuteMove { get; set; }
  public IRelayCommand<DeckEditorMTGCard>? OnDeleteAcceleratorInvoked { get; set; }

  private void DeleteAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
  {
    // TODO: this does not work
    if (FocusState == FocusState.Unfocused
      || SelectedItem is not DeckEditorMTGCard selection)
      return;

    if (OnDeleteAcceleratorInvoked?.CanExecute(selection) == true)
    {
      var index = SelectedIndex;

      OnDeleteAcceleratorInvoked.Execute(selection);

      // Recalculate the index and focus the element in the index position if the element exists.
      if ((index = Math.Clamp(index, -1, Items.Count - 1)) >= 0)
      {
        (ContainerFromIndex(index) as UIElement)?.Focus(FocusState.Programmatic);
        SelectedIndex = index;
      }
    }
  }

  private void ListView_LosingFocus(UIElement sender, LosingFocusEventArgs args)
  {
    // Deselect list selection if the list loses focus
    //    so the delete keyboard accelerator does not delete item in the list
    if (args.NewFocusedElement is ListViewItem item && Items.Contains(item.Content))
      return;

    this.DeselectAll();

    args.Handled = true;
  }

  private void ListView_DragItemsStarting(object _, DragItemsStartingEventArgs e)
  {
    if (e.Items.FirstOrDefault() is not DeckCardViewModel dragItem)
    {
      NotificationService.RaiseNotification(this, new(NotificationService.NotificationType.Error, "No items to drag"));
      e.Cancel = true;
      return;
    }

    e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
    e.Data.Properties.Add(nameof(CardDragArgs), new CardDragArgs(dragItem.CopyModel(), origin: this));
  }

  protected override void OnDragEnter(DragEventArgs e)
  {
    e.Handled = true;

    // Block dropping if the origin is the same or the item is invalid
    if (e.Data.Properties.TryGetValue(nameof(CardDragArgs), out var prop) && prop is CardDragArgs args)
    {
      if (args.Origin.Equals(this)) e.AcceptedOperation = DataPackageOperation.None;
      else if (args.Item is DeckEditorMTGCard) e.AcceptedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
      else if (args.Item is MTGCard) e.AcceptedOperation = DataPackageOperation.Copy;
    }
    else if (e.DataView.Contains(StandardDataFormats.Text))
      e.AcceptedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
  }

  protected override void OnDragOver(DragEventArgs e)
  {
    e.Handled = true;

    if (e.AcceptedOperation == DataPackageOperation.None)
      return;

    // Change operation to 'Move' if the move modifier is down and move is an accepted operation.
    e.AcceptedOperation = (e.Modifiers & CardDragArgs.MoveModifier) == CardDragArgs.MoveModifier
      && (e.AllowedOperations & DataPackageOperation.Move) == DataPackageOperation.Move
    ? DataPackageOperation.Move : DataPackageOperation.Copy;
  }

  protected override async void OnDrop(DragEventArgs e)
  {
    var def = e.GetDeferral();

    e.Handled = true;

    e.Data.Properties.TryGetValue(nameof(CardDragArgs), out var prop);
    var args = (prop as CardDragArgs);

    if ((e.AcceptedOperation & DataPackageOperation.Move) == DataPackageOperation.Move)
    {
      // Move
      if (args?.Item is DeckEditorMTGCard editorCard)
      {
        var moveOrigin = args.Origin as CardDragArgs.IMoveOrigin;

        // Begin from
        if ((moveOrigin?.OnDropBeginMoveFrom?.CanExecute(editorCard) is true))
          moveOrigin.OnDropBeginMoveFrom.Execute(editorCard);

        // Begin to
        if (OnDropBeginMoveTo?.CanExecute(editorCard) is true)
          await OnDropBeginMoveTo.ExecuteAsync(editorCard);

        // Execute
        if (OnDropExecuteMove?.CanExecute(null) == true) OnDropExecuteMove.Execute(null);
        if (moveOrigin?.OnDropExecuteMove?.CanExecute(null) == true) moveOrigin.OnDropExecuteMove.Execute(null);
      }
    }
    else if ((e.AcceptedOperation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
    {
      if (args?.Item is MTGCard dropCard)
      {
        var editorCard = (dropCard as DeckEditorMTGCard) ?? new DeckEditorMTGCard(dropCard.Info);

        // Copy
        if (OnDropCopy?.CanExecute(editorCard) is true)
          await OnDropCopy.ExecuteAsync(editorCard);
      }
      else if (e.DataView.Contains(StandardDataFormats.Text) && await e.DataView.GetTextAsync() is string importText)
      {
        // Import
        if (OnDropImport?.CanExecute(importText) is true)
          await OnDropImport.ExecuteAsync(importText);
      }
    }

    def.Complete();
  }
}
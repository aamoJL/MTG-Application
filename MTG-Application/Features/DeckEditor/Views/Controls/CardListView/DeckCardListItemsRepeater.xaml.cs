using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCard;
using MTGApplication.General.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.Views.DragAndDrop;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Features.DeckEditor.Views.Controls.CardListView;

public partial class DeckCardListItemsRepeater : UserControl, CardDragArgs.IMoveOrigin
{
  public static readonly DependencyProperty ItemsSourceProperty =
      DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(DeckCardListItemsRepeater), new PropertyMetadata(null));

  public static readonly DependencyProperty ItemTemplateProperty =
      DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(DeckCardListItemsRepeater), new PropertyMetadata(null));

  public static readonly DependencyProperty LayoutProperty =
      DependencyProperty.Register(nameof(Layout), typeof(Layout), typeof(DeckCardListItemsRepeater), new PropertyMetadata(null));

  public static readonly DependencyProperty OnDropCopyProperty =
      DependencyProperty.Register(nameof(OnDropCopy), typeof(IAsyncRelayCommand<DeckEditorMTGCard>), typeof(DeckCardListItemsRepeater), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropImportProperty =
      DependencyProperty.Register(nameof(OnDropImport), typeof(IAsyncRelayCommand<string>), typeof(DeckCardListItemsRepeater), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveFromProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveFrom), typeof(IRelayCommand<DeckEditorMTGCard>), typeof(DeckCardListItemsRepeater), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveToProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveTo), typeof(IAsyncRelayCommand<DeckEditorMTGCard>), typeof(DeckCardListItemsRepeater), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropExecuteMoveProperty =
      DependencyProperty.Register(nameof(OnDropExecuteMove), typeof(IRelayCommand), typeof(DeckCardListItemsRepeater), new PropertyMetadata(default));

  public DeckCardListItemsRepeater()
  {
    InitializeComponent();

    var deleteAccelerator = new KeyboardAccelerator() { Key = Windows.System.VirtualKey.Delete };
    deleteAccelerator.Invoked += DeleteAccelerator_Invoked;
    KeyboardAccelerators.Add(deleteAccelerator);

    Repeater.DragItemsStarting += ItemsRepeater_DragItemsStarting;
  }

  public object ItemsSource
  {
    get => GetValue(ItemsSourceProperty);
    set => SetValue(ItemsSourceProperty, value);
  }
  public object ItemTemplate
  {
    get => GetValue(ItemTemplateProperty);
    set => SetValue(ItemTemplateProperty, value);
  }
  public Layout Layout
  {
    get => (Layout)GetValue(LayoutProperty);
    set => SetValue(LayoutProperty, value);
  }
  public bool CanDragItems { get; set; } = false;

  public IAsyncRelayCommand<DeckEditorMTGCard> OnDropCopy
  {
    get => (IAsyncRelayCommand<DeckEditorMTGCard>)GetValue(OnDropCopyProperty);
    set => SetValue(OnDropCopyProperty, value);
  }
  public IAsyncRelayCommand<string> OnDropImport
  {
    get => (IAsyncRelayCommand<string>)GetValue(OnDropImportProperty);
    set => SetValue(OnDropImportProperty, value);
  }
  public IRelayCommand<DeckEditorMTGCard> OnDropBeginMoveFrom
  {
    get => (IRelayCommand<DeckEditorMTGCard>)GetValue(OnDropBeginMoveFromProperty);
    set => SetValue(OnDropBeginMoveFromProperty, value);
  }
  public IAsyncRelayCommand<DeckEditorMTGCard> OnDropBeginMoveTo
  {
    get => (IAsyncRelayCommand<DeckEditorMTGCard>)GetValue(OnDropBeginMoveToProperty);
    set => SetValue(OnDropBeginMoveToProperty, value);
  }
  public IRelayCommand OnDropExecuteMove
  {
    get => (IRelayCommand)GetValue(OnDropExecuteMoveProperty);
    set => SetValue(OnDropExecuteMoveProperty, value);
  }
  public IRelayCommand<DeckEditorMTGCard>? OnDeleteAcceleratorInvoked { get; set; }

  private void DeleteAccelerator_Invoked(KeyboardAccelerator _, KeyboardAcceleratorInvokedEventArgs e)
  {
    if (Repeater.SelectedItem is not DeckCardViewModel selection)
      return;

    var item = selection.CopyModel();

    if (OnDeleteAcceleratorInvoked?.CanExecute(item) == true)
    {
      OnDeleteAcceleratorInvoked.Execute(item);

      e.Handled = true;
    }
  }

  private void ItemsRepeater_DragItemsStarting(object? item, DragStartingEventArgs e)
  {
    if (item is not DeckCardViewModel dragItem)
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
    if (e.DataView.Properties.TryGetValue(nameof(CardDragArgs), out var prop) && prop is CardDragArgs args)
    {
      if (args.Origin.Equals(this)) e.AcceptedOperation = DataPackageOperation.None;
      else if (args.Item is DeckEditorMTGCard) e.AcceptedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
      else if (args.Item is MTGCard) e.AcceptedOperation = DataPackageOperation.Copy;
    }
    else if (e.DataView.Contains(StandardDataFormats.Text))
    {
      e.AcceptedOperation = DataPackageOperation.Copy;
      e.DragUIOverride.Caption = "Import";
    }
  }

  protected override void OnDragOver(DragEventArgs e)
  {
    e.Handled = true;

    if (e.AcceptedOperation == DataPackageOperation.None)
      return;

    // Change operation to 'Move' if the move modifier is down and move is an accepted operation.
    if ((e.AllowedOperations & DataPackageOperation.Move) == DataPackageOperation.Move
      && (e.Modifiers & CardDragArgs.MoveModifier) == CardDragArgs.MoveModifier)
    {
      e.AcceptedOperation = DataPackageOperation.Move;
    }
    else
      e.AcceptedOperation = DataPackageOperation.Copy;
  }

  protected override async void OnDrop(DragEventArgs e)
  {
    var def = e.GetDeferral();

    e.Handled = true;

    e.DataView.Properties.TryGetValue(nameof(CardDragArgs), out var prop);
    var args = prop as CardDragArgs;

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

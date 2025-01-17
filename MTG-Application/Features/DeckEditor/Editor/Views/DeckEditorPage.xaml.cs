using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardListView;
using MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardView;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.Views.DragAndDrop;
using System;
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;

namespace MTGApplication.Features.DeckEditor.Views;
public sealed partial class DeckEditorPage : Page, INotifyPropertyChanged
{
  public enum CardViewType { Group, Image, Text }

  public DeckEditorPage()
  {
    InitializeComponent();

    Loaded += DeckEditorPage_Loaded;
  }

  private void DeckEditorPage_Loaded(object sender, RoutedEventArgs e)
  {
    Loaded -= DeckEditorPage_Loaded;

    DeckEditorViewDialogs.RegisterConfirmDialogs(ViewModel.Confirmers, root: XamlRoot);
    NotificationService.RegisterNotifications(ViewModel.Notifier, this);
  }

  public DeckEditorViewModel ViewModel { get; } = new(App.MTGCardImporter);

  public CardFilters CardFilter { get; } = new();
  public CardSorter CardSorter { get; } = new();
  public CardViewType DeckCardsViewType
  {
    get;
    set
    {
      if (field != value)
      {
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(DeckCardsViewType)));
      }
    }
  } = CardViewType.Image;

  public event PropertyChangedEventHandler? PropertyChanged;

  [RelayCommand]
  private void SetDeckDisplayType(string type)
  {
    if (Enum.TryParse<CardViewType>(type, out var result))
      DeckCardsViewType = result;
  }

  protected override void OnNavigatedTo(NavigationEventArgs e)
  {
    base.OnNavigatedTo(e);

    if (e.Parameter is string deckName && ViewModel.OpenDeckCommand.CanExecute(deckName))
      ViewModel.OpenDeckCommand.Execute(deckName);
  }

  private async void SaveDeckKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.SaveDeckCommand.CanExecute(null))
      await ViewModel.SaveDeckCommand.ExecuteAsync(null);
  }

  private async void OpenDeckKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.OpenDeckCommand.CanExecute(null))
      await ViewModel.OpenDeckCommand.ExecuteAsync(null);
  }

  private async void NewDeckKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.NewDeckCommand.CanExecute(null))
      await ViewModel.NewDeckCommand.ExecuteAsync(null);
  }

  private void ResetFiltersKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (CardFilter.ResetCommand.CanExecute(null))
      CardFilter.ResetCommand.Execute(null);
  }

  private void UndoKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.UndoCommand.CanExecute(null))
      ViewModel.UndoCommand.Execute(null);
  }

  private void RedoKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    args.Handled = true;

    if (ViewModel.RedoCommand.CanExecute(null))
      ViewModel.RedoCommand.Execute(null);
  }

  private void DeleteCardKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
  {
    if (args.Element is ListViewBase listview)
    {
      if (listview.SelectedIndex == -1
        || listview.DataContext is not CardListViewModel listViewViewModel
        || (listview.SelectedIndex is int index && index < 0)
        || listview.Items[index] is not ItemCollection selectedCard
        || listViewViewModel.RemoveCardCommand.CanExecute(selectedCard))
        return;

      listViewViewModel.RemoveCardCommand.Execute(selectedCard);

      // Recalculate the index and focus the element in the index position if the element exists.
      if ((index = Math.Clamp(index, -1, listview.Items.Count - 1)) >= 0)
      {
        (listview.ContainerFromIndex(index) as UIElement)?.Focus(FocusState.Programmatic);

        listview.SelectedIndex = index;
      }

      args.Handled = true;
    }
    else if (args.Element is ItemsView itemsView)
    {
      if (itemsView.SelectedItem is not DeckEditorMTGCard selectedItem
        || itemsView.DataContext is not CardListViewModel itemsViewViewModel
        || itemsView.ItemsSource is not IList source
        || (source.IndexOf(selectedItem) is int index && index < 0)
        || !itemsViewViewModel.RemoveCardCommand.CanExecute(selectedItem))
        return;

      itemsViewViewModel.RemoveCardCommand.Execute(selectedItem);

      itemsView.Select(index < source.Count ? index : index - 1);

      args.Handled = true;
    }
  }

  private void ListView_LosingFocus(UIElement sender, LosingFocusEventArgs args)
  {
    // Deselect list selection if the list loses focus
    //    so the delete keyboard accelerator does not delete item in the list
    if (sender is ListViewBase listview)
    {
      if (args.NewFocusedElement is ListViewItem item
        && listview.Items.Contains(item.Content))
        return;

      listview.DeselectAll();

      args.Handled = true;
    }
    else if (sender is ItemsView itemsView)
    {
      if (args.NewFocusedElement is ItemContainer itemContainer
        && itemContainer.FindParent<ItemsView>() == itemsView)
        return;

      itemsView.DeselectAll();

      args.Handled = true;
    }
  }
}

public partial class AdvancedCardItemsView : ItemsView
{
  public static readonly DependencyProperty OnDropCopyProperty =
      DependencyProperty.Register(nameof(OnDropCopy), typeof(IAsyncRelayCommand), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropImportProperty =
      DependencyProperty.Register(nameof(OnDropImport), typeof(IAsyncRelayCommand), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveFromProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveFrom), typeof(IRelayCommand), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveToProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveTo), typeof(IAsyncRelayCommand), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropExecuteMoveProperty =
      DependencyProperty.Register(nameof(OnDropExecuteMove), typeof(IRelayCommand), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(default));

  protected DragAndDrop<CardMoveArgs>? DragAndDrop => field ??= new()
  {
    OnCopy = async (item) => await (OnDropCopy?.ExecuteAsync(new DeckEditorMTGCard(item.Card.Info, item.Count)) ?? Task.CompletedTask),
    OnExternalImport = async (data) => await (OnDropImport?.ExecuteAsync(data) ?? Task.CompletedTask),
    OnBeginMoveTo = async (item) => await (OnDropBeginMoveTo?.ExecuteAsync((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count)) ?? Task.CompletedTask),
    OnBeginMoveFrom = (item) => OnDropBeginMoveFrom?.Execute((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count)),
    OnExecuteMove = (item) => OnDropExecuteMove?.Execute((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count))
  };

  public IAsyncRelayCommand OnDropCopy
  {
    get => (IAsyncRelayCommand)GetValue(OnDropCopyProperty);
    set => SetValue(OnDropCopyProperty, value);
  }
  public IAsyncRelayCommand OnDropImport
  {
    get => (IAsyncRelayCommand)GetValue(OnDropImportProperty);
    set => SetValue(OnDropImportProperty, value);
  }
  public IRelayCommand OnDropBeginMoveFrom
  {
    get => (IRelayCommand)GetValue(OnDropBeginMoveFromProperty);
    set => SetValue(OnDropBeginMoveFromProperty, value);
  }
  public IAsyncRelayCommand OnDropBeginMoveTo
  {
    get => (IAsyncRelayCommand)GetValue(OnDropBeginMoveToProperty);
    set => SetValue(OnDropBeginMoveToProperty, value);
  }
  public IRelayCommand OnDropExecuteMove
  {
    get => (IRelayCommand)GetValue(OnDropExecuteMoveProperty);
    set => SetValue(OnDropExecuteMoveProperty, value);
  }

  protected override async void OnDrop(DragEventArgs e)
  {
    var def = e.GetDeferral();

    await DragAndDrop!.Drop(
      e.AcceptedOperation,
      e.DataView.Contains(StandardDataFormats.Text) ? await e.DataView.GetTextAsync() : string.Empty);

    def.Complete();
  }

  protected override void OnDragOver(DragEventArgs e) => DragAndDrop?.DragOver(e);
}

public partial class AdvancedDeckEditorCardImageView : DeckEditorCardImageView
{
  public AdvancedDeckEditorCardImageView() : base()
  {
    DragStarting += AdvancedDeckEditorCardImageView_DragStarting;
    DropCompleted += AdvancedDeckEditorCardImageView_DropCompleted;
  }

  protected DragAndDrop<CardMoveArgs>? DragAndDrop => field ??= new()
  {
    OnBeginMoveFrom = (item) => OnDropBeginMoveFrom?.Execute((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count)),
  };

  private async void AdvancedDeckEditorCardImageView_DragStarting(UIElement sender, DragStartingEventArgs args)
  {
    var deferral = args.GetDeferral();

    DragAndDrop!.OnDragStarting(new CardMoveArgs(Model, Model.Count), out var operation);

    // Set the drag UI to the image element of the dragged element
    args.DragUI.SetContentFromSoftwareBitmap(await GetDragUI(), args.GetPosition(ImageElement));
    args.Data.RequestedOperation = operation;

    deferral.Complete();
  }

  private void AdvancedDeckEditorCardImageView_DropCompleted(UIElement sender, DropCompletedEventArgs args)
    => DragAndDrop!.DropCompleted();

  private async Task<SoftwareBitmap> GetDragUI()
  {
    var renderTargetBitmap = new RenderTargetBitmap();
    await renderTargetBitmap.RenderAsync(ImageElement);

    return SoftwareBitmap.CreateCopyFromBuffer(
      await renderTargetBitmap.GetPixelsAsync(),
      BitmapPixelFormat.Bgra8,
      renderTargetBitmap.PixelWidth,
      renderTargetBitmap.PixelHeight,
      BitmapAlphaMode.Premultiplied);
  }
}
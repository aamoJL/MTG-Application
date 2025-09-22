using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Views.Controls;
using MTGApplication.General.Views.DragAndDrop;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardListView;

public partial class DeckEditorListViewContainer : UserControl
{
  public static readonly DependencyProperty ItemsSourceProperty =
      DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(DeckEditorListViewContainer), new PropertyMetadata(default));

  public static readonly DependencyProperty ItemTemplateProperty =
      DependencyProperty.Register(nameof(ItemTemplate), typeof(object), typeof(DeckEditorListViewContainer), new PropertyMetadata(default));

  public static readonly DependencyProperty LayoutProperty =
      DependencyProperty.Register(nameof(Layout), typeof(Layout), typeof(DeckEditorListViewContainer), new PropertyMetadata(new UniformGridLayout()));

  public static readonly DependencyProperty OnDropCopyProperty =
      DependencyProperty.Register(nameof(OnDropCopy), typeof(IAsyncRelayCommand), typeof(DeckEditorListViewContainer), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropImportProperty =
      DependencyProperty.Register(nameof(OnDropImport), typeof(IAsyncRelayCommand), typeof(DeckEditorListViewContainer), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveFromProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveFrom), typeof(IRelayCommand), typeof(DeckEditorListViewContainer), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveToProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveTo), typeof(IAsyncRelayCommand), typeof(DeckEditorListViewContainer), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropExecuteMoveProperty =
      DependencyProperty.Register(nameof(OnDropExecuteMove), typeof(IRelayCommand), typeof(DeckEditorListViewContainer), new PropertyMetadata(default));

  public DeckEditorListViewContainer()
  {
    InitializeComponent();

    Drop += OnDrop;
    DragOver += OnDragOver;
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
  public bool CenterOnFocus { get; set; } = false;

  [NotNull]
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

  protected virtual void OnDragOver(object sender, DragEventArgs e)
  {
    if (ItemsSource is IList source
      && source.Cast<object>().Contains(DragAndDrop<CardMoveArgs>.Item?.Card))
      return;

    DragAndDrop?.DragOver(e);

    e.Handled = true;
  }

  protected virtual async void OnDrop(object sender, DragEventArgs e)
  {
    var def = e.GetDeferral();

    await DragAndDrop.Drop(
      e.AcceptedOperation,
      e.DataView.Contains(StandardDataFormats.Text) ? await e.DataView.GetTextAsync() : string.Empty);

    e.Handled = true;

    def.Complete();
  }

  private void KeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
  {
    if (args.Element is SelectableItemsRepeater sir)
    {
      if (sir.SelectedItem is not DeckEditorMTGCard selectedItem
        || sir.DataContext is not ICardListViewModel itemsViewViewModel
        || sir.ItemsSource is not IList source
        || (source.IndexOf(selectedItem) is int index && index < 0)
        || itemsViewViewModel.RemoveCardCommand?.CanExecute(selectedItem) is not true)
        return;

      itemsViewViewModel.RemoveCardCommand.Execute(selectedItem);

      args.Handled = true;
    }
  }
}

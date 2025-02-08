using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardListView;

[Obsolete("ItemsView is very unstable on version: 1.5.250108004. Use ListViewContainers instead.")]
public partial class AdvancedCardItemsView : ItemsView
{
  public static readonly DependencyProperty OnDropCopyProperty =
      DependencyProperty.Register(nameof(OnDropCopy), typeof(IAsyncRelayCommand), typeof(AdvancedCardItemsView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropImportProperty =
      DependencyProperty.Register(nameof(OnDropImport), typeof(IAsyncRelayCommand), typeof(AdvancedCardItemsView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveFromProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveFrom), typeof(IRelayCommand), typeof(AdvancedCardItemsView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveToProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveTo), typeof(IAsyncRelayCommand), typeof(AdvancedCardItemsView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropExecuteMoveProperty =
      DependencyProperty.Register(nameof(OnDropExecuteMove), typeof(IRelayCommand), typeof(AdvancedCardItemsView), new PropertyMetadata(default));

  public static readonly DependencyProperty LoseSelectionWithFocusProperty =
      DependencyProperty.Register(nameof(LoseSelectionWithFocus), typeof(bool), typeof(AdvancedCardItemsView), new PropertyMetadata(false));

  public AdvancedCardItemsView()
    => LosingFocus += ItemsView_LosingFocus;

  [NotNull]
  protected DragAndDrop<CardMoveArgs>? DragAndDrop => field ??= new()
  {
    OnCopy = async (item) => await (OnDropCopy?.ExecuteAsync(new DeckEditorMTGCard(item.Card.Info, item.Count)) ?? Task.CompletedTask),
    OnExternalImport = async (data) => await (OnDropImport?.ExecuteAsync(data) ?? Task.CompletedTask),
    OnBeginMoveTo = async (item) => await (OnDropBeginMoveTo?.ExecuteAsync((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count)) ?? Task.CompletedTask),
    OnBeginMoveFrom = (item) => OnDropBeginMoveFrom?.Execute((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count)),
    OnExecuteMove = (item) => OnDropExecuteMove?.Execute((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count))
  };

  public bool LoseSelectionWithFocus
  {
    get => (bool)GetValue(LoseSelectionWithFocusProperty);
    set => SetValue(LoseSelectionWithFocusProperty, value);
  }

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

    await DragAndDrop.Drop(
      e.AcceptedOperation,
      e.DataView.Contains(StandardDataFormats.Text) ? await e.DataView.GetTextAsync() : string.Empty);

    def.Complete();
  }

  protected override void OnDragOver(DragEventArgs e)
  {
    base.OnDragOver(e);

    if (ItemsSource is IList source
      && source.Cast<object>().Contains(DragAndDrop<CardMoveArgs>.Item?.Card))
      return;

    DragAndDrop?.DragOver(e);
  }

  private void ItemsView_LosingFocus(UIElement sender, Microsoft.UI.Xaml.Input.LosingFocusEventArgs args)
  {
    if (!LoseSelectionWithFocus
      || (args.NewFocusedElement is ItemContainer itemContainer
        && itemContainer.FindParent<ItemsView>() == this))
      return;

    DeselectAll();
  }
}
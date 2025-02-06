using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using MTGApplication.Features.DeckEditor.Editor.Models;
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

public partial class DeckEditorCardItemsRepeater : AdvancedItemsRepeater
{
  public static readonly DependencyProperty OnDropCopyProperty =
      DependencyProperty.Register(nameof(OnDropCopy), typeof(IAsyncRelayCommand), typeof(AdvancedItemsRepeater), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropImportProperty =
      DependencyProperty.Register(nameof(OnDropImport), typeof(IAsyncRelayCommand), typeof(AdvancedItemsRepeater), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveFromProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveFrom), typeof(IRelayCommand), typeof(AdvancedItemsRepeater), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveToProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveTo), typeof(IAsyncRelayCommand), typeof(AdvancedItemsRepeater), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropExecuteMoveProperty =
      DependencyProperty.Register(nameof(OnDropExecuteMove), typeof(IRelayCommand), typeof(AdvancedItemsRepeater), new PropertyMetadata(default));

  public DeckEditorCardItemsRepeater()
  {
    Drop += OnDrop;
    DragOver += OnDragOver;
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

  [NotNull]
  protected DragAndDrop<CardMoveArgs>? DragAndDrop => field ??= new()
  {
    OnCopy = async (item) => await (OnDropCopy?.ExecuteAsync(new DeckEditorMTGCard(item.Card.Info, item.Count)) ?? Task.CompletedTask),
    OnExternalImport = async (data) => await (OnDropImport?.ExecuteAsync(data) ?? Task.CompletedTask),
    OnBeginMoveTo = async (item) => await (OnDropBeginMoveTo?.ExecuteAsync((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count)) ?? Task.CompletedTask),
    OnBeginMoveFrom = (item) => OnDropBeginMoveFrom?.Execute((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count)),
    OnExecuteMove = (item) => OnDropExecuteMove?.Execute((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count))
  };

  protected virtual void OnDragOver(object sender, DragEventArgs e)
  {
    if (ItemsSource is IList source
      && source.Cast<object>().Contains(DragAndDrop<CardMoveArgs>.Item?.Card))
      return;

    DragAndDrop?.DragOver(e);
  }

  protected virtual async void OnDrop(object sender, DragEventArgs e)
  {
    var def = e.GetDeferral();

    await DragAndDrop.Drop(
      e.AcceptedOperation,
      e.DataView.Contains(StandardDataFormats.Text) ? await e.DataView.GetTextAsync() : string.Empty);

    def.Complete();
  }
}
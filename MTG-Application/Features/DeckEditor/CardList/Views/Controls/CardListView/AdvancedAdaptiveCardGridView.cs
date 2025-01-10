using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Views.Controls.AdaptiveGridView;
using MTGApplication.General.Views.DragAndDrop;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardListView;
public partial class AdvancedAdaptiveCardGridView : AdaptiveGridView
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

  public AdvancedAdaptiveCardGridView()
  {
    DragAndDrop = new(itemToArgsConverter: (item) => new CardMoveArgs(item, item.Count))
    {
      OnCopy = async (item) => await (OnDropCopy?.ExecuteAsync(new DeckEditorMTGCard(item.Card.Info, item.Count)) ?? Task.CompletedTask),
      OnExternalImport = async (data) => await (OnDropImport?.ExecuteAsync(data) ?? Task.CompletedTask),
      OnBeginMoveTo = async (item) => await (OnDropBeginMoveTo?.ExecuteAsync((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count)) ?? Task.CompletedTask),
      OnBeginMoveFrom = (item) => OnDropBeginMoveFrom?.Execute((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count)),
      OnExecuteMove = (item) => OnDropExecuteMove?.Execute((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count))
    };

    DragItemsStarting += DragAndDrop.DragStarting;
    DragItemsCompleted += DragAndDrop.DragCompleted;
  }

  protected ListViewDragAndDrop<DeckEditorMTGCard> DragAndDrop { get; }

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

  protected override void OnDrop(DragEventArgs e) => DragAndDrop.Drop(e);

  protected override void OnDragOver(DragEventArgs e) => DragAndDrop.DragOver(e);
}
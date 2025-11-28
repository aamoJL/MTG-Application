using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardListView;

public partial class AdvancedCardListView : ListView
{
  public AdvancedCardListView()
  {
    DragAndDrop = new(itemToArgsConverter: (item) => new CardMoveArgs(item, item.Count))
    {
      OnCopy = async (item) => await (OnDropCopy?.ExecuteAsync(new DeckEditorMTGCard(item.Card.Info, item.Count)) ?? Task.CompletedTask),
      OnExternalImport = async (data) => await (OnDropImport?.ExecuteAsync(data) ?? Task.CompletedTask),
      OnBeginMoveTo = async (item) => await (OnDropBeginMoveTo?.ExecuteAsync(new DeckEditorMTGCard(item.Card.Info, item.Count)) ?? Task.CompletedTask),
      OnBeginMoveFrom = (item) => OnDropBeginMoveFrom?.Execute(new DeckEditorMTGCard(item.Card.Info, item.Count)),
      OnExecuteMove = (item) => OnDropExecuteMove?.Execute(new DeckEditorMTGCard(item.Card.Info, item.Count))
    };

    DragItemsStarting += DragAndDrop.DragStarting;
  }

  private ListViewDragAndDrop<DeckEditorMTGCard> DragAndDrop { get; }

  public IAsyncRelayCommand? OnDropCopy { get; set; }
  public IAsyncRelayCommand? OnDropImport { get; set; }
  public ICommand? OnDropBeginMoveFrom { get; set; }
  public IAsyncRelayCommand? OnDropBeginMoveTo { get; set; }
  public ICommand? OnDropExecuteMove { get; set; }

  protected override void OnDragOver(DragEventArgs e) => DragAndDrop.DragOver(e);

  protected override void OnDrop(DragEventArgs e) => DragAndDrop.Drop(e);
}
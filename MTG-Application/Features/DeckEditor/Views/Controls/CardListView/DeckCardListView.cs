using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Views.Controls.CardListView;

public partial class DeckCardListView : ListView
{
  public DeckCardListView()
  {
    DragAndDrop = new()
    {
      OnCopy = async (item) => await (OnDropCopy?.ExecuteAsync(GetDropItem(item)) ?? Task.CompletedTask),
      OnExternalImport = async (data) => await (OnDropImport?.ExecuteAsync(data) ?? Task.CompletedTask),
      OnBeginMoveTo = async (item) => await (OnDropBeginMoveTo?.ExecuteAsync(GetDropItem(item)) ?? Task.CompletedTask),
      OnBeginMoveFrom = (item) => OnDropBeginMoveFrom?.Execute(GetDropItem(item)),
      OnExecuteMove = (item) => OnDropExecuteMove?.Execute(GetDropItem(item))
    };

    DragItemsStarting += DragAndDrop.DragStarting;

    var deleteAccelerator = new KeyboardAccelerator() { Key = Windows.System.VirtualKey.Delete };
    deleteAccelerator.Invoked += DeleteAccelerator_Invoked;
    KeyboardAccelerators.Add(deleteAccelerator);
  }

  private ListViewDragAndDrop<MTGCard> DragAndDrop { get; }

  public IAsyncRelayCommand<DeckEditorMTGCard>? OnDropCopy { get; set; }
  public IAsyncRelayCommand<string>? OnDropImport { get; set; }
  public IRelayCommand<DeckEditorMTGCard>? OnDropBeginMoveFrom { get; set; }
  public IAsyncRelayCommand<DeckEditorMTGCard>? OnDropBeginMoveTo { get; set; }
  public IRelayCommand? OnDropExecuteMove { get; set; }
  public IRelayCommand<DeckEditorMTGCard>? OnDeleteAcceleratorInvoked { get; set; }

  protected override void OnDragOver(DragEventArgs e) => DragAndDrop.DragOver(e);

  protected override void OnDrop(DragEventArgs e) => DragAndDrop.Drop(e);

  private void DeleteAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
  {
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

  protected static DeckEditorMTGCard GetDropItem(MTGCard card)
  {
    if (card is DeckEditorMTGCard editorCard)
      return new(editorCard.Info) { Count = editorCard.Count };
    else
      return new(card.Info);
  }
}
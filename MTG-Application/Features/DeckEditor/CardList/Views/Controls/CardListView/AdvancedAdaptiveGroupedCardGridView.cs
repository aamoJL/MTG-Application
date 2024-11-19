using Microsoft.UI.Xaml;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Views.DragAndDrop;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardListView;

public partial class AdvancedAdaptiveGroupedCardGridView : AdvancedAdaptiveCardGridView
{
  protected override void OnDragOver(DragEventArgs e)
  {
    base.OnDragOver(e);

    // Only accept Move if the card was moved from the same list, but from a different group 
    if (e.AcceptedOperation != Windows.ApplicationModel.DataTransfer.DataPackageOperation.None
      && ((DataContext as CardListViewModel)?.Cards.Contains(ListViewDragAndDrop<DeckEditorMTGCard>.Item.Card as DeckEditorMTGCard) is true)
      && !Items.Contains(ListViewDragAndDrop<DeckEditorMTGCard>.Item.Card as DeckEditorMTGCard))
    {
      e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
      e.DragUIOverride.Caption = "Change";
    }
  }
}
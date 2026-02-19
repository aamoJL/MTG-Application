using Microsoft.UI.Xaml;

namespace MTGApplication.Features.DeckEditor.Views.Controls.CardListView;

public partial class DeckEditorGroupedListViewContainer : DeckEditorListViewContainer
{
  protected override void OnDragOver(object sender, DragEventArgs e)
  {
    base.OnDragOver(sender, e);

    //e.Handled = true;

    //if (DataContext is not DeckCardGroupViewModel viewmodel
    //  || DragAndDrop<CardMoveArgs>.Item?.Card is not DeckEditorMTGCard item
    //  || e.AcceptedOperation == DataPackageOperation.None)
    //  return;

    //// Accept Move only if the card was moved from the same source, but from a different group 
    //if (viewmodel.SourceContains(item) && (ItemsSource as IList)?.Contains(item) is not true)
    //{
    //  e.AcceptedOperation = DataPackageOperation.Move;
    //  e.DragUIOverride.Caption = "Change";
    //}
  }
}
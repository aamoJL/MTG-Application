using Microsoft.UI.Xaml;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Views.DragAndDrop;
using System.Collections;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardListView;

public partial class AdvancedGroupedCardItemsView : AdvancedCardItemsView
{
  protected override void OnDragOver(DragEventArgs e)
  {
    base.OnDragOver(e);

    if (DataContext is not CardListViewModel viewmodel
      || ListViewDragAndDrop<DeckEditorMTGCard>.Item?.Card is not DeckEditorMTGCard item
      || e.AcceptedOperation == DataPackageOperation.None)
      return;

    // Only accept Move if the card was moved from the same list, but from a different group 
    if (viewmodel.Cards.Contains(item)
      && (ItemsSource as IList)?.Contains(item) is not true)
    {
      e.AcceptedOperation = DataPackageOperation.Move;
      e.DragUIOverride.Caption = "Change";
    }
  }
}
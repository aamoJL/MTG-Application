using Microsoft.UI.Xaml;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;
using MTGApplication.General.Views.DragAndDrop;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Features.DeckEditor.Views.Controls.CardListView;

public partial class GroupedDeckCardListItemsRepeater : DeckCardListItemsRepeater
{
  protected override void SetDragEventArgs(DragEventArgs e)
  {
    base.SetDragEventArgs(e);

    if (e.AcceptedOperation == DataPackageOperation.None)
      return;

    if ((e.AcceptedOperation & DataPackageOperation.Move) == DataPackageOperation.Move
      && e.DataView.Properties.TryGetValue(nameof(CardDragArgs), out var prop) && prop is CardDragArgs args
      && args.Item is DeckEditorMTGCard item
      && DataContext is DeckCardGroupViewModel groupVM)
    {
      // Change drop operation to "Change" if the card is from the same source
      if (groupVM.SourceContains(item))
      {
        e.AcceptedOperation = DataPackageOperation.Move;
        e.DragUIOverride.Caption = "Change";
      }
    }
  }
}

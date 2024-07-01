using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Features.DeckTesting.Services;

public class DeckTestingDragAndDropEvents
{
  public void ListView_Drop(object sender, DragEventArgs e)
  {
    if (DragAndDrop<CardMoveArgs>.Item?.Card is MTGCard internalCard)
      ((sender as ListViewBase)?.ItemsSource as Collection<DeckTestingMTGCard>)
        ?.Add(new(internalCard.Info));
  }

  public void ListView_DragOver(object _, DragEventArgs e)
  {
    // Block dropping if the item is invalid
    if (!e.DataView.Contains(StandardDataFormats.Text) && DragAndDrop<CardMoveArgs>.Item == null)
      return;

    e.AcceptedOperation = DataPackageOperation.Copy;
  }
}

using Microsoft.UI.Xaml;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;
using System;
using System.Collections;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardListView;

[Obsolete("ItemsView is very unstable on version: 1.5.250108004. Use ListViewContainers instead.")]
public partial class AdvancedGroupedCardItemsView : AdvancedCardItemsView
{
  protected override void OnDragOver(DragEventArgs e)
  {
    base.OnDragOver(e);

    if (DataContext is not CardGroupViewModel viewmodel
      || DragAndDrop<CardMoveArgs>.Item?.Card is not DeckEditorMTGCard item
      || e.AcceptedOperation == DataPackageOperation.None)
      return;

    // Only accept Move if the card was moved from the same list, but from a different group 
    if (viewmodel.Source.Contains(item)
      && (ItemsSource as IList)?.Contains(item) is not true)
    {
      e.AcceptedOperation = DataPackageOperation.Move;
      e.DragUIOverride.Caption = "Change";
    }
  }
}
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.CardDeck;
using MTGApplication.General.Models.Card;
using System;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.General.Views;

public class CardDragAndDrop
{
  public static CardListViewModel OriginList { get; set; }

  private bool IsDragging { get; set; } = false;

  public Action<MTGCard> OnAdd { get; set; }
  public Action<CardListViewModel.MoveArgs> OnMove { get; set; }
  public Action<string> OnImport { get; set; }

  public void DragStarting(object sender, DragItemsStartingEventArgs e)
  {
    if (e.Items[0] is MTGCard card)
    {
      e.Data.SetText(CardToJSON(card));
      e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;

      IsDragging = true;
      OriginList = (sender as FrameworkElement)?.DataContext as CardListViewModel;
    }
  }

  public void DragOver(object sender, DragEventArgs e)
  {
    if (IsDragging || !e.DataView.Contains(StandardDataFormats.Text))
      return;

    // Change operation to 'Move' if the shift key is down
    e.AcceptedOperation =
      (e.Modifiers & Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift)
      == Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift
      ? DataPackageOperation.Move : DataPackageOperation.Copy;
  }

  public async void Drop(object sender, DragEventArgs e)
  {
    if (IsDragging) return; // don't drop on the origin

    var def = e.GetDeferral();
    var data = await e.DataView.GetTextAsync();
    var operation = e.AcceptedOperation;

    if (!string.IsNullOrEmpty(data) && ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy
        || (operation & DataPackageOperation.Move) == DataPackageOperation.Move))
    {
      if (TryParseCardJSON(data) is MTGCard card)
      {
        if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
        {
          OnAdd?.Invoke(card);
        }
        else if ((operation & DataPackageOperation.Move) == DataPackageOperation.Move)
        {
          if (OriginList == null) OnAdd?.Invoke(card);
          else OnMove?.Invoke(new(card, OriginList));
        }
      }
      else
      {
        OnImport?.Invoke(data);
      }
    }

    def.Complete();
  }

  public void DragCompleted(ListViewBase sender, DragItemsCompletedEventArgs e)
  {
    IsDragging = false;
    OriginList = null;
  }

  private string CardToJSON(MTGCard card)
  {
    return JsonSerializer.Serialize(new
    {
      card.Info,
      card.Count
    });
  }

  private MTGCard TryParseCardJSON(string json)
  {
    try { return JsonSerializer.Deserialize<MTGCard>(json); }
    catch { return null; }
  }
}

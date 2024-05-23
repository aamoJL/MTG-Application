using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Views;
using System.Linq;

namespace MTGApplication.Features.CardSearch;
public sealed partial class CardSearchPage : Page
{
  public CardSearchPage()
  {
    InitializeComponent();

    ViewModel.Confirmers.ShowCardPrintsConfirmer.OnConfirm = async (msg)
      => (await new DraggableMTGCardGridViewDialog($"{msg.Title} (Prints can be dragged)", "MTGPrintGridViewItemTemplate", "MTGAdaptiveGridViewStyle")
      {
        Items = msg.Data.ToArray(),
        SecondaryButtonText = string.Empty,
        PrimaryButtonText = string.Empty,
        CloseButtonText = "Close"
      }.ShowAsync(new(XamlRoot))) as MTGCard;
  }

  public CardSearchViewModel ViewModel { get; } = new(App.MTGCardAPI);
  public ListViewDragAndDrop CardDragAndDrop { get; } = new(new MTGCardCopier()) { AcceptMove = false };
}
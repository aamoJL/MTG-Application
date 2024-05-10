using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Views;

namespace MTGApplication.Features.CardSearch;
public sealed partial class CardSearchPage : Page
{
  public CardSearchPage() => InitializeComponent();

  public CardSearchViewModel ViewModel { get; } = new(App.MTGCardAPI);
  public ListViewDragAndDrop<MTGCard> CardDragAndDrop { get; } = new() { AcceptMove = false };
}
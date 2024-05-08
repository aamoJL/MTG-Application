using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Views;

namespace MTGApplication.Features.CardSearch;
public sealed partial class CardSearchPage : Page
{
  public CardSearchPage() => InitializeComponent();

  public CardSearchViewModel ViewModel { get; } = new(App.MTGCardAPI);
  public CardDragAndDrop CardDragAndDrop { get; } = new();
}
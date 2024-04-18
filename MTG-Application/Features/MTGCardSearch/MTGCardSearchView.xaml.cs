using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.Features.CardSearch;
public sealed partial class MTGCardSearchView : Page
{
  public MTGCardSearchView() => InitializeComponent();

  public MTGCardSearchViewModel ViewModel { get; } = new(App.MTGCardAPI);
}

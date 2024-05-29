using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.Features.CardCollection;
public sealed partial class CardCollectionPage : Page
{
  public CardCollectionPage() => InitializeComponent();

  public CardCollectionViewModel ViewModel { get; } = new(App.MTGCardAPI);
}

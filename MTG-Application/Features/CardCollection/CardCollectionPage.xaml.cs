using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.Features.CardCollection;
public sealed partial class CardCollectionPage : Page
{
  public CardCollectionPage()
  {
    InitializeComponent();
    
    CardCollectionViewDialogs.RegisterConfirmDialogs(ViewModel.Confirmers, () => new(XamlRoot));
  }

  public CardCollectionViewModel ViewModel { get; } = new(App.MTGCardAPI);
}

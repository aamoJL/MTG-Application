using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace MTGApplication.Features.CardDeck;
public sealed partial class MTGDeckSelectorView : Page
{
  public MTGDeckSelectorView()
  {
    InitializeComponent();

    Loaded += MTGDeckSelectorView_Loaded;
  }

  public MTGDeckSelectorViewModel ViewModel { get; } = new();

  public ICommand DeckSelected { get; set; }

  private async void MTGDeckSelectorView_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
  {
    Loaded -= MTGDeckSelectorView_Loaded;

    await ViewModel.LoadDecksCommand.ExecuteAsync(null);
  }

  [RelayCommand]
  public void SelectDeck(MTGDeckSelectionListItem item)
    => DeckSelected.Execute(item?.Title ?? string.Empty);
}

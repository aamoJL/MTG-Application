using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using System.Windows.Input;

namespace MTGApplication.Features.CardDeck;
public sealed partial class DeckSelectorPage : Page
{
  public DeckSelectorPage()
  {
    InitializeComponent();

    Loaded += MTGDeckSelectorView_Loaded;
  }

  public DeckSelectorViewModel ViewModel { get; } = new(new DeckDTORepository(new()), App.MTGCardAPI);

  public ICommand DeckSelectedCommand
  {
    get => (ICommand)GetValue(DeckSelectedCommandProperty);
    set => SetValue(DeckSelectedCommandProperty, value);
  }

  [RelayCommand]
  public void SelectDeck(DeckSelectionListItem item) => DeckSelectedCommand?.Execute(item?.Title ?? string.Empty);

  private async void MTGDeckSelectorView_Loaded(object sender, RoutedEventArgs e)
  {
    Loaded -= MTGDeckSelectorView_Loaded;

    if (ViewModel.LoadDecksCommand.CanExecute(null)) await ViewModel.LoadDecksCommand.ExecuteAsync(null);
  }

  public static readonly DependencyProperty DeckSelectedCommandProperty =
      DependencyProperty.Register(nameof(DeckSelectedCommand), typeof(ICommand), typeof(DeckSelectorPage), new PropertyMetadata(null));
}

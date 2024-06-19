using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.Features.DeckSelection.ViewModels;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository;
using System.Windows.Input;

namespace MTGApplication.Features.DeckSelection.Views;
public sealed partial class DeckSelectionPage : Page
{
  public static readonly DependencyProperty DeckSelectedCommandProperty =
      DependencyProperty.Register(nameof(DeckSelectedCommand), typeof(ICommand), typeof(DeckSelectionPage), new PropertyMetadata(null));

  public DeckSelectionPage()
  {
    InitializeComponent();

    Loaded += MTGDeckSelectorView_Loaded;
  }

  public DeckSelectionViewModel ViewModel { get; } = new(new DeckDTORepository(new()), App.MTGCardImporter);

  public ICommand DeckSelectedCommand
  {
    get => (ICommand)GetValue(DeckSelectedCommandProperty);
    set => SetValue(DeckSelectedCommandProperty, value);
  }

  [RelayCommand]
  public void SelectDeck(DeckSelectionDeck item) => DeckSelectedCommand?.Execute(item?.Title ?? string.Empty);

  private async void MTGDeckSelectorView_Loaded(object sender, RoutedEventArgs e)
  {
    Loaded -= MTGDeckSelectorView_Loaded;

    if (ViewModel.LoadDecksCommand.CanExecute(null))
      await ViewModel.LoadDecksCommand.ExecuteAsync(null);
  }
}

using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.Features.DeckSelection.ViewModels;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository;
using MTGApplication.General.Services.NotificationService;
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

  private void MTGDeckSelectorView_Loaded(object sender, RoutedEventArgs e)
  {
    Loaded -= MTGDeckSelectorView_Loaded;

    NotificationService.RegisterNotifications(ViewModel.Notifier, this);

    // Workaround to notify user if the deck fetching sends a notification before the notifier has been registered to the view.
    var deckUpdateTask = ViewModel.WaitForDeckUpdate();

    if (deckUpdateTask.IsFaulted)
      ViewModel.Notifier.Notify(new(NotificationService.NotificationType.Error, $"Error: {deckUpdateTask.Exception.Message}"));
  }
}

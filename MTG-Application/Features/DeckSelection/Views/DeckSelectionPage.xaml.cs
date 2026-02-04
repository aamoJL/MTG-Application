using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckSelection.ViewModels;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository;
using MTGApplication.General.Services.NotificationService;

namespace MTGApplication.Features.DeckSelection.Views;

public sealed partial class DeckSelectionPage : Page
{
  public DeckSelectionPage()
  {
    InitializeComponent();

    Loaded += MTGDeckSelectorView_Loaded;
  }

  public DeckSelectionPageViewModel ViewModel { get; set; } = new(new DeckDTORepository(new()), App.MTGCardImporter);

  private void MTGDeckSelectorView_Loaded(object sender, RoutedEventArgs e)
  {
    Loaded -= MTGDeckSelectorView_Loaded;

    NotificationService.RegisterNotifications(ViewModel.Notifier, this);

    if (ViewModel.RefreshDecksCommand.CanExecute(null))
      _ = ViewModel.RefreshDecksCommand.ExecuteAsync(null);
  }
}

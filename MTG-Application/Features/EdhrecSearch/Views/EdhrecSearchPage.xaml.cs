using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.Features.EdhrecSearch.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;
using static MTGApplication.General.Services.Importers.CardImporter.EdhrecImporter;

namespace MTGApplication.Features.EdhrecSearch.Views;
public sealed partial class EdhrecSearchPage : Page
{
  public EdhrecSearchPage() => InitializeComponent();

  public EdhrecSearchPageViewModel ViewModel { get; } = new(App.MTGCardImporter);
  public ListViewDragAndDrop<MTGCard> CardDragAndDrop { get; } = new(itemToArgsConverter: (item) => { return new CardMoveArgs(item); }) { AcceptMove = false };

  protected override void OnNavigatedTo(NavigationEventArgs e)
  {
    base.OnNavigatedTo(e);

    if (e.Parameter is CommanderTheme[] themes)
      ViewModel.CommanderThemes = themes;
  }
}

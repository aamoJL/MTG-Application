using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.EdhrecSearch.ViewModels;
using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;

namespace MTGApplication.Features.EdhrecSearch.Views;
public sealed partial class EdhrecSearchPage : Page
{
  public EdhrecSearchPage() => InitializeComponent();

  public EdhrecSearchPageViewModel ViewModel { get; } = new(App.MTGCardImporter);
  public ListViewDragAndDrop<MTGCard> CardDragAndDrop { get; } = new(itemToArgsConverter: (item) => { return new CardMoveArgs(item); }) { AcceptMove = false };
}

using Microsoft.UI.Xaml.Controls;
using MTGApplication.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Views.Pages.Tabs;
public sealed partial class DeckBuilderDeckDisplayTabView : Page
{
  public DeckBuilderDeckDisplayTabView() => InitializeComponent();

  public DeckBuilderDeckDisplayViewModel ViewModel { get; } = new();

  public async Task Init() => await ViewModel.LoadDeckItems();
}

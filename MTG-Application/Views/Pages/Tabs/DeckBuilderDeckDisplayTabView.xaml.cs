using Microsoft.UI.Xaml.Controls;
using MTGApplication.Interfaces;
using MTGApplication.ViewModels;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MTGApplication.Views.Pages.Tabs;
public sealed partial class DeckBuilderDeckDisplayTabView : Page, ITabViewTab
{
  public DeckBuilderDeckDisplayTabView()
  {
    InitializeComponent();
  }

  public string Header => "Select deck";
  public DeckBuilderDeckDisplayViewModel ViewModel { get; } = new();

  public event PropertyChangedEventHandler PropertyChanged;

  public Task<bool> TabCloseRequested() => Task.FromResult(true);

  public async Task Init() => await ViewModel.Init();
}

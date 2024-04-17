using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MTGApplication.Interfaces;
using System.ComponentModel;
using System.Threading.Tasks;
using static MTGApplication.Views.Controls.MTGCardPreviewControl;

namespace MTGApplication.Views.Pages.Tabs;
public sealed partial class DeckBuilderTabFrame : Page, ITabViewTab
{
  public DeckBuilderTabFrame(CardPreviewProperties previewProperties)
  {
    InitializeComponent();
    PreviewProperties = previewProperties;
  }

  private string header = "Select deck";
  public string Header
  {
    get => header;
    set
    {
      header = value;
      PropertyChanged?.Invoke(this, new(nameof(Header)));
    }
  }

  public CardPreviewProperties PreviewProperties { get; }

  public event PropertyChangedEventHandler PropertyChanged;

  public Task<bool> TabCloseRequested() => Task.FromResult(true);

  public void Init()
  {
    if (BaseFrame.Navigate(typeof(DeckBuilderDeckDisplayTabView), null, new SuppressNavigationTransitionInfo()))
    {
      var deckDisplayContent = (BaseFrame.Content as DeckBuilderDeckDisplayTabView);
      _ = deckDisplayContent.Init();

      Header = deckDisplayContent.Header;

      deckDisplayContent.ViewModel.DeckSelected += DeckDisplayViewModel_DeckSelected;
    }
  }

  private async void DeckDisplayViewModel_DeckSelected(object sender, ViewModels.DeckBuilderDeckDisplayViewModel.DeckSelectedEventArgs e)
  {
    (BaseFrame.Content as DeckBuilderDeckDisplayTabView).ViewModel.DeckSelected -= DeckDisplayViewModel_DeckSelected;

    if (BaseFrame.Navigate(typeof(DeckBuilderTabView), null, new SuppressNavigationTransitionInfo()))
    {
      var deckBuilderContent = (BaseFrame.Content as DeckBuilderTabView);
      deckBuilderContent.CardPreviewProperties = PreviewProperties;
      if (!string.IsNullOrEmpty(e.Name))
      {
        await deckBuilderContent.DeckBuilderViewModel.LoadDeck(e.Name);
      }

      Header = deckBuilderContent.Header;

      deckBuilderContent.PropertyChanged += DeckBuilderContent_PropertyChanged;
    }
  }

  private void DeckBuilderContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(DeckBuilderTabView.Header))
      Header = (sender as DeckBuilderTabView).Header;
  }
}

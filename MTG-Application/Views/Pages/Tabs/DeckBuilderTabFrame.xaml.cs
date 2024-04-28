using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MTGApplication.Features.CardDeck;
using MTGApplication.General.Views;
using MTGApplication.Interfaces;
using MTGApplication.ViewModels;
using System.ComponentModel;
using System.Threading.Tasks;
using static MTGApplication.Views.Controls.MTGCardPreviewControl;

namespace MTGApplication.Views.Pages.Tabs;
public sealed partial class DeckBuilderTabFrame : Page, ITabViewTab, ISavable
{
  public DeckBuilderTabFrame(CardPreviewProperties previewProperties)
  {
    InitializeComponent();
    PreviewProperties = previewProperties;
  }

  private string header = "Deck selection";
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

  public bool HasUnsavedChanges
  {
    get => (BaseFrame.Content as ISavable)?.HasUnsavedChanges ?? false;
    set { }
  }

  public event PropertyChangedEventHandler PropertyChanged;

  public async Task<bool> TabCloseRequested()
  {
    if (BaseFrame.Content is ISavable savable)
      return !savable.HasUnsavedChanges || await savable.SaveUnsavedChanges();
    else
      return await Task.FromResult(true);
  }

  /// <summary>
  /// Initializer for the frame.
  /// The frame will navigate to <see cref="DeckSelectorPage"/>
  /// </summary>
  /// <returns></returns>
  public DeckBuilderTabFrame Init()
  {
    if (BaseFrame.Navigate(typeof(DeckSelectorPage), null, new SuppressNavigationTransitionInfo()))
    {
      var deckDisplayContent = (BaseFrame.Content as DeckSelectorPage);
      //_ = deckDisplayContent.Init();
      //TODO: init

      //deckDisplayContent.ViewModel.DeckSelected += DeckDisplayViewModel_DeckSelected;
    }

    return this;
  }

  private async void DeckDisplayViewModel_DeckSelected(object sender, DeckSelectorViewModel.DeckSelectedEventArgs e)
  {
    //(sender as MTGDeckSelectorViewModel).DeckSelected -= DeckDisplayViewModel_DeckSelected;

    if (BaseFrame.Navigate(typeof(DeckBuilderTabView), null, new SuppressNavigationTransitionInfo()))
    {
      var deckBuilderContent = BaseFrame.Content as DeckBuilderTabView;
      deckBuilderContent.CardPreviewProperties = PreviewProperties;

      if (!string.IsNullOrEmpty(e.Name))
      {
        await deckBuilderContent.DeckBuilderViewModel.LoadDeck(e.Name);
      }

      Header = deckBuilderContent.DeckBuilderViewModel.DeckName;

      deckBuilderContent.DeckBuilderViewModel.PropertyChanged += (s, e) =>
      {
        if (e.PropertyName == nameof(DeckBuilderViewModel.DeckName))
          Header = (sender as DeckBuilderViewModel).DeckName;
      };
    }
  }

  public async Task<bool> SaveUnsavedChanges() => await (BaseFrame.Content as ISavable)?.SaveUnsavedChanges();
}

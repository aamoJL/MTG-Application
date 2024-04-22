using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MTGApplication.Features.CardDeck;

namespace MTGApplication.General.Views;
/// <summary>
/// Use case to create new tab item with <see cref=""/> as the content
/// </summary>
public class CreateNewDeckViewTabItemUseCase : UseCase<CustomTabViewItem>
{
  public override CustomTabViewItem Execute()
  {
    var tabFrame = new Frame();

    tabFrame.Content = new MTGDeckSelectorView()
    {
      DeckSelected = new RelayCommand<string>((string selectedDeck) =>
      {
        tabFrame.Navigate(typeof(MTGDeckEditorView), selectedDeck ?? "", new SuppressNavigationTransitionInfo());
      }),
    };

    return new CustomTabViewItem()
    {
      Frame = tabFrame,
    };
  }
}

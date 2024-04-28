using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MTGApplication.Features.CardDeck;
using MTGApplication.General.UseCases;

namespace MTGApplication.General.Views;
/// <summary>
/// Use case to create new tab item with <see cref=""/> as the content
/// </summary>
public class CreateNewDeckViewTabItem : UseCase<CustomTabViewItem>
{
  public override CustomTabViewItem Execute()
  {
    var tabFrame = new Frame();
    tabFrame.Content = new DeckSelectorPage()
    {
      DeckSelectedCommand = new RelayCommand<string>((string selectedDeck) =>
      {
        tabFrame.Navigate(typeof(DeckEditorView), selectedDeck ?? "", new SuppressNavigationTransitionInfo());
      }),
    };

    return new CustomTabViewItem()
    {
      Frame = tabFrame,
    };
  }
}

using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MTGApplication.Features.DeckEditor;
using MTGApplication.Features.DeckSelector;
using MTGApplication.General.ViewModels;
using MTGApplication.General.Views.Controls;

namespace MTGApplication.General.Views.AppWindows;
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
        tabFrame.Navigate(typeof(DeckEditorPage), selectedDeck ?? "", new SuppressNavigationTransitionInfo());
      }),
    };

    return new CustomTabViewItem()
    {
      Frame = tabFrame,
    };
  }
}

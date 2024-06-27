using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public class OpenDeckTestingWindow(DeckEditorViewModel viewmodel) : ViewModelCommand<DeckEditorViewModel>(viewmodel)
{
  protected override bool CanExecute() => Viewmodel.DeckCardList.Cards.Any();

  protected override void Execute()
  {
    var testingDeck = new DeckTestingDeck(
      DeckCards: Viewmodel.DeckCardList.Cards.SelectMany(c => Enumerable.Range(1, c.Count).Select(i => c as MTGCard)).ToList(),
      Commander: Viewmodel.Commander,
      Partner: Viewmodel.Partner);

    new AppWindows.DeckTestingWindow.DeckTestingWindow(testingDeck).Activate();
  }
}
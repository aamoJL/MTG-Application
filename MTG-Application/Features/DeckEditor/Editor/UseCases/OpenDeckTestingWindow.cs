using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public partial class DeckEditorViewModelCommands
{
  public class OpenDeckTestingWindow(DeckEditorViewModel viewmodel) : SyncCommand
  {
    public DeckEditorViewModel Viewmodel { get; } = viewmodel;

    protected override bool CanExecute() => Viewmodel.DeckCardList!.Cards.Any();

    protected override void Execute()
    {
      var testingDeck = new DeckTestingDeck(
        DeckCards: [.. Viewmodel.DeckCardList.Cards.SelectMany(c => Enumerable.Range(1, c.Count).Select(i => c as MTGCard))],
        Commander: Viewmodel.Commander.Card,
        Partner: Viewmodel.Partner.Card);

      new AppWindows.DeckTestingWindow.DeckTestingWindow(testingDeck).Activate();
    }
  }
}
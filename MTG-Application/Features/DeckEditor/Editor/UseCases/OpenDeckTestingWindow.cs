using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public class OpenDeckTestingWindow(DeckEditorViewModel viewmodel) : ViewModelCommand<DeckEditorViewModel>(viewmodel)
{
  protected override bool CanExecute() => Viewmodel.DeckCardList.Cards.Any();

  protected override void Execute()
  {
    new AppWindows.DeckTestingWindow.DeckTestingWindow().Activate();
  }
}
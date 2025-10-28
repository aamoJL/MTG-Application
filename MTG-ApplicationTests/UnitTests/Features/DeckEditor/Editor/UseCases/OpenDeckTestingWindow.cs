using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.ViewModels;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.UseCases;

[TestClass]
public class OpenDeckTestingWindow : DeckEditorViewModelTestBase, ICanExecuteCommandTests
{
  [TestMethod(DisplayName = "Should be able to execute if the deck has cards")]
  public void ValidState_CanExecute()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
    }.MockVM();

    Assert.IsTrue(viewmodel.OpenDeckTestingWindowCommand.CanExecute(null));
  }

  [TestMethod(DisplayName = "Should not be able to execute if the deck is empty")]
  public void InvalidState_CanNotExecute()
  {
    var viewmodel = new Mocker(_dependencies).MockVM();

    Assert.IsFalse(viewmodel.OpenDeckTestingWindowCommand.CanExecute(null));
  }
}

using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.ViewModels;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.UseCases;

[TestClass]
public class OpenEdhrecSearchWindow : DeckEditorViewModelTestBase, ICanExecuteCommandTests
{
  [TestMethod(DisplayName = "Should be able to execute if the deck has a commander")]
  public void ValidState_CanExecute()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = new() { Commander = DeckEditorMTGCardMocker.CreateMTGCardModel() },
    }.MockVM();

    Assert.IsTrue(viewmodel.OpenEdhrecSearchWindowCommand.CanExecute(null));
  }

  [TestMethod(DisplayName = "Should not be able to execute if the deck does not have a commander")]
  public void InvalidState_CanNotExecute()
  {
    var viewmodel = new Mocker(_dependencies).MockVM();

    Assert.IsFalse(viewmodel.OpenEdhrecSearchWindowCommand.CanExecute(null));
  }
}

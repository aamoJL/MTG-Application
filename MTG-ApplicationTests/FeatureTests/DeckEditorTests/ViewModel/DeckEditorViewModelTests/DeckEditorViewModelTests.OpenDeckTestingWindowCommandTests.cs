using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class OpenDeckTestingWindowCommandTests : DeckEditorViewModelTestsBase, ICanExecuteCommandTests
  {
    [TestMethod("Should be able to execute if the deck has cards")]
    public void ValidState_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
      }.MockVM();

      Assert.IsTrue(viewmodel.OpenDeckTestingWindowCommand.CanExecute(null));
    }

    [TestMethod("Should not be able to execute if the deck is empty")]
    public void InvalidState_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM();

      Assert.IsFalse(viewmodel.OpenDeckTestingWindowCommand.CanExecute(null));
    }
  }
}

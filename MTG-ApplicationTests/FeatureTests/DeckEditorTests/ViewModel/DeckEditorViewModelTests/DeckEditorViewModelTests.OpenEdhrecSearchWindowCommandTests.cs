using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class OpenEdhrecSearchWindowCommandTests : DeckEditorViewModelTestsBase, ICanExecuteCommandTests
  {
    [TestMethod("Should be able to execute if the deck has a commander")]
    public void ValidState_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = new() { Commander = DeckEditorMTGCardMocker.CreateMTGCardModel() },
      }.MockVM();

      Assert.IsTrue(viewmodel.OpenEdhrecSearchWindowCommand.CanExecute(null));
    }

    [TestMethod("Should not be able to execute if the deck does not have a commander")]
    public void InvalidState_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM();

      Assert.IsFalse(viewmodel.OpenEdhrecSearchWindowCommand.CanExecute(null));
    }
  }
}

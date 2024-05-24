using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class ShowTokensTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public void ShowTokens_HasOnlyCommander_CanExecute()
    {
      var viewmodel = MockVM(deck: new MTGCardDeck()
      {
        Commander = MTGCardModelMocker.CreateMTGCardModel()
      });

      Assert.IsTrue(viewmodel.ShowDeckTokensCommand.CanExecute(null));
    }

    [TestMethod]
    public void ShowTokens_HasNoCards_CanNotExecute()
    {
      var viewmodel = MockVM();

      Assert.IsFalse(viewmodel.ShowDeckTokensCommand.CanExecute(null));
    }

    [TestMethod]
    public void ShowTokens_HasCards_CanExecute()
    {
      var viewmodel = MockVM(deck: new MTGCardDeck()
      {
        DeckCards = [MTGCardModelMocker.CreateMTGCardModel()]
      });

      Assert.IsTrue(viewmodel.ShowDeckTokensCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task ShowTokens_ConfirmationShown()
    {
      var viewmodel = MockVM(deck: _savedDeck, confirmers: new()
      {
        ShowTokensConfirmer = new TestExceptionConfirmer<MTGCard, IEnumerable<MTGCard>>(),
      });

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ShowDeckTokensCommand.ExecuteAsync(null));
    }
  }
}

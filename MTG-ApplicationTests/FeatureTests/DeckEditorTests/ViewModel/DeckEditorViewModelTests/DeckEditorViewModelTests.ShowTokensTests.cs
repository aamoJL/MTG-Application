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
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = new MTGCardDeck()
        {
          Commander = MTGCardModelMocker.CreateMTGCardModel()
        },
      }.MockVM();

      Assert.IsTrue(viewmodel.ShowDeckTokensCommand.CanExecute(null));
    }

    [TestMethod]
    public void ShowTokens_HasNoCards_CanNotExecute()
    {

      var viewmodel = new Mocker(_dependencies).MockVM();

      Assert.IsFalse(viewmodel.ShowDeckTokensCommand.CanExecute(null));
    }

    [TestMethod]
    public void ShowTokens_HasCards_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = new MTGCardDeck()
        {
          DeckCards = [MTGCardModelMocker.CreateMTGCardModel()]
        },
      }.MockVM();

      Assert.IsTrue(viewmodel.ShowDeckTokensCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task ShowTokens_ConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        Confirmers = new()
        {
          ShowTokensConfirmer = new TestExceptionConfirmer<DeckEditorMTGCard, IEnumerable<DeckEditorMTGCard>>(),
        }
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ShowDeckTokensCommand.ExecuteAsync(null));
    }
  }
}

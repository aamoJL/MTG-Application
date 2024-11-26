using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Models;
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
        Deck = new DeckEditorMTGDeck()
        {
          Commander = DeckEditorMTGCardMocker.CreateMTGCardModel()
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
        Deck = new DeckEditorMTGDeck()
        {
          DeckCards = [DeckEditorMTGCardMocker.CreateMTGCardModel()]
        },
      }.MockVM();

      Assert.IsTrue(viewmodel.ShowDeckTokensCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task ShowTokens_ConfirmationShown()
    {
      var confirmer = new TestConfirmer<MTGCard, IEnumerable<MTGCard>>();
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        Confirmers = new()
        {
          ShowTokensConfirmer = confirmer,
        }
      }.MockVM();

      await viewmodel.ShowDeckTokensCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }
  }
}

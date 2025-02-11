using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class ChangeCardPrintCommandTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public async Task ChangePrint_ConfirmationShown()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var confirmer = new TestConfirmer<MTGCard, IEnumerable<MTGCard>>();
      _ = new Mocker(_dependencies)
      {
        Deck = new()
        {
          DeckCards = [card]
        },
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = confirmer
        }
      }.MockVM();

      await card.ChangePrintCommand.ExecuteAsync(card);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task ChangePrint_PrintChanged()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();

      var oldPrint = card.Info with { };
      var newPrint = card.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };

      var viewmodel = new Mocker(_dependencies)
      {
        Deck = new()
        {
          DeckCards = [card]
        },
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new()
          {
            OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint))
          }
        }
      }.MockVM();

      await card.ChangePrintCommand.ExecuteAsync(card);

      Assert.AreEqual(newPrint.SetCode, card.Info.SetCode);

      viewmodel.UndoStack.Undo();

      Assert.AreEqual(oldPrint.SetCode, card.Info.SetCode);

      viewmodel.UndoStack.Redo();

      Assert.AreEqual(newPrint.SetCode, card.Info.SetCode);
    }
  }
}


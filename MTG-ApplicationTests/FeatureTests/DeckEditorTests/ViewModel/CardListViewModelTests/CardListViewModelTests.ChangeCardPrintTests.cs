using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests;

public partial class CardListViewModelTests
{
  [TestClass]
  public class ChangeCardPrintTests
  {
    [TestMethod]
    public async Task ChangePrint_ConfirmationShown()
    {
      var confirmer = new TestConfirmer<MTGCard, IEnumerable<MTGCard>>();
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var viewmodel = new CardListViewModel(new TestMTGCardImporter(), new()
      {
        ChangeCardPrintConfirmer = confirmer
      })
      {
        Cards = [existingCard],
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task ChangePrint_PrintChanged()
    {
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CardListViewModel(new TestMTGCardImporter(), new()
      {
        ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint)) }
      })
      {
        Cards = [existingCard],
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);

      Assert.AreEqual(viewmodel.Cards.First().Info.SetCode, newPrint.SetCode);
    }

    [TestMethod]
    public async Task ChangePrint_CardDoesNotExist_NothingHappens()
    {
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(name: "abc", setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), Name = "zyx", SetCode = "zyx" };
      var viewmodel = new CardListViewModel(new TestMTGCardImporter(), new()
      {
        ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint)) }
      })
      {
        Cards = [existingCard],
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);

      Assert.AreEqual(viewmodel.Cards.First().Info.SetCode, existingCard.Info.SetCode);
      Assert.AreEqual(1, viewmodel.Cards.Count);
    }

    [TestMethod]
    public async Task ChangePrint_Undo_CardHasOldPrint()
    {
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CardListViewModel(new TestMTGCardImporter(), new()
      {
        ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint)) }
      })
      {
        Cards = [existingCard],
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);
      viewmodel.UndoStack.Undo();

      Assert.AreEqual(viewmodel.Cards.First().Info.SetCode, existingCard.Info.SetCode);
    }

    [TestMethod]
    public async Task ChangePrint_Redo_CardHasNewPrintAgain()
    {
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CardListViewModel(new TestMTGCardImporter(), new()
      {
        ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint)) }
      })
      {
        Cards = [existingCard],
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);
      viewmodel.UndoStack.Undo();
      viewmodel.UndoStack.Redo();

      Assert.AreEqual(viewmodel.Cards.First().Info.SetCode, newPrint.SetCode);
    }
  }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class ChangeCardPrintTests
  {
    [TestMethod]
    public void ChangePrint_HasCard_CanExecute()
    {
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter())
      {
        Card = DeckEditorMTGCardMocker.CreateMTGCardModel(),
      };

      Assert.IsTrue(viewmodel.ChangeCardPrintCommand.CanExecute(viewmodel.Card));
    }

    [TestMethod]
    public void ChangePrint_HasNoCard_CanExecute()
    {
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter());

      Assert.IsFalse(viewmodel.ChangeCardPrintCommand.CanExecute(viewmodel.Card));
    }

    [TestMethod]
    public async Task ChangePrint_ConfirmationShown()
    {
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter())
      {
        Card = existingCard,
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new TestExceptionConfirmer<MTGCard, IEnumerable<MTGCard>>()
        }
      };

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard));
    }

    [TestMethod]
    public async Task ChangePrint_PrintChanged()
    {
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter())
      {
        Card = existingCard,
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint)) }
        }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);

      Assert.AreEqual(viewmodel.Card.Info.SetCode, newPrint.SetCode);
    }

    [TestMethod]
    public async Task ChangePrint_OnCardPropertyChangeInvoked()
    {
      var invoked = false;
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter())
      {
        Card = existingCard,
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint)) }
        },
        OnChange = (card) => { invoked = true; }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);

      Assert.IsTrue(invoked);
    }

    [TestMethod]
    public async Task ChangePrint_CardDoesNotExist_NothingHappens()
    {
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(name: "abc", setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), Name = "zyx", SetCode = "zyx" };
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter())
      {
        Card = existingCard,
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint)) }
        }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);

      Assert.AreEqual(viewmodel.Card.Info.SetCode, existingCard.Info.SetCode);
    }

    [TestMethod]
    public async Task ChangePrint_Undo_CardHasOldPrint()
    {
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter())
      {
        Card = existingCard,
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint)) }
        }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);
      viewmodel.UndoStack.Undo();

      Assert.AreEqual(viewmodel.Card.Info.SetCode, existingCard.Info.SetCode);
    }

    [TestMethod]
    public async Task ChangePrint_Redo_CardHasNewPrintAgain()
    {
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter())
      {
        Card = existingCard,
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint)) }
        }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);
      viewmodel.UndoStack.Undo();
      viewmodel.UndoStack.Redo();

      Assert.AreEqual(viewmodel.Card.Info.SetCode, newPrint.SetCode);
    }
  }
}

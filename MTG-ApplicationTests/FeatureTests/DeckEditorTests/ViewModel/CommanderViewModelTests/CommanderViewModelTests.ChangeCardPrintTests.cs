using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.Features.DeckEditor.Editor.Models;
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
      var commands = new CommanderViewModel(new TestMTGCardImporter(), () => DeckEditorMTGCardMocker.CreateMTGCardModel());

      Assert.IsTrue(commands.ChangeCardPrintCommand.CanExecute(null));
    }

    [TestMethod]
    public void ChangePrint_HasNoCard_CanExecute()
    {
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter(), () => null);

      Assert.IsFalse(viewmodel.ChangeCardPrintCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task ChangePrint_ConfirmationShown()
    {
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter(), () => existingCard)
      {
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new TestExceptionConfirmer<MTGCard, IEnumerable<MTGCard>>()
        }
      };

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard));
    }

    [TestMethod]
    public async Task ChangePrint_InvokedWithPrint()
    {
      DeckEditorMTGCard? result = null;
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter(), () => existingCard)
      {
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint)) }
        },
        OnChange = (card) => { result = card; }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);

      Assert.AreEqual(newPrint.SetCode, result?.Info.SetCode);
    }

    [TestMethod]
    public async Task ChangePrint_Undo_InvokedWithOldPrint()
    {
      DeckEditorMTGCard? result = null;
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter(), () => existingCard)
      {
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint)) }
        },
        OnChange = (card) => { result = card; }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);
      viewmodel.UndoStack.Undo();

      Assert.AreEqual(result?.Info.SetCode, existingCard.Info.SetCode);
    }

    [TestMethod]
    public async Task ChangePrint_Redo_InvokedWithNewPrint()
    {
      DeckEditorMTGCard? result = null;
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter(), () => existingCard)
      {
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint)) }
        },
        OnChange = (card) => { result = card; }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);
      viewmodel.UndoStack.Undo();
      viewmodel.UndoStack.Redo();

      Assert.AreEqual(result?.Info.SetCode, newPrint.SetCode);
    }
  }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests.DeckEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderCommandsTests
{
  [TestClass]
  public class ChangeCardPrintTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public void ChangePrint_HasCard_CanExecute()
    {
      var commands = new CommanderCommands(new Mocker(_dependencies) { Deck = _savedDeck }.MockVM(), CommanderCommands.CommanderType.Commander);

      Assert.IsTrue(commands.ChangeCardPrintCommand.CanExecute(null));
    }

    [TestMethod]
    public void ChangePrint_HasNoCard_CanExecute()
    {
      var viewmodel = new CommanderCommands(new Mocker(_dependencies).MockVM(), CommanderCommands.CommanderType.Commander);

      Assert.IsFalse(viewmodel.ChangeCardPrintCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task ChangePrint_ConfirmationShown()
    {
      var confirmer = new TestConfirmer<MTGCard, IEnumerable<MTGCard>>();
      var existingCard = _savedDeck.Commander;
      var viewmodel = new CommanderCommands(new Mocker(_dependencies) { Deck = _savedDeck }.MockVM(), CommanderCommands.CommanderType.Commander)
      {
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = confirmer
        }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task ChangePrint_InvokedWithPrint()
    {
      DeckEditorMTGCard result = null;
      var existingCard = _savedDeck.Commander;
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CommanderCommands(new Mocker(_dependencies) { Deck = _savedDeck }.MockVM(), CommanderCommands.CommanderType.Commander)
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
      DeckEditorMTGCard result = null;
      var existingCard = _savedDeck.Commander;
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CommanderCommands(new Mocker(_dependencies) { Deck = _savedDeck }.MockVM(), CommanderCommands.CommanderType.Commander)
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
      DeckEditorMTGCard result = null;
      var existingCard = _savedDeck.Commander;
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CommanderCommands(new Mocker(_dependencies) { Deck = _savedDeck }.MockVM(), CommanderCommands.CommanderType.Commander)
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests.DeckEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class ChangeCardPrintTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public void ChangePrint_HasCard_CanExecute()
    {
      var commands = new CommanderViewModel(_dependencies.Importer)
      {
        Card = _savedDeck.Commander
      };

      Assert.IsTrue(commands.ChangeCardPrintCommand.CanExecute(null));
    }

    [TestMethod]
    public void ChangePrint_HasNoCard_CanExecute()
    {
      var viewmodel = new CommanderViewModel(_dependencies.Importer);

      Assert.IsFalse(viewmodel.ChangeCardPrintCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task ChangePrint_ConfirmationShown()
    {
      var confirmer = new TestConfirmer<MTGCard, IEnumerable<MTGCard>>();
      var existingCard = _savedDeck.Commander;
      var viewmodel = new CommanderViewModel(_dependencies.Importer)
      {
        Card = existingCard,
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = confirmer
        }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task ChangePrint_PrintChanged()
    {
      var existingCard = _savedDeck.Commander;
      var oldPrint = existingCard.Info with { };
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CommanderViewModel(_dependencies.Importer)
      {
        Card = existingCard,
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new()
          {
            OnConfirm = async (arg) => await Task.FromResult(new MTGCard(newPrint))
          }
        }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);

      Assert.AreEqual(newPrint.SetCode, viewmodel.Card.Info.SetCode);

      viewmodel.UndoStack.Undo();

      Assert.AreEqual(oldPrint.SetCode, viewmodel.Card.Info.SetCode);

      viewmodel.UndoStack.Redo();

      Assert.AreEqual(newPrint.SetCode, viewmodel.Card.Info.SetCode);
    }
  }
}

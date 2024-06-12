﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.API;
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
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var viewmodel = new CardListViewModel(new TestMTGCardImporter())
      {
        Cards = [existingCard],
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new TestExceptionConfirmer<MTGCardInfo, IEnumerable<MTGCardInfo>>()
        }
      };

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard));
    }

    [TestMethod]
    public async Task ChangePrint_PrintChanged()
    {
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), SetCode = "zyx" };
      var viewmodel = new CardListViewModel(new TestMTGCardImporter())
      {
        Cards = [existingCard],
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(newPrint) }
        }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);

      Assert.AreEqual(viewmodel.Cards.First().Info.SetCode, newPrint.SetCode);
    }

    [TestMethod]
    public async Task ChangePrint_CardDoesNotExist_NothingHappens()
    {
      var existingCard = DeckEditorMTGCardMocker.CreateMTGCardModel(name: "abc", setCode: "abc");
      var newPrint = existingCard.Info with { ScryfallId = Guid.NewGuid(), Name = "zyx", SetCode = "zyx" };
      var viewmodel = new CardListViewModel(new TestMTGCardImporter())
      {
        Cards = [existingCard],
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(newPrint) }
        }
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
      var viewmodel = new CardListViewModel(new TestMTGCardImporter())
      {
        Cards = [existingCard],
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(newPrint) }
        }
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
      var viewmodel = new CardListViewModel(new TestMTGCardImporter())
      {
        Cards = [existingCard],
        Confirmers = new()
        {
          ChangeCardPrintConfirmer = new() { OnConfirm = async (arg) => await Task.FromResult(newPrint) }
        }
      };

      await viewmodel.ChangeCardPrintCommand.ExecuteAsync(existingCard);
      viewmodel.UndoStack.Undo();
      viewmodel.UndoStack.Redo();

      Assert.AreEqual(viewmodel.Cards.First().Info.SetCode, newPrint.SetCode);
    }
  }
}

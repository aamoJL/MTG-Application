﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests;

public partial class CardListViewModelTests
{
  [TestClass]
  public class AddCardTests
  {
    [TestMethod]
    public void AddCard_CardAdded()
    {
      var viewmodel = new CardListViewModel(new TestMTGCardImporter());

      viewmodel.AddCardCommand.Execute(DeckEditorMTGCardMocker.CreateMTGCardModel());

      Assert.AreEqual(1, viewmodel.Cards.Count);
    }

    [TestMethod]
    public void AddCard_Undo_CardRemoved()
    {
      var viewmodel = new CardListViewModel(new TestMTGCardImporter());

      viewmodel.AddCardCommand.Execute(DeckEditorMTGCardMocker.CreateMTGCardModel());

      Assert.AreEqual(1, viewmodel.Cards.Count);

      viewmodel.UndoStack.Undo();

      Assert.AreEqual(0, viewmodel.Cards.Count);
    }

    [TestMethod]
    public void AddCard_Redo_CardAddedAgain()
    {
      var viewmodel = new CardListViewModel(new TestMTGCardImporter());

      viewmodel.AddCardCommand.Execute(DeckEditorMTGCardMocker.CreateMTGCardModel());

      Assert.AreEqual(1, viewmodel.Cards.Count);

      viewmodel.UndoStack.Undo();

      Assert.AreEqual(0, viewmodel.Cards.Count);

      viewmodel.UndoStack.Redo();

      Assert.AreEqual(1, viewmodel.Cards.Count);
    }

    [TestMethod]
    public async Task AddCard_Existing_ConflictConfirmationShown()
    {
      var confirmer = new TestConfirmer<ConfirmationResult>();
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel(new TestMTGCardImporter(), new()
      {
        AddSingleConflictConfirmer = confirmer
      })
      {
        Cards = [card],
      };

      await viewmodel.AddCardCommand.ExecuteAsync(card);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }
  }
}

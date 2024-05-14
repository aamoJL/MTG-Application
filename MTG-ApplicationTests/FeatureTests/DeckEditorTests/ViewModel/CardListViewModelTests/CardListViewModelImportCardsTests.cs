using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Services.IOService;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class CardListViewModelImportCardsTests
{
  [TestMethod]
  public void ImportCards_SerializedCardData_CardAdded()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI());
    JsonService.TrySerializeObject(Mocker.MTGCardModelMocker.CreateMTGCardModel(), out var json);

    viewmodel.ImportCardsCommand.Execute(json);

    Assert.AreEqual(1, viewmodel.Cards.Count);
  }

  [TestMethod]
  public void ImportCards_SerializedCardData_Undo_CardRemoved()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI());
    JsonService.TrySerializeObject(Mocker.MTGCardModelMocker.CreateMTGCardModel(), out var json);

    viewmodel.ImportCardsCommand.Execute(json);
    viewmodel.UndoStack.Undo();

    Assert.AreEqual(0, viewmodel.Cards.Count);
  }

  [TestMethod]
  public void ImportCards_SerializedCardData_Redo_CardAddedAgain()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI());
    JsonService.TrySerializeObject(Mocker.MTGCardModelMocker.CreateMTGCardModel(), out var json);

    viewmodel.ImportCardsCommand.Execute(json);
    viewmodel.UndoStack.Undo();
    viewmodel.UndoStack.Redo();

    Assert.AreEqual(1, viewmodel.Cards.Count);
  }
}
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.CardList.UseCases.NormalList;

[TestClass]
public class RemoveCard
{
  [TestMethod]
  public void RemoveCard_CardRemoved()
  {
    var viewmodel = new CardListViewModel([], new TestMTGCardImporter());
    var card = DeckEditorMTGCardMocker.CreateMTGCardModel();

    viewmodel.AddCardCommand.Execute(card);
    viewmodel.RemoveCardCommand.Execute(card);

    Assert.IsEmpty(viewmodel.Cards);
  }

  [TestMethod]
  public void RemoveCard_Undo_CardAdded()
  {
    var viewmodel = new CardListViewModel([], new TestMTGCardImporter());
    var card = DeckEditorMTGCardMocker.CreateMTGCardModel();

    viewmodel.AddCardCommand.Execute(card);
    viewmodel.RemoveCardCommand.Execute(card);
    viewmodel.UndoStack.Undo();

    Assert.HasCount(1, viewmodel.Cards);
  }

  [TestMethod]
  public void RemoveCard_Redo_CardRemovedAgain()
  {
    var viewmodel = new CardListViewModel([], new TestMTGCardImporter());
    var card = DeckEditorMTGCardMocker.CreateMTGCardModel();

    viewmodel.AddCardCommand.Execute(card);
    viewmodel.RemoveCardCommand.Execute(card);
    viewmodel.UndoStack.Undo();
    viewmodel.UndoStack.Redo();

    Assert.IsEmpty(viewmodel.Cards);
  }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class DeckEditorViewModelUndoTests
{
  [TestMethod]
  public void Undo_NoCommands_CanNotExecute()
  {
    var viewmodel = new DeckEditorViewModel();

    Assert.IsFalse(viewmodel.UndoCommand.CanExecute(null));
  }

  [TestMethod]
  public void Undo_HasCommand_CanExecute()
  {
    var viewmodel = new DeckEditorViewModel();

    viewmodel.DeckCardList.AddCardCommand.Execute(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    Assert.IsTrue(viewmodel.UndoCommand.CanExecute(null));
  }

  [TestMethod]
  public void Undo_Execute_ReverseActionInvoked()
  {
    var viewmodel = new DeckEditorViewModel();

    viewmodel.DeckCardList.AddCardCommand.Execute(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    Assert.AreEqual(1, viewmodel.DeckCardList.Cards.Count);

    viewmodel.UndoCommand.Execute(null);

    Assert.AreEqual(0, viewmodel.DeckCardList.Cards.Count);
  }
}
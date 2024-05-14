using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class DeckEditorViewModelRedoTests
{
  [TestMethod]
  public void Redo_NoCommands_CanNotExecute()
  {
    var viewmodel = new DeckEditorViewModel();

    Assert.IsFalse(viewmodel.RedoCommand.CanExecute(null));
  }

  [TestMethod]
  public void Redo_HasCommand_CanExecute()
  {
    var viewmodel = new DeckEditorViewModel();

    viewmodel.DeckCardList.AddCardCommand.Execute(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    viewmodel.UndoCommand.Execute(null);

    Assert.IsTrue(viewmodel.RedoCommand.CanExecute(null));
  }

  [TestMethod]
  public void Redo_Execute_ActionInvokedAgain()
  {
    var viewmodel = new DeckEditorViewModel();

    viewmodel.DeckCardList.AddCardCommand.Execute(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    Assert.AreEqual(1, viewmodel.DeckCardList.Cards.Count);

    viewmodel.UndoCommand.Execute(null);

    Assert.AreEqual(0, viewmodel.DeckCardList.Cards.Count);

    viewmodel.RedoCommand.Execute(null);

    Assert.AreEqual(1, viewmodel.DeckCardList.Cards.Count);
  }
}

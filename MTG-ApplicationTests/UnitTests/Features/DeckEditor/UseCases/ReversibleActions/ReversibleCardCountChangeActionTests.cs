using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases.ReversibleActions;

[TestClass]
public class ReversibleCardCountChangeActionTests
{
  [TestMethod]
  public void Change()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "1")) { Count = 1 };

    var action = new ReversibleCardCountChangeAction();

    action.Action((card, 2));

    Assert.AreEqual(2, card.Count);
  }

  [TestMethod]
  public void Change_Undo()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "1")) { Count = 1 };

    var action = new ReversibleCardCountChangeAction();

    action.Action((card, 2));
    action.ReverseAction((card, 1));

    Assert.AreEqual(1, card.Count);
  }

  [TestMethod]
  public void Change_Redo()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "1")) { Count = 1 };

    var action = new ReversibleCardCountChangeAction();

    action.Action((card, 2));
    action.ReverseAction((card, 1));
    action.Action((card, 2));
    action.ReverseAction((card, 1));
    action.Action((card, 2));

    Assert.AreEqual(2, card.Count);
  }
}
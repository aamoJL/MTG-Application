using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases.ReversibleActions;

[TestClass]
public class ReversibleCardGroupChangeActionTests
{
  [TestMethod]
  public void Change()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "1")) { Group = string.Empty };

    var action = new ReversibleCardGroupChangeAction();

    action.Action((card, "key"));

    Assert.AreEqual("key", card.Group);
  }

  [TestMethod]
  public void Change_Undo()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "1")) { Group = string.Empty };

    var action = new ReversibleCardGroupChangeAction();

    action.Action((card, "key"));
    action.ReverseAction((card, string.Empty));

    Assert.AreEqual(string.Empty, card.Group);
  }

  [TestMethod]
  public void Change_Redo()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "1")) { Group = string.Empty };

    var action = new ReversibleCardGroupChangeAction();

    action.Action((card, "key"));
    action.ReverseAction((card, string.Empty));
    action.Action((card, "key"));

    Assert.AreEqual("key", card.Group);
  }
}

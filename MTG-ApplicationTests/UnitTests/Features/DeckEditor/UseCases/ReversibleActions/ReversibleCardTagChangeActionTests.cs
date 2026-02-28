using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases.ReversibleActions;

[TestClass]
public class ReversibleCardTagChangeActionTests
{
  [TestMethod]
  public void Change()
  {
    CardTag? tag = null;
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "1")) { CardTag = tag };
    var action = new ReversibleCardTagChangeAction();

    var changed = CardTag.Add;
    action.Action((card, changed));

    Assert.AreEqual(changed, card.CardTag);
  }

  [TestMethod]
  public void Change_Undo()
  {
    CardTag? tag = null;
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "1")) { CardTag = tag };
    var action = new ReversibleCardTagChangeAction();

    var changed = CardTag.Add;
    action.Action((card, changed));
    action.ReverseAction((card, tag));

    Assert.AreEqual(tag, card.CardTag);
  }

  [TestMethod]
  public void Change_Redo()
  {
    CardTag? tag = null;
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "1")) { CardTag = tag };
    var action = new ReversibleCardTagChangeAction();

    var changed = CardTag.Add;
    action.Action((card, changed));
    action.ReverseAction((card, tag));
    action.Action((card, changed));
    action.ReverseAction((card, tag));
    action.Action((card, changed));

    Assert.AreEqual(changed, card.CardTag);
  }
}

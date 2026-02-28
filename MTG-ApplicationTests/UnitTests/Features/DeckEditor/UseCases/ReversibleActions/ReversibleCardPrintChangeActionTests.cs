using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases.ReversibleActions;

[TestClass]
public class ReversibleCardPrintChangeActionTests
{
  [TestMethod]
  public void Change()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1", setCode: "abc");
    var card = new DeckEditorMTGCard(info);
    var action = new ReversibleCardPrintChangeAction();

    var changed = info with { SetCode = "xyz" };
    action.Action((card, changed));

    Assert.AreEqual(changed, card.Info);
  }

  [TestMethod]
  public void Change_Undo()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1", setCode: "abc");
    var card = new DeckEditorMTGCard(info);
    var action = new ReversibleCardPrintChangeAction();

    var changed = info with { SetCode = "xyz" };
    action.Action((card, changed));
    action.ReverseAction((card, info));

    Assert.AreEqual(info, card.Info);
  }

  [TestMethod]
  public void Change_Redo()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1", setCode: "abc");
    var card = new DeckEditorMTGCard(info);
    var action = new ReversibleCardPrintChangeAction();

    var changed = info with { SetCode = "xyz" };
    action.Action((card, changed));
    action.ReverseAction((card, info));
    action.Action((card, changed));
    action.ReverseAction((card, info));
    action.Action((card, changed));

    Assert.AreEqual(changed, card.Info);
  }
}
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases.ReversibleActions;

[TestClass]
public class ReversibleCardCollectionGroupsChangeActionTests
{
  [TestMethod]
  public void ChangeGroups()
  {
    var action = new ReversibleCardCollectionGroupsChangeAction();

    IEnumerable<DeckEditorMTGCard> cards = [
      new(MTGCardInfoMocker.MockInfo(name: "1")){Group = "Old"},
      new(MTGCardInfoMocker.MockInfo(name: "2")){Group = "Old"},
      new(MTGCardInfoMocker.MockInfo(name: "3")){Group = "Old"},
    ];

    var param = (new List<DeckEditorMTGCard>(cards), "New");
    action.Action(param);

    Assert.IsTrue(cards.All(x => x.Group == "New"));
  }

  [TestMethod]
  public void ChangeGroups_Undo()
  {
    var action = new ReversibleCardCollectionGroupsChangeAction();

    IEnumerable<DeckEditorMTGCard> cards = [
      new(MTGCardInfoMocker.MockInfo(name: "1")){Group = "Old"},
      new(MTGCardInfoMocker.MockInfo(name: "2")){Group = "Old"},
      new(MTGCardInfoMocker.MockInfo(name: "3")){Group = "Old"},
    ];

    var param = (new List<DeckEditorMTGCard>(cards), "New");
    action.Action(param);
    action.ReverseAction(param);

    Assert.IsTrue(cards.All(x => x.Group == "Old"));
  }

  [TestMethod]
  public void ChangeGroups_Redo()
  {
    var action = new ReversibleCardCollectionGroupsChangeAction();

    IEnumerable<DeckEditorMTGCard> cards = [
      new(MTGCardInfoMocker.MockInfo(name: "1")){Group = "Old"},
      new(MTGCardInfoMocker.MockInfo(name: "2")){Group = "Old"},
      new(MTGCardInfoMocker.MockInfo(name: "3")){Group = "Old"},
    ];

    var param = (new List<DeckEditorMTGCard>(cards), "New");
    action.Action(param);
    action.ReverseAction(param);
    action.Action(param);
    action.ReverseAction(param);
    action.Action(param);

    Assert.IsTrue(cards.All(x => x.Group == "New"));
  }
}
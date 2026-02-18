using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases.ReversibleActions;

[TestClass]
public class ReversibleMoveCardsToGroupActionTests
{
  [TestMethod]
  public void Move()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo(name: "1")) { Group = string.Empty },
      new(MTGCardInfoMocker.MockInfo(name: "2")) { Group = "key" },
    ];
    var group = new DeckEditorCardGroup("key", source);

    var action = new ReversibleMoveCardsToGroupAction(group);

    var moved = source[0];
    action.Action([moved]);

    CollectionAssert.Contains(source, moved);
    CollectionAssert.Contains(group.Cards, moved);
    Assert.AreEqual("key", moved.Group);
  }

  [TestMethod]
  public void Move_Undo()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo(name: "1")) { Group = string.Empty },
      new(MTGCardInfoMocker.MockInfo(name: "2")) { Group = "key" },
    ];
    var group = new DeckEditorCardGroup("key", source);

    var action = new ReversibleMoveCardsToGroupAction(group);

    var moved = source[0];
    action.Action([moved]);
    action.ReverseAction([moved]);

    CollectionAssert.Contains(source, moved);
    CollectionAssert.DoesNotContain(group.Cards, moved);
    Assert.AreEqual(string.Empty, moved.Group);
  }

  [TestMethod]
  public void Move_Redo()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo(name: "1")) { Group = string.Empty },
      new(MTGCardInfoMocker.MockInfo(name: "2")) { Group = "key" },
    ];
    var group = new DeckEditorCardGroup("key", source);

    var action = new ReversibleMoveCardsToGroupAction(group);

    var moved = source[0];
    action.Action([moved]);
    action.ReverseAction([moved]);
    action.Action([moved]);

    CollectionAssert.Contains(source, moved);
    CollectionAssert.Contains(group.Cards, moved);
    Assert.AreEqual("key", moved.Group);
  }
}
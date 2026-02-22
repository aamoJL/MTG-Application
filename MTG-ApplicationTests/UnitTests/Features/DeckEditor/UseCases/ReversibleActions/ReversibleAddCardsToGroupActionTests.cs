using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases.ReversibleActions;

[TestClass]
public class ReversibleAddCardsToGroupActionTests
{
  [TestMethod]
  public void AddToGroup()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo(name: "1")) { Group = string.Empty }
    ];
    var group = new DeckEditorCardGroup("key", source);

    var action = new ReversibleAddCardsToGroupAction(group);

    var added = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "2")) { Group = "old" };
    action.Action([added]);

    CollectionAssert.Contains(source, added);
    CollectionAssert.Contains(group.Cards, added);
    Assert.AreEqual("key", added.Group);
  }

  [TestMethod]
  public void AddToGroup_Undo()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo(name: "1")) { Group = string.Empty }
    ];
    var group = new DeckEditorCardGroup("key", source);

    var action = new ReversibleAddCardsToGroupAction(group);

    var added = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "2")) { Group = "old" };
    action.Action([added]);
    action.ReverseAction([added]);

    CollectionAssert.DoesNotContain(source, added);
    CollectionAssert.DoesNotContain(group.Cards, added);
    Assert.AreEqual("old", added.Group);
  }

  [TestMethod]
  public void AddToGroup_Redo()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo(name: "1")) { Group = string.Empty }
    ];
    var group = new DeckEditorCardGroup("key", source);

    var action = new ReversibleAddCardsToGroupAction(group);

    var added = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "2")) { Group = "old" };
    action.Action([added]);
    action.ReverseAction([added]);
    action.Action([added]);
    action.ReverseAction([added]);
    action.Action([added]);

    CollectionAssert.Contains(source, added);
    CollectionAssert.Contains(group.Cards, added);
    Assert.AreEqual("key", added.Group);
  }

  [TestMethod]
  public void AddToGroup_Exists_ExceptionThrown()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo(name: "1")) { Group = string.Empty }
    ];
    var group = new DeckEditorCardGroup("key", source);

    var action = new ReversibleAddCardsToGroupAction(group);

    var added = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "1")) { Group = "old" };

    Assert.Throws<InvalidOperationException>(() => action.Action([added]));
  }
}
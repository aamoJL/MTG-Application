using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases.ReversibleActions;

[TestClass]
public class ReversibleRenameGroupActionTests
{
  [TestMethod]
  public void Rename()
  {
    var group = new DeckEditorCardGroup("Old", [
      new(MTGCardInfoMocker.MockInfo(name: "1")){ Group = "Old" },
      new(MTGCardInfoMocker.MockInfo(name: "2")){ Group = "Old" },
      new(MTGCardInfoMocker.MockInfo(name: "3")){ Group = "Old" },
    ]);
    var action = new ReversibleRenameGroupAction();

    action.Action((group, "New"));

    Assert.AreEqual("New", group.GroupKey);
    Assert.HasCount(3, group.Cards);
  }

  [TestMethod]
  public void Rename_Undo()
  {
    var group = new DeckEditorCardGroup("Old", [
      new(MTGCardInfoMocker.MockInfo(name: "1")){ Group = "Old" },
      new(MTGCardInfoMocker.MockInfo(name: "2")){ Group = "Old" },
      new(MTGCardInfoMocker.MockInfo(name: "3")){ Group = "Old" },
    ]);
    var action = new ReversibleRenameGroupAction();

    action.Action((group, "New"));
    action.ReverseAction((group, "Old"));

    Assert.AreEqual("Old", group.GroupKey);
    Assert.HasCount(3, group.Cards);
  }

  [TestMethod]
  public void Rename_Redo()
  {
    var group = new DeckEditorCardGroup("Old", [
      new(MTGCardInfoMocker.MockInfo(name: "1")){ Group = "Old" },
      new(MTGCardInfoMocker.MockInfo(name: "2")){ Group = "Old" },
      new(MTGCardInfoMocker.MockInfo(name: "3")){ Group = "Old" },
    ]);
    var action = new ReversibleRenameGroupAction();

    action.Action((group, "New"));
    action.ReverseAction((group, "Old"));
    action.Action((group, "New"));

    Assert.AreEqual("New", group.GroupKey);
    Assert.HasCount(3, group.Cards);
  }
}

[TestClass]
public class ReversibleRemoveCardsFromGroupActionTests
{
  [TestMethod]
  public void Remove()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo(name: "1")) {Group = "key" },
      new(MTGCardInfoMocker.MockInfo(name: "2")) {Group = "key" },
      new(MTGCardInfoMocker.MockInfo(name: "3")) {Group = "key" },
    ];
    var group = new DeckEditorCardGroup("key", source);
    var action = new ReversibleRemoveCardsFromGroupSourceAction(group);

    var removed = source[1];
    action.Action([removed]);

    CollectionAssert.DoesNotContain(source, removed);
    CollectionAssert.DoesNotContain(group.Cards, removed);
    Assert.HasCount(2, source);
    Assert.HasCount(2, group.Cards);
    Assert.AreEqual(string.Empty, removed.Group);
  }

  [TestMethod]
  public void Remove_Undo()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo(name: "1")) {Group = "key" },
      new(MTGCardInfoMocker.MockInfo(name: "2")) {Group = "key" },
      new(MTGCardInfoMocker.MockInfo(name: "3")) {Group = "key" },
    ];
    var group = new DeckEditorCardGroup("key", source);
    var action = new ReversibleRemoveCardsFromGroupSourceAction(group);

    var removed = source[1];
    action.Action([removed]);
    action.ReverseAction([removed]);

    CollectionAssert.Contains(source, removed);
    CollectionAssert.Contains(group.Cards, removed);
    Assert.HasCount(3, source);
    Assert.HasCount(3, group.Cards);
    Assert.AreEqual("key", removed.Group);
  }

  [TestMethod]
  public void Remove_Redo()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo(name: "1")) {Group = "key" },
      new(MTGCardInfoMocker.MockInfo(name: "2")) {Group = "key" },
      new(MTGCardInfoMocker.MockInfo(name: "3")) {Group = "key" },
    ];
    var group = new DeckEditorCardGroup("key", source);
    var action = new ReversibleRemoveCardsFromGroupSourceAction(group);

    var removed = source[1];
    action.Action([removed]);
    action.ReverseAction([removed]);
    action.Action([removed]);

    CollectionAssert.DoesNotContain(source, removed);
    CollectionAssert.DoesNotContain(group.Cards, removed);
    Assert.HasCount(2, source);
    Assert.HasCount(2, group.Cards);
    Assert.AreEqual(string.Empty, removed.Group);
  }
}
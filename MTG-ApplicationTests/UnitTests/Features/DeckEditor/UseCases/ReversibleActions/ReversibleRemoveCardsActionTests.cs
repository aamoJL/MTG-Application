using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases.ReversibleActions;

[TestClass]
public class ReversibleRemoveCardsActionTests
{
  [TestMethod]
  public void Remove()
  {
    var collection = new List<DeckEditorMTGCard>()
    {
      new(MTGCardInfoMocker.MockInfo(name: "1")),
      new(MTGCardInfoMocker.MockInfo(name: "2")),
      new(MTGCardInfoMocker.MockInfo(name: "3")),
    };
    var action = new ReversibleRemoveCardsAction(collection);

    var removed = collection[1];
    action.Action([removed]);

    CollectionAssert.DoesNotContain(collection, removed);
  }

  [TestMethod]
  public void Remove_Undo()
  {
    var collection = new List<DeckEditorMTGCard>()
    {
      new(MTGCardInfoMocker.MockInfo(name: "1")),
      new(MTGCardInfoMocker.MockInfo(name: "2")),
      new(MTGCardInfoMocker.MockInfo(name: "3")),
    };
    var action = new ReversibleRemoveCardsAction(collection);

    var removed = collection[1];
    action.Action([removed]);
    action.ReverseAction([removed]);

    CollectionAssert.Contains(collection, removed);
  }

  [TestMethod]
  public void Remove_Redo()
  {
    var collection = new List<DeckEditorMTGCard>()
    {
      new(MTGCardInfoMocker.MockInfo(name: "1")),
      new(MTGCardInfoMocker.MockInfo(name: "2")),
      new(MTGCardInfoMocker.MockInfo(name: "3")),
    };
    var action = new ReversibleRemoveCardsAction(collection);

    var removed = collection[1];
    action.Action([removed]);
    action.ReverseAction([removed]);
    action.Action([removed]);

    CollectionAssert.DoesNotContain(collection, removed);
  }

  [TestMethod]
  public void ReduceCount()
  {
    var collection = new List<DeckEditorMTGCard>()
    {
      new(MTGCardInfoMocker.MockInfo(name: "1")),
      new(MTGCardInfoMocker.MockInfo(name: "2")) { Count = 2 },
      new(MTGCardInfoMocker.MockInfo(name: "3")),
    };
    var action = new ReversibleRemoveCardsAction(collection);

    var card = new DeckEditorMTGCard(collection[1].Info with { }) { Count = 1 };
    action.Action([card]);

    Assert.AreEqual(1, collection[1].Count);
  }

  [TestMethod]
  public void ReduceCount_Undo()
  {
    var collection = new List<DeckEditorMTGCard>()
    {
      new(MTGCardInfoMocker.MockInfo(name: "1")),
      new(MTGCardInfoMocker.MockInfo(name: "2")) { Count = 2 },
      new(MTGCardInfoMocker.MockInfo(name: "3")),
    };
    var action = new ReversibleRemoveCardsAction(collection);

    var card = new DeckEditorMTGCard(collection[1].Info with { }) { Count = 1 };
    action.Action([card]);
    action.ReverseAction([card]);

    Assert.AreEqual(2, collection[1].Count);
  }

  [TestMethod]
  public void ReduceCount_Redo()
  {
    var collection = new List<DeckEditorMTGCard>()
    {
      new(MTGCardInfoMocker.MockInfo(name: "1")),
      new(MTGCardInfoMocker.MockInfo(name: "2")) { Count = 2 },
      new(MTGCardInfoMocker.MockInfo(name: "3")),
    };
    var action = new ReversibleRemoveCardsAction(collection);

    var card = new DeckEditorMTGCard(collection[1].Info with { }) { Count = 1 };
    action.Action([card]);
    action.ReverseAction([card]);
    action.Action([card]);

    Assert.AreEqual(1, collection[1].Count);
  }
}
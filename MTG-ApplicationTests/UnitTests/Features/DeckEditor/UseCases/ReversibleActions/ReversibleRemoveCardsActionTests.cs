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
    action.ReverseAction([removed]);
    action.Action([removed]);

    CollectionAssert.DoesNotContain(collection, removed);
  }

  [TestMethod]
  public void Remove_DoesNotExist_ExceptionThrown()
  {
    var collection = new List<DeckEditorMTGCard>()
    {
      new(MTGCardInfoMocker.MockInfo(name: "1")),
      new(MTGCardInfoMocker.MockInfo(name: "2")),
      new(MTGCardInfoMocker.MockInfo(name: "3")),
    };
    var action = new ReversibleRemoveCardsAction(collection);

    var removed = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "4"));

    Assert.Throws<InvalidOperationException>(() => action.Action([removed]));
  }
}
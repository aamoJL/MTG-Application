using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases.ReversibleActions;

[TestClass]
public class ReversibleAddCardsActionTests
{
  [TestMethod]
  public void AddCards()
  {
    var collection = new List<DeckEditorMTGCard>()
    {
      new(MTGCardInfoMocker.MockInfo(name: "1")),
      new(MTGCardInfoMocker.MockInfo(name: "2")),
      new(MTGCardInfoMocker.MockInfo(name: "3")),
    };

    var action = new ReversibleAddCardsAction(collection);

    DeckEditorMTGCard added = new(MTGCardInfoMocker.MockInfo(name: "4"));
    action.Action([added]);

    CollectionAssert.Contains(collection, added);
  }

  [TestMethod]
  public void AddCards_Undo()
  {
    var collection = new List<DeckEditorMTGCard>()
    {
      new(MTGCardInfoMocker.MockInfo(name: "1")),
      new(MTGCardInfoMocker.MockInfo(name: "2")),
      new(MTGCardInfoMocker.MockInfo(name: "3")),
    };

    var action = new ReversibleAddCardsAction(collection);

    DeckEditorMTGCard added = new(MTGCardInfoMocker.MockInfo(name: "4"));
    action.Action([added]);
    action.ReverseAction([added]);

    CollectionAssert.DoesNotContain(collection, added);
  }

  [TestMethod]
  public void AddCards_Redo()
  {
    var collection = new List<DeckEditorMTGCard>()
    {
      new(MTGCardInfoMocker.MockInfo(name: "1")),
      new(MTGCardInfoMocker.MockInfo(name: "2")),
      new(MTGCardInfoMocker.MockInfo(name: "3")),
    };

    var action = new ReversibleAddCardsAction(collection);

    DeckEditorMTGCard added = new(MTGCardInfoMocker.MockInfo(name: "4"));
    action.Action([added]);
    action.ReverseAction([added]);
    action.Action([added]);
    action.ReverseAction([added]);
    action.Action([added]);

    CollectionAssert.Contains(collection, added);
  }

  [TestMethod]
  public void AddCards_Exists_ExceptionThrown()
  {
    var collection = new List<DeckEditorMTGCard>()
    {
      new(MTGCardInfoMocker.MockInfo(name: "1")),
      new(MTGCardInfoMocker.MockInfo(name: "2")),
      new(MTGCardInfoMocker.MockInfo(name: "3")),
    };

    var action = new ReversibleAddCardsAction(collection);

    DeckEditorMTGCard added = new(MTGCardInfoMocker.MockInfo(name: "1"));

    Assert.Throws<InvalidOperationException>(() => action.Action([added]));
  }
}
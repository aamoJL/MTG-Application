using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases.ReversibleActions;

[TestClass]
public class ReversibleAddGroupActionTests
{
  [TestMethod]
  public void Add()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo(name: "1")){ Group = "1" },
      new(MTGCardInfoMocker.MockInfo(name: "2")){ Group = "1" },
      new(MTGCardInfoMocker.MockInfo(name: "3")){ Group = string.Empty },
    ];
    var groups = new List<DeckEditorCardGroup>()
    {
      new("1", source),
      new(string.Empty, source),
    };

    var action = new ReversibleAddGroupAction(groups);

    var added = new DeckEditorCardGroup("2", source);
    action.Action(added);

    CollectionAssert.Contains(groups, added);
    Assert.AreEqual(groups[1], added); // Alphabetical order, empty last
    Assert.HasCount(0, added.Cards);
  }

  [TestMethod]
  public void Add_Undo()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo(name: "1")){ Group = "1" },
      new(MTGCardInfoMocker.MockInfo(name: "2")){ Group = "1" },
      new(MTGCardInfoMocker.MockInfo(name: "3")){ Group = string.Empty },
    ];
    var groups = new List<DeckEditorCardGroup>()
    {
      new("1", source),
      new(string.Empty, source),
    };

    var action = new ReversibleAddGroupAction(groups);

    var added = new DeckEditorCardGroup("2", source);
    action.Action(added);
    action.ReverseAction(added);

    CollectionAssert.DoesNotContain(groups, added);
  }

  [TestMethod]
  public void Add_Redo()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo(name: "1")){ Group = "1" },
      new(MTGCardInfoMocker.MockInfo(name: "2")){ Group = "1" },
      new(MTGCardInfoMocker.MockInfo(name: "3")){ Group = string.Empty },
    ];
    var groups = new List<DeckEditorCardGroup>()
    {
      new("1", source),
      new(string.Empty, source),
    };

    var action = new ReversibleAddGroupAction(groups);

    var added = new DeckEditorCardGroup("2", source);
    action.Action(added);
    action.ReverseAction(added);
    action.Action(added);
    action.ReverseAction(added);
    action.Action(added);

    CollectionAssert.Contains(groups, added);
    Assert.AreEqual(groups[1], added); // Alphabetical order, empty last
    Assert.HasCount(0, added.Cards);
  }
}
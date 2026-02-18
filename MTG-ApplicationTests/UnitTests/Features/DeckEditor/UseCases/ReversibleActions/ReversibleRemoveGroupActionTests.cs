using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases.ReversibleActions;

[TestClass]
public class ReversibleRemoveGroupActionTests
{
  [TestMethod]
  public void RemoveGroup()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo()){ Group = "Key 1" },
      new(MTGCardInfoMocker.MockInfo()){ Group = "Key 2" },
      new(MTGCardInfoMocker.MockInfo()){ Group = string.Empty },
    ];

    var groups = new List<DeckEditorCardGroup>()
    {
      new("Key 1", source),
      new("Key 2", source),
      new(string.Empty, source),
    };
    var action = new ReversibleRemoveGroupAction(groups);

    var removed = groups[1];
    action.Action(removed);

    Assert.HasCount(2, groups);
    Assert.HasCount(0, removed.Cards);
    Assert.HasCount(2, groups.FirstOrDefault(x => x.GroupKey == string.Empty)!.Cards);
  }

  [TestMethod]
  public void RemoveGroup_Undo()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo()){ Group = "Key 1" },
      new(MTGCardInfoMocker.MockInfo()){ Group = "Key 2" },
      new(MTGCardInfoMocker.MockInfo()){ Group = string.Empty },
    ];

    var groups = new List<DeckEditorCardGroup>()
    {
      new("Key 1", source),
      new("Key 2", source),
      new(string.Empty, source),
    };

    var action = new ReversibleRemoveGroupAction(groups);

    var removed = groups[1];
    action.Action(removed);
    action.ReverseAction(removed);

    Assert.HasCount(3, groups);
    Assert.HasCount(1, removed.Cards);
    Assert.HasCount(1, groups.FirstOrDefault(x => x.GroupKey == string.Empty)!.Cards);
  }

  [TestMethod]
  public void RemoveGroup_Redo()
  {
    ObservableCollection<DeckEditorMTGCard> source = [
      new(MTGCardInfoMocker.MockInfo()){ Group = "Key 1" },
      new(MTGCardInfoMocker.MockInfo()){ Group = "Key 2" },
      new(MTGCardInfoMocker.MockInfo()){ Group = string.Empty },
    ];

    var groups = new List<DeckEditorCardGroup>()
    {
      new("Key 1", source),
      new("Key 2", source),
      new(string.Empty, source),
    };

    var action = new ReversibleRemoveGroupAction(groups);

    var removed = groups[1];
    action.Action(removed);
    action.ReverseAction(removed);
    action.Action(removed);

    Assert.HasCount(2, groups);
    Assert.HasCount(0, removed.Cards);
    Assert.HasCount(2, groups.FirstOrDefault(x => x.GroupKey == string.Empty)!.Cards);
  }
}
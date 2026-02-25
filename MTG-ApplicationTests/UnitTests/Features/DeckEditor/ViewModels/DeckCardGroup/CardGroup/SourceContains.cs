using MTGApplication.Features.DeckEditor.Models;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;

[TestClass]
public class SourceContains
{
  [TestMethod]
  public void Contains()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Name")) { Count = 1, Group = string.Empty };
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new(string.Empty, [card])
    };
    var vm = factory.Build();

    Assert.IsTrue(vm.SourceContains(card));
  }

  [TestMethod]
  public void DoesNotContain()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Name")) { Count = 1, Group = string.Empty };
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new(string.Empty, [])
    };
    var vm = factory.Build();

    Assert.IsFalse(vm.SourceContains(card));
  }
}

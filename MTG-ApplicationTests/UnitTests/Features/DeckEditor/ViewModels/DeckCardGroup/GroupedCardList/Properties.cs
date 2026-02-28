using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_GroupViewModels()
  {
    var factory = new TestGroupedDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo()){Group = "3"},
        new(MTGCardInfoMocker.MockInfo()){Group = string.Empty},
        new(MTGCardInfoMocker.MockInfo()){Group = "1"},
        new(MTGCardInfoMocker.MockInfo()){Group = "2"},
      ]
    };
    var vm = factory.Build();

    Assert.HasCount(4, vm.GroupViewModels);

    var expected = new string[] { "1", "2", "3", string.Empty };
    CollectionAssert.AreEqual(expected, vm.GroupViewModels.Select(x => x.GroupKey).ToArray());
  }
}
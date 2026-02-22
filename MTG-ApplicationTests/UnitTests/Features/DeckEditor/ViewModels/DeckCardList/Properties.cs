using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardList;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_CardViewModels_Empty()
  {
    var factory = new TestDeckCardListViewModelFactory();
    var vm = factory.Build();

    Assert.IsEmpty(vm.CardViewModels);
  }

  [TestMethod]
  public void Init_CardViewModels_Populated()
  {
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo()),
        new(MTGCardInfoMocker.MockInfo()),
        new(MTGCardInfoMocker.MockInfo()),
      ]
    };
    var vm = factory.Build();

    Assert.HasCount(3, vm.CardViewModels);
  }
}
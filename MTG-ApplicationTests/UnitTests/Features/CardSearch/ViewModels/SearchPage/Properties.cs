namespace MTGApplicationTests.UnitTests.Features.CardSearch.ViewModels.SearchPage;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_QueryCards()
  {
    var factory = new TestSearchPageViewModelFactory();
    var vm = factory.Build();

    Assert.HasCount(0, vm.QueryCards.Collection);
    Assert.AreEqual(0, vm.QueryCards.TotalCardCount);
  }
}
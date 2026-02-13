namespace MTGApplicationTests.UnitTests.Features.DeckSelection.ViewModels.SelectionPage;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_DeckItems()
  {
    var factory = new TestDeckSelectionPageViewModelFactory();
    var vm = factory.Build();

    Assert.HasCount(0, vm.DeckItems);
  }
}

namespace MTGApplicationTests.UnitTests.Features.EdhrecSearch.ViewModels;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_CommanderThemes()
  {
    var factory = new TestEDHSearchPageViewModelFactory();
    var vm = factory.Build();

    Assert.HasCount(0, vm.CommanderThemes);
  }

  [TestMethod]
  public void Set_CommanderThemes()
  {
    var factory = new TestEDHSearchPageViewModelFactory();
    var vm = factory.Build();

    vm.CommanderThemes = [new()];

    Assert.HasCount(1, vm.CommanderThemes);
  }

  [TestMethod]
  public void Init_QueryCards()
  {
    var factory = new TestEDHSearchPageViewModelFactory();
    var vm = factory.Build();

    Assert.HasCount(0, vm.QueryCards.Collection);
    Assert.AreEqual(0, vm.QueryCards.TotalCardCount);
  }

  [TestMethod]
  public void Set_QueryCards_PropertyChanged()
  {
    var factory = new TestEDHSearchPageViewModelFactory();
    var vm = factory.Build();

    var changed = false;
    vm.PropertyChanged += (_, e) =>
    {
      if (e.PropertyName == nameof(vm.QueryCards))
        changed = true;
    };

    vm.SelectCommanderThemeCommand.Execute(new());

    Assert.IsTrue(changed);
  }
}

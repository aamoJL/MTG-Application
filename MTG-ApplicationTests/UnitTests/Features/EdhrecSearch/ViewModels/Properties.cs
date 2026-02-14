using MTGApplicationTests.TestUtility.Importers;

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
  public async Task Set_QueryCards_PropertyChanged()
  {
    var factory = new TestEDHSearchPageViewModelFactory()
    {
      EdhrecImporter = new()
      {
        CardNames = ["Name"]
      },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([])
      }
    };
    var vm = factory.Build();

    var changed = false;
    vm.PropertyChanged += (_, e) =>
    {
      if (e.PropertyName == nameof(vm.QueryCards))
        changed = true;
    };

    await vm.SelectCommanderThemeCommand.ExecuteAsync(new());

    Assert.IsTrue(changed);
  }
}

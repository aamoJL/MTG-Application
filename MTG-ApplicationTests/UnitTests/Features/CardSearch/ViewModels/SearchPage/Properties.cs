using MTGApplicationTests.TestUtility.Importers;

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

  [TestMethod]
  public async Task Set_QueryCards_PropertyChanged()
  {
    var factory = new TestSearchPageViewModelFactory()
    {
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

    await vm.SubmitSearchCommand.ExecuteAsync("query");

    Assert.IsTrue(changed);
  }
}
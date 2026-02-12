using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.CardSearch.ViewModels.SearchPage;

[TestClass]
public class SubmitSearch
{
  [TestMethod]
  public async Task Submit_QueryCardsChanged()
  {
    var factory = new TestSearchPageViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo())]),
      }
    };
    var vm = factory.Build();

    // First, set the collection to have cards
    await vm.SubmitSearchCommand.ExecuteAsync("query");

    Assert.AreEqual(1, vm.QueryCards.TotalCardCount);
  }

  [TestMethod]
  public async Task Submit_QueryCards_ClearedBeforeFetch()
  {
    var factory = new TestSearchPageViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo())]),
      }
    };
    var vm = factory.Build();

    // First, set the collection to have cards
    await vm.SubmitSearchCommand.ExecuteAsync("query");

    Assert.AreEqual(1, vm.QueryCards.TotalCardCount);

    var cleared = false;
    vm.PropertyChanged += (_, e) =>
    {
      if (e.PropertyName == nameof(vm.QueryCards))
      {
        if (vm.QueryCards.TotalCardCount == 0)
          cleared = true;
      }
    };

    // Second, submit again to know if the collection will be cleared
    await vm.SubmitSearchCommand.ExecuteAsync("query");

    Assert.IsTrue(cleared);
  }

  [TestMethod]
  public async Task Submit_Cancelled_QueryCardsNotChanged()
  {
    var factory = new TestSearchPageViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo())]),
        CancellationTokenSource = new()
      }
    };
    var vm = factory.Build();

    var task = vm.SubmitSearchCommand.ExecuteAsync("query");
    vm.SubmitSearchCommand.Cancel();
    factory.Importer.CancellationTokenSource.Cancel();
    await task;

    Assert.HasCount(0, vm.QueryCards.Collection);
  }

  [TestMethod]
  public async Task Submit_Cancelled_Return()
  {
    var factory = new TestSearchPageViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([]),
        CancellationTokenSource = new()
      }
    };
    var vm = factory.Build();

    var task = vm.SubmitSearchCommand.ExecuteAsync("query");
    vm.SubmitSearchCommand.Cancel();
    factory.Importer.CancellationTokenSource.Cancel();
    await task;

    NotificationAssert.NotificationNotSent(NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Submit_Exception_NotificationShown()
  {
    var factory = new TestSearchPageViewModelFactory()
    {
      Importer = new()
      {
        Result = null
      }
    };
    var vm = factory.Build();

    await vm.SubmitSearchCommand.ExecuteAsync("query");

    NotificationAssert.NotificationSent(NotificationType.Error, factory.Notifier);
  }
}

using MTGApplication.Features.EdhrecSearch.ViewModels;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.Features.CardSearch.ViewModels.SearchCard.CardSearchMTGCardViewModel;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.EdhrecSearch.ViewModels;

public class TestEDHSearchPageViewModelFactory
{
  public Worker Worker { get; set; } = new();
  public TestMTGCardImporter Importer { get; set; } = new();
  public TestEdhrecImporter EdhrecImporter { get; set; } = new TestEdhrecImporter();
  public TestNotifier Notifier { get; set; } = new();
  public SearchCardConfirmers CardConfirmers { get; set; } = new();

  public EdhrecSearchPageViewModel Build()
  {
    return new()
    {
      Worker = Worker,
      Importer = Importer,
      EdhrecImporter = EdhrecImporter,
      Notifier = Notifier,
      CardConfirmers = CardConfirmers,
    };
  }
}

[TestClass]
public class SelectCommanderTheme
{
  [TestMethod]
  public async Task Select_QueryCardsChanged()
  {
    var factory = new TestEDHSearchPageViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo())]),
      },
      EdhrecImporter = new()
      {
        CardNames = ["Name"]
      }
    };
    var vm = factory.Build();

    await vm.SelectCommanderThemeCommand.ExecuteAsync(new());

    Assert.AreEqual(1, vm.QueryCards.TotalCardCount);
  }

  [TestMethod]
  public async Task Select_QueryCards_ClearedBeforeFetch()
  {
    var factory = new TestEDHSearchPageViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo())]),
      },
      EdhrecImporter = new()
      {
        CardNames = ["Name"]
      }
    };
    var vm = factory.Build();

    // First, set the collection to have cards
    await vm.SelectCommanderThemeCommand.ExecuteAsync(new());

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
    await vm.SelectCommanderThemeCommand.ExecuteAsync(new());

    Assert.IsTrue(cleared);
  }

  [TestMethod]
  public async Task Select_Cancelled_QueryCardsNotChanged()
  {
    var factory = new TestEDHSearchPageViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo())]),
        CancellationTokenSource = new()
      }
    };
    var vm = factory.Build();

    var task = vm.SelectCommanderThemeCommand.ExecuteAsync(new());
    vm.SelectCommanderThemeCommand.Cancel();
    factory.Importer.CancellationTokenSource.Cancel();
    await task;

    Assert.HasCount(0, vm.QueryCards.Collection);
  }

  [TestMethod]
  public async Task Select_Cancelled_Return()
  {
    var factory = new TestEDHSearchPageViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([]),
        CancellationTokenSource = new()
      },
      EdhrecImporter = new()
      {
        CardNames = ["Name"]
      }
    };
    var vm = factory.Build();

    var task = vm.SelectCommanderThemeCommand.ExecuteAsync(new());
    vm.SelectCommanderThemeCommand.Cancel();
    factory.Importer.CancellationTokenSource.Cancel();
    await task;

    NotificationAssert.NotificationNotSent(NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Select_Exception_NotificationShown()
  {
    var factory = new TestEDHSearchPageViewModelFactory()
    {
      Importer = new()
      {
        Result = null
      }
    };
    var vm = factory.Build();

    await vm.SelectCommanderThemeCommand.ExecuteAsync(new());

    NotificationAssert.NotificationSent(NotificationType.Error, factory.Notifier);
  }
}
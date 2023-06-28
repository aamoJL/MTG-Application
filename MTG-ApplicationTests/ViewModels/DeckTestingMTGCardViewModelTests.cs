using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.ViewModels;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.ViewModels;
[TestClass]
public class DeckTestingMTGCardViewModelTests
{
  [TestMethod]
  public void IncreasePlusCountersTest()
  {
    var vm = new DeckTestingMTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    Assert.AreEqual(vm.PlusCounters, 0);

    vm.IncreasePlusCounters();
    Assert.AreEqual(vm.PlusCounters, 1);
  }

  [TestMethod]
  public void DecreasePlusCountersTest()
  {
    var counters = 2;
    var vm = new DeckTestingMTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel()) { PlusCounters = counters };

    Assert.AreEqual(vm.PlusCounters, counters);

    vm.DecreasePlusCounters();
    Assert.AreEqual(vm.PlusCounters, counters - 1);
  }

  [TestMethod]
  public void DecreasePlusCountersTest_Zero()
  {
    var counters = 0;
    var vm = new DeckTestingMTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel()) { PlusCounters = counters };

    Assert.AreEqual(vm.PlusCounters, counters);

    vm.DecreasePlusCounters();
    Assert.AreEqual(vm.PlusCounters, counters);
  }

  [TestMethod]
  public void IncreaseCountCountersTest()
  {
    var vm = new DeckTestingMTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    Assert.AreEqual(vm.CountCounters, 1);

    vm.IncreaseCountCounters();
    Assert.AreEqual(vm.CountCounters, 2);
  }

  [TestMethod]
  public void DecreaseCountCountersTest()
  {
    var counters = 2;
    var vm = new DeckTestingMTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel()) { CountCounters = counters };

    Assert.AreEqual(vm.CountCounters, counters);

    vm.DecreaseCountCounters();
    Assert.AreEqual(vm.CountCounters, counters - 1);
  }

  [TestMethod]
  public void DecreaseCountCountersTest_One()
  {
    var counters = 1;
    var vm = new DeckTestingMTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel()) { CountCounters = counters };

    Assert.AreEqual(vm.CountCounters, counters);

    vm.DecreaseCountCounters();
    Assert.AreEqual(vm.CountCounters, counters);
  }
}

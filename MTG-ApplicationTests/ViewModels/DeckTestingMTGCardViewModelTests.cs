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
}

using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.ViewModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.Deck;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_DeckName()
  {
    var factory = new TestDeckViewModelFactory();
    var vm = factory.Build();

    Assert.AreEqual(string.Empty, vm.DeckName);
  }

  [TestMethod]
  public void Change_ModelName_DeckNameChanged()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Model = new() { Name = string.Empty }
    };
    var vm = factory.Build();

    vm.AssertPropertyChanged(nameof(vm.DeckName), () =>
    {
      factory.Model.Name = "New";
    });

    Assert.AreEqual("New", vm.DeckName);
  }

  [TestMethod]
  public void Init_DeckSize()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Model = new()
      {
        DeckCards = [
          new(MTGCardInfoMocker.MockInfo()){Count = 1},
          new(MTGCardInfoMocker.MockInfo()){Count = 2},
          new(MTGCardInfoMocker.MockInfo()){Count = 3},
        ],
        Commander = new(MTGCardInfoMocker.MockInfo()),
        CommanderPartner = new(MTGCardInfoMocker.MockInfo()),
      }
    };
    var vm = factory.Build();

    Assert.AreEqual(8, vm.DeckSize);
  }

  [TestMethod]
  public void Change_ModelCards_DeckSizeChanged()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Model = new()
      {
        DeckCards = [new(MTGCardInfoMocker.MockInfo()) { Count = 1 }],
      }
    };
    var vm = factory.Build();

    vm.AssertPropertyChanged(nameof(vm.DeckSize), () =>
    {
      factory.Model.DeckCards.Add(new(MTGCardInfoMocker.MockInfo()) { Count = 2 });
    });
    Assert.AreEqual(3, vm.DeckSize);

    vm.AssertPropertyChanged(nameof(vm.DeckSize), () =>
    {
      factory.Model.DeckCards.RemoveAt(0);
    });
    Assert.AreEqual(2, vm.DeckSize);
  }

  [TestMethod]
  public void Change_ModelCommanders_DeckSizeChanged()
  {
    var factory = new TestDeckViewModelFactory();
    var vm = factory.Build();

    vm.AssertPropertyChanged(nameof(vm.DeckSize), () =>
    {
      factory.Model.Commander = new(MTGCardInfoMocker.MockInfo());
    });
    Assert.AreEqual(1, vm.DeckSize);

    vm.AssertPropertyChanged(nameof(vm.DeckSize), () =>
    {
      factory.Model.CommanderPartner = new(MTGCardInfoMocker.MockInfo());
    });
    Assert.AreEqual(2, vm.DeckSize);

    vm.AssertPropertyChanged(nameof(vm.DeckSize), () =>
    {
      factory.Model.Commander = null;
    });
    Assert.AreEqual(1, vm.DeckSize);

    vm.AssertPropertyChanged(nameof(vm.DeckSize), () =>
    {
      factory.Model.CommanderPartner = null;
    });
    Assert.AreEqual(0, vm.DeckSize);
  }

  [TestMethod]
  public void Init_DeckPrice()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Model = new()
      {
        DeckCards = [
          new(MTGCardInfoMocker.MockInfo(price: 1)){Count = 1},
          new(MTGCardInfoMocker.MockInfo(price: 2)){Count = 2},
          new(MTGCardInfoMocker.MockInfo(price: 3)){Count = 3},
        ],
        Commander = new(MTGCardInfoMocker.MockInfo(price: 4)),
        CommanderPartner = new(MTGCardInfoMocker.MockInfo(price: 5)),
      }
    };
    var vm = factory.Build();

    Assert.AreEqual(23, vm.DeckPrice);
  }

  [TestMethod]
  public void Change_ModelCards_DeckPriceChanged()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Model = new()
      {
        DeckCards = [new(MTGCardInfoMocker.MockInfo(price: 1)) { Count = 1 }],
      }
    };
    var vm = factory.Build();

    vm.AssertPropertyChanged(nameof(vm.DeckPrice), () =>
    {
      factory.Model.DeckCards.Add(new(MTGCardInfoMocker.MockInfo(price: 2)) { Count = 2 });
    });
    Assert.AreEqual(5, vm.DeckPrice);

    vm.AssertPropertyChanged(nameof(vm.DeckPrice), () =>
    {
      factory.Model.DeckCards.RemoveAt(0);
    });
    Assert.AreEqual(4, vm.DeckPrice);
  }

  [TestMethod]
  public void Change_ModelCommanders_DeckPriceChanged()
  {
    var factory = new TestDeckViewModelFactory();
    var vm = factory.Build();

    vm.AssertPropertyChanged(nameof(vm.DeckPrice), () =>
    {
      factory.Model.Commander = new(MTGCardInfoMocker.MockInfo(price: 5));
    });
    Assert.AreEqual(5, vm.DeckPrice);

    vm.AssertPropertyChanged(nameof(vm.DeckPrice), () =>
    {
      factory.Model.CommanderPartner = new(MTGCardInfoMocker.MockInfo(price: 3));
    });
    Assert.AreEqual(8, vm.DeckPrice);

    vm.AssertPropertyChanged(nameof(vm.DeckPrice), () =>
    {
      factory.Model.Commander = null;
    });
    Assert.AreEqual(3, vm.DeckPrice);

    vm.AssertPropertyChanged(nameof(vm.DeckPrice), () =>
    {
      factory.Model.CommanderPartner = null;
    });
    Assert.AreEqual(0, vm.DeckPrice);
  }
}
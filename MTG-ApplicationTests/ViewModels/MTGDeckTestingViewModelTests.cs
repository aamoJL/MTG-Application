using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.ViewModels;

[TestClass]
public class MTGDeckTestingViewModelTests
{
  [TestMethod]
  public void NewGameTest_Init()
  {
    var cards = new MTGCard[] { Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 20) };
    var vm = new MTGDeckTestingViewModel(cards);

    Assert.AreEqual(cards.Sum(x => x.Count) - 7, vm.Library.Count);
    Assert.AreEqual(7, vm.Hand.Count);
    Assert.AreEqual(0, vm.Exile.Count);
    Assert.AreEqual(0, vm.Graveyard.Count);
    ShuffleTest();
  }

  [TestMethod]
  public void DrawTest()
  {
    var cards = new MTGCard[] { Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 20) };
    var vm = new MTGDeckTestingViewModel(cards);

    var oldLibraryCount = vm.Library.Count;
    var oldHandCount = vm.Hand.Count;

    vm.Draw();
    Assert.AreEqual(oldLibraryCount - 1, vm.Library.Count);
    Assert.AreEqual(oldHandCount + 1, vm.Hand.Count);
  }

  [TestMethod]
  public void DrawTest_EmptyLibrary()
  {
    var cards = new MTGCard[] { Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 1) };
    var vm = new MTGDeckTestingViewModel(cards);

    vm.Draw();
    vm.Draw();
    Assert.AreEqual(0, vm.Library.Count);
    Assert.AreEqual(1, vm.Hand.Count);
  }

  [TestMethod]
  public void ShuffleTest()
  {
    var cards = new MTGCard[] { Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 20) };
    var vm = new MTGDeckTestingViewModel(cards);

    var oldLibrary = vm.Library.ToArray();

    vm.Shuffle();
    CollectionAssert.AreNotEqual(oldLibrary, vm.Library.ToArray());
    CollectionAssert.AreEquivalent(oldLibrary, vm.Library.ToArray());
  }

  [TestMethod]
  public void LibraryAddTopTest()
  {
    var cards = new MTGCard[] { Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 5) };
    var vm = new MTGDeckTestingViewModel(cards);
    var newCard = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    vm.LibraryAddBottom(newCard);
    Assert.AreEqual(newCard, vm.Library.First());
  }

  [TestMethod]
  public void LibraryAddBottomTest()
  {
    var cards = new MTGCard[] { Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 5) };
    var vm = new MTGDeckTestingViewModel(cards);
    var newCard = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    vm.LibraryAddBottom(newCard);
    Assert.AreEqual(newCard, vm.Library.Last());
  }
}

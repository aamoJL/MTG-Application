using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.ViewModels;

[TestClass]
public class MTGDeckTestingViewModelTests
{
  public static MTGDeckTestingViewModel Init(int cardCount = 100)
  {
    var vm = new MTGDeckTestingViewModel(new TestCardAPI())
    {
      DeckCards = new MTGCard[] { Mocker.MTGCardModelMocker.CreateMTGCardModel(count: cardCount) },
    };
    vm.NewGame();

    return vm;
  }

  [TestMethod]
  public void NewGameTest_Init()
  {
    var count = 100;
    var vm = Init(count);

    Assert.AreEqual(count - 7, vm.Library.Count);
    Assert.AreEqual(7, vm.Hand.Count);
    Assert.AreEqual(0, vm.Exile.Count);
    Assert.AreEqual(0, vm.Graveyard.Count);
    ShuffleTest();
  }

  [TestMethod]
  public void DrawTest()
  {
    var vm = Init();

    var oldLibraryCount = vm.Library.Count;
    var oldHandCount = vm.Hand.Count;

    vm.Draw();
    Assert.AreEqual(oldLibraryCount - 1, vm.Library.Count);
    Assert.AreEqual(oldHandCount + 1, vm.Hand.Count);
  }

  [TestMethod]
  public void DrawTest_EmptyLibrary()
  {
    var vm = Init(1);

    vm.Draw();
    vm.Draw();
    Assert.AreEqual(0, vm.Library.Count);
    Assert.AreEqual(1, vm.Hand.Count);
  }

  [TestMethod]
  public void ShuffleTest()
  {
    var vm = Init(10);

    var oldLibrary = vm.Library.ToArray();

    vm.Shuffle();
    CollectionAssert.AreNotEqual(oldLibrary, vm.Library.ToArray());
    CollectionAssert.AreEquivalent(oldLibrary, vm.Library.ToArray());
  }

  [TestMethod]
  public void LibraryAddTopTest()
  {
    var vm = Init(5);
    var newCard = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    vm.LibraryAddBottom(newCard);
    Assert.AreEqual(newCard, vm.Library.First());
  }

  [TestMethod]
  public void LibraryAddBottomTest()
  {
    var vm = Init(5);
    var newCard = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    vm.LibraryAddBottom(newCard);
    Assert.AreEqual(newCard, vm.Library.Last());
  }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.ViewModels;

[TestClass]
public class MTGDeckTestingViewModelTests
{
  public static MTGDeckTestingViewModel Init(int cardCount = 100)
  {
    var vm = new MTGDeckTestingViewModel(deck: new MTGCardDeck()
    {
      DeckCards = new() { Mocker.MTGCardModelMocker.CreateMTGCardModel(count: cardCount) },
      Commander = Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 1, name: "Commander"),
      CommanderPartner = Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 1, name: "Partner"),
    });
    vm.NewGame();

    return vm;
  }

  [TestMethod]
  public void NewGameTest_Init()
  {
    var count = 100;
    var vm = Init(count);

    var unsortedLibrary = new List<MTGCardViewModel>();

    foreach (var item in vm.CardDeck.DeckCards)
    {
      for (var i = 0; i < item.Count; i++)
      {
        unsortedLibrary.Add(new(item));
      }
    }

    Assert.AreEqual(count - 7, vm.Library.Count);
    Assert.AreEqual(7, vm.Hand.Count);
    Assert.AreEqual(0, vm.Exile.Count);
    Assert.AreEqual(0, vm.Graveyard.Count);
    Assert.AreEqual(2, vm.CommandZone.Count);
    Assert.AreEqual(0, vm.TurnCount);
    CollectionAssert.AreNotEqual(unsortedLibrary.ToArray(), vm.Library.ToArray());
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
    var vm = Init(8);

    vm.Draw();
    vm.Draw();
    Assert.AreEqual(0, vm.Library.Count);
    Assert.AreEqual(8, vm.Hand.Count);
  }

  [TestMethod]
  public void NewTurnTest()
  {
    var vm = Init(20);

    vm.NewTurn();
    Assert.AreEqual(8, vm.Hand.Count);
    Assert.AreEqual(1, vm.TurnCount);
  }

  [TestMethod]
  public void ShuffleTest()
  {
    var vm = Init(100);

    var oldLibrary = vm.Library.ToArray();

    vm.Shuffle();
    CollectionAssert.AreNotEqual(oldLibrary, vm.Library.ToArray());
    CollectionAssert.AreEquivalent(oldLibrary, vm.Library.ToArray());
  }

  [TestMethod]
  public void LibraryAddTopTest()
  {
    var vm = Init(5);
    var newCard = new DeckTestingMTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    vm.LibraryAddBottom(newCard);
    Assert.AreEqual(newCard, vm.Library.First());
  }

  [TestMethod]
  public void LibraryAddBottomTest()
  {
    var vm = Init(5);
    var newCard = new DeckTestingMTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    vm.LibraryAddBottom(newCard);
    Assert.AreEqual(newCard, vm.Library.Last());
  }
}

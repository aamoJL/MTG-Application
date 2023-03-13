﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.ViewModels;
using MTGApplication.Models;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.ViewModels
{
  [TestClass]
  public class MTGCardViewModelTests
  {
    [TestMethod]
    public void FlipCardCommand_TwoFacesTest()
    {
      var frontUri = "frontUri";
      var backUri = "backUri";

      var vm = new MTGCardViewModel(
        Mocker.MTGCardModelMocker.CreateMTGCardModel(
          frontFace: Mocker.MTGCardModelMocker.CreateCardFace(imageUri: frontUri),
          backFace: Mocker.MTGCardModelMocker.CreateCardFace(imageUri: backUri)));

      Assert.IsTrue(vm.HasBackFace);
      Assert.IsTrue(vm.SelectedFace == MTGCard.CardSide.Front);
      Assert.AreEqual(frontUri, vm.SelectedFaceUri);

      vm.FlipCard();
      Assert.IsTrue(vm.SelectedFace == MTGCard.CardSide.Back);
      Assert.AreEqual(backUri, vm.SelectedFaceUri);
    }

    [TestMethod]
    public void FlipCardCommand_OneFaceTest()
    {
      var frontUri = "frontUri";

      var vm = new MTGCardViewModel(
        Mocker.MTGCardModelMocker.CreateMTGCardModel(
          frontFace: Mocker.MTGCardModelMocker.CreateCardFace(imageUri: frontUri)));

      Assert.IsFalse(vm.HasBackFace);
      Assert.IsTrue(vm.SelectedFace == MTGCard.CardSide.Front);
      Assert.AreEqual(frontUri, vm.SelectedFaceUri);

      vm.FlipCard();
      Assert.IsTrue(vm.SelectedFace == MTGCard.CardSide.Front);
      Assert.AreEqual(frontUri, vm.SelectedFaceUri);
    }

    [TestMethod]
    public void FlipCardCommandTest_CanExecute()
    {
      var oneFaceCard = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(
        frontFace: Mocker.MTGCardModelMocker.CreateCardFace()));
      var twoFaceCard = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(
        frontFace: Mocker.MTGCardModelMocker.CreateCardFace(),
        backFace: Mocker.MTGCardModelMocker.CreateCardFace()));

      Assert.IsFalse(oneFaceCard.FlipCardCommand.CanExecute(null));
      Assert.IsTrue(twoFaceCard.FlipCardCommand.CanExecute(null));
    }

    [TestMethod]
    public void IncreaseCountCommandTest()
    {
      var count = 3;
      var vm = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: count));

      vm.IncreaseCount();
      Assert.AreEqual(count + 1, vm.Model.Count);
    }

    [TestMethod]
    public void DecreaseCountCommand_CountOverOneTest()
    {
      var count = 3;
      var vm = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: count));

      vm.DecreaseCount();
      Assert.AreEqual(count - 1, vm.Model.Count);
    }

    [TestMethod]
    public void DecreaseCountCommand_CountEqualsOneTest()
    {
      var count = 1;
      var vm = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: count));

      vm.DecreaseCount();
      Assert.AreEqual(count, vm.Model.Count);
    }

    [TestMethod]
    public void DecreaseCountCommandTest_CanExecute()
    {
      var oneCountCard = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(
        frontFace: Mocker.MTGCardModelMocker.CreateCardFace(), count: 1));
      var twoCountCard = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(
        frontFace: Mocker.MTGCardModelMocker.CreateCardFace(), count: 2));

      Assert.IsFalse(oneCountCard.DecreaseCountCommand.CanExecute(null));
      Assert.IsTrue(twoCountCard.DecreaseCountCommand.CanExecute(null));
    }
  }
}
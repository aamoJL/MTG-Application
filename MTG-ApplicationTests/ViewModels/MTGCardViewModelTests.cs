using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.ViewModels;
using MTGApplication.Models;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.ViewModels;

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

    Assert.IsTrue(vm.HasBackFaceImage);
    Assert.IsTrue(vm.SelectedFaceSide == MTGCard.CardSide.Front);
    Assert.AreEqual(frontUri, vm.SelectedFaceUri);

    vm.FlipCard();
    Assert.IsTrue(vm.SelectedFaceSide == MTGCard.CardSide.Back);
    Assert.AreEqual(backUri, vm.SelectedFaceUri);
  }

  [TestMethod]
  public void FlipCardCommand_OneFaceTest()
  {
    var frontUri = "frontUri";

    var vm = new MTGCardViewModel(
      Mocker.MTGCardModelMocker.CreateMTGCardModel(
        frontFace: Mocker.MTGCardModelMocker.CreateCardFace(imageUri: frontUri)));

    Assert.IsFalse(vm.HasBackFaceImage);
    Assert.IsTrue(vm.SelectedFaceSide == MTGCard.CardSide.Front);
    Assert.AreEqual(frontUri, vm.SelectedFaceUri);

    vm.FlipCard();
    Assert.IsTrue(vm.SelectedFaceSide == MTGCard.CardSide.Front);
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
  public void IncreaseCountTest()
  {
    var count = 3;
    var vm = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: count));

    vm.Count++;
    Assert.AreEqual(count + 1, vm.Model.Count);
  }

  [TestMethod]
  public void DecreaseCount_CountOverOneTest()
  {
    var count = 3;
    var vm = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: count));

    vm.Count--;
    Assert.AreEqual(count - 1, vm.Model.Count);
  }

  [TestMethod]
  public void DecreaseCount_CountEqualsOneTest()
  {
    var count = 1;
    var vm = new MTGCardViewModel(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: count));

    vm.Count--;
    Assert.AreEqual(count, vm.Model.Count);
  }
}
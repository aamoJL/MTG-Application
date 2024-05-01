using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.Models;

[TestClass]
public class BasicCardViewModelTests
{
  [TestMethod("Should not be able to flip a card if the card has no back face")]
  public void SwitchFace_OneSidedCard_CanNotExecute()
  {
    var view = new BasicCardViewModel()
    {
      Model = Mocker.MTGCardModelMocker.CreateMTGCardModel(
        backFace: null)
    };

    Assert.IsFalse(view.SwitchFaceImageCommand.CanExecute(null));

    view.SwitchFaceImageCommand.Execute(null);

    Assert.AreEqual(view.Model.Info.FrontFace.ImageUri, view.SelectedFaceUri);
  }

  [TestMethod("Two faced card's face should change when switching the card's face")]
  public void SwitchFace_TwoSidedCard_FaceChanged()
  {
    var view = new BasicCardViewModel()
    {
      Model = Mocker.MTGCardModelMocker.CreateMTGCardModel(
        backFace: Mocker.MTGCardModelMocker.CreateCardFace())
    };

    Assert.IsTrue(view.SwitchFaceImageCommand.CanExecute(null));

    view.SwitchFaceImageCommand.Execute(null);

    Assert.AreEqual(view.Model.Info.BackFace?.ImageUri, view.SelectedFaceUri);
  }
}

using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.CollectionCard;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_Info()
  {
    var model = new MTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestCollectionCardViewModelFactory();
    var vm = factory.Build(model);

    Assert.AreEqual(model.Info, vm.Info);
  }

  [TestMethod]
  public void Init_IsOwned()
  {
    var model = new MTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestCollectionCardViewModelFactory();
    var vm = factory.Build(model);

    Assert.IsFalse(vm.IsOwned);
  }
}
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.CollectionCard;

[TestClass]
public class SwitchOwnership
{
  [TestMethod]
  public void Switch_ToOwned()
  {
    var model = new MTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestCollectionCardViewModelFactory()
    {
      IsOwned = false
    };
    var vm = factory.Build(model);

    vm.SwitchOwnershipCommand.Execute(null);

    Assert.IsTrue(vm.IsOwned);
  }

  [TestMethod]
  public void Switch_ToNotOwned()
  {
    var model = new MTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestCollectionCardViewModelFactory()
    {
      IsOwned = true
    };
    var vm = factory.Build(model);

    vm.SwitchOwnershipCommand.Execute(null);

    Assert.IsFalse(vm.IsOwned);
  }
}

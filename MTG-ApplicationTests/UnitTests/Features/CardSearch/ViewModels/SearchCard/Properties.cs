using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.CardSearch.ViewModels.SearchCard;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_Info()
  {
    var model = new MTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestSearchCardViewModelFactory();
    var vm = factory.Build(model);

    Assert.AreEqual(model.Info, vm.Info);
  }
}
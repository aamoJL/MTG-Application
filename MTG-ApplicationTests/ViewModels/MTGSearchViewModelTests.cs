using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.ViewModels
{
  [TestClass]
  public partial class MTGSearchViewModelTests
  {
    [TestMethod]
    public async Task SearchSubmitCommandTest()
    {
      MTGAPISearch<MTGCardViewModelSource, MTGCardViewModel> vm = new(new TestCardAPI()
      {
        ExpectedCards = new MTGCard[]
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Seconds"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
        }
      })
      {
        SearchQuery = "NotEmpty"
      };
      
      await vm.SearchSubmit();
      Assert.IsTrue(vm.TotalCardCount > 0);

      vm.SearchQuery = string.Empty;
      await vm.SearchSubmit();
      Assert.AreEqual(0, vm.TotalCardCount);
    }
  }
}

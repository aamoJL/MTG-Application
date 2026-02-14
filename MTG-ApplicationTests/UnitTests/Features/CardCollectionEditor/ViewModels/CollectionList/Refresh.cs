using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.CollectionList;

[TestClass]
public class Refresh
{
  [TestMethod]
  public async Task Refresh_Success_CardsAdded()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo())])
      }
    };
    var vm = factory.Build(model);

    await vm.RefreshCommand.ExecuteAsync(null);

    Assert.AreEqual(1, vm.QueryCards.TotalCardCount);
  }

  [TestMethod]
  public async Task Refresh_Exception_NotificationShown()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
    };
    var vm = factory.Build(model);

    await vm.RefreshCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}
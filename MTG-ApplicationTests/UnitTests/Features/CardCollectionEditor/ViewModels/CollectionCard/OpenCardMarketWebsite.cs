using MTGApplication.General.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.CollectionCard;

[TestClass]
public class OpenCardMarketWebsite
{
  [TestMethod]
  public async Task Open_Opened()
  {
    var opened = false;
    var model = new MTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestCollectionCardViewModelFactory()
    {
      NetworkService = new()
      {
        OpenAction = async (uri) =>
        {
          opened = true;
          return true;
        }
      }
    };
    var vm = factory.Build(model);

    await vm.OpenCardMarketWebsiteCommand.ExecuteAsync(null);

    Assert.IsTrue(opened);
  }

  [TestMethod]
  public async Task Open_Exception_NotificationShown()
  {
    var model = new MTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestCollectionCardViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      NetworkService = new()
      {
        OpenAction = async (uri) => throw new()
      }
    };
    var vm = factory.Build(model);

    await vm.OpenCardMarketWebsiteCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}

using MTGApplication.General.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardSearch.ViewModels.SearchCard;

[TestClass]
public class OpenAPIWebsite
{
  [TestMethod]
  public async Task Open_Opened()
  {
    var opened = false;
    var model = new MTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestSearchCardViewModelFactory()
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

    await vm.OpenAPIWebsiteCommand.ExecuteAsync(null);

    Assert.IsTrue(opened);
  }

  [TestMethod]
  public async Task Open_Exception_NotificationShown()
  {
    var model = new MTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestSearchCardViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      NetworkService = new()
      {
        OpenAction = async (uri) => throw new()
      }
    };
    var vm = factory.Build(model);

    await vm.OpenAPIWebsiteCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}

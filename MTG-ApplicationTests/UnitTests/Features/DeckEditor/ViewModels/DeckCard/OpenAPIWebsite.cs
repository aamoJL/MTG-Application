using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCard;

[TestClass]
public class OpenAPIWebsite
{
  [TestMethod]
  public async Task OpenWebsite()
  {
    var opened = false;
    var factory = new TestDeckCardViewModelFactory()
    {
      NetworkService = new()
      {
        OpenAction = async _ =>
        {
          opened = true;
          return await Task.FromResult(true);
        }
      }
    };
    var vm = factory.Build();

    await vm.OpenAPIWebsiteCommand.ExecuteAsync(null);

    Assert.IsTrue(opened);
  }

  [TestMethod]
  public async Task OpenWebsite_Exception_NotificationShown()
  {
    var factory = new TestDeckCardViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      NetworkService = new()
      {
        OpenAction = async _ => throw new()
      }
    };
    var vm = factory.Build();

    await vm.OpenAPIWebsiteCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}

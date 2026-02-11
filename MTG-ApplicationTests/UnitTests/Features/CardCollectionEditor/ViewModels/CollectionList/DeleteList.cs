using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.CollectionList;

[TestClass]
public class DeleteList
{
  [TestMethod]
  public async Task Delete_Cancel_Return()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      CollectionListConfirmers = new()
      {
        ConfirmListDelete = (_) => Task.FromResult(ConfirmationResult.Cancel)
      }
    };
    var vm = factory.Build(model);

    await vm.DeleteListCommand.ExecuteAsync(null);
  }

  [TestMethod]
  public async Task Delete_Success_OnDeleteCalled()
  {
    var called = false;
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      CollectionListConfirmers = new()
      {
        ConfirmListDelete = (_) => Task.FromResult(ConfirmationResult.Yes)
      },
      OnListDelete = async (_) =>
      {
        called = true;
        await Task.CompletedTask;
      }
    };
    var vm = factory.Build(model);

    await vm.DeleteListCommand.ExecuteAsync(null);

    Assert.IsTrue(called);
  }

  [TestMethod]
  public async Task Delete_Exception_NotificationShown()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      CollectionListConfirmers = new()
      {
        ConfirmListDelete = (_) => throw new()
      }
    };
    var vm = factory.Build(model);

    await vm.DeleteListCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}
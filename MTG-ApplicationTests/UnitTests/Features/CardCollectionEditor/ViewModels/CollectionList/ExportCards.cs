using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.CollectionList;

[TestClass]
public class ExportCards
{
  [TestMethod]
  public async Task Export_ScryfallIdsInConfirmation()
  {
    var id = Guid.NewGuid();
    var text = string.Empty;
    var model = new MTGCardCollectionList()
    {
      Cards = [new(MTGCardInfoMocker.MockInfo(scryfallId: id))]
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      CollectionListConfirmers = new()
      {
        ConfirmCardExport = async data =>
        {
          text = data.Data;
          return await Task.FromResult(data.Data);
        }
      }
    };
    var vm = factory.Build(model);

    await vm.ExportCardsCommand.ExecuteAsync(null);

    Assert.AreEqual(id.ToString(), text);
  }

  [TestMethod]
  public async Task Export_Cancel_Return()
  {
    var model = new MTGCardCollectionList()
    {
      Cards = [.. MTGCardMocker.Mock(5)]
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      CollectionListConfirmers = new()
      {
        ConfirmCardExport = async data =>
        {
          return await Task.FromResult<string>(null);
        }
      },
    };
    var vm = factory.Build(model);

    await vm.ExportCardsCommand.ExecuteAsync(null);

    Assert.IsNull(factory.Exporter.Result);
  }

  [TestMethod]
  public async Task Export_Empty_Return()
  {
    var model = new MTGCardCollectionList()
    {
      Cards = [.. MTGCardMocker.Mock(5)]
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      CollectionListConfirmers = new()
      {
        ConfirmCardExport = async data =>
        {
          return await Task.FromResult(string.Empty);
        }
      },
    };
    var vm = factory.Build(model);

    await vm.ExportCardsCommand.ExecuteAsync(null);

    Assert.IsNull(factory.Exporter.Result);
  }

  [TestMethod]
  public async Task Export_Success_Exported()
  {
    var model = new MTGCardCollectionList()
    {
      Cards = [.. MTGCardMocker.Mock(5)]
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      CollectionListConfirmers = new()
      {
        ConfirmCardExport = async data =>
        {
          return await Task.FromResult("Export text");
        }
      },
      Exporter = new() { Response = true }
    };
    var vm = factory.Build(model);

    await vm.ExportCardsCommand.ExecuteAsync(null);

    Assert.AreEqual("Export text", factory.Exporter.Result);
  }

  [TestMethod]
  public async Task Export_Success_NotificationShown()
  {
    var model = new MTGCardCollectionList()
    {
      Cards = [.. MTGCardMocker.Mock(5)]
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      CollectionListConfirmers = new()
      {
        ConfirmCardExport = async data => await Task.FromResult("Export text")
      },
      Exporter = new() { Response = true }
    };
    var vm = factory.Build(model);

    await vm.ExportCardsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Success, factory.Notifier);
  }

  [TestMethod]
  public async Task Export_Exception_NotificationShown()
  {
    var model = new MTGCardCollectionList()
    {
      Cards = [.. MTGCardMocker.Mock(5)]
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      CollectionListConfirmers = new()
      {
        ConfirmCardExport = async _ => throw new()
      },
      Exporter = new() { Response = true }
    };
    var vm = factory.Build(model);

    await vm.ExportCardsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}
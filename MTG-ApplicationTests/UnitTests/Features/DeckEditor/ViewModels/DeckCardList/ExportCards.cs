using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardList;

[TestClass]
public class ExportCards
{
  [TestMethod]
  public async Task Export_ById()
  {
    var factory = new TestDeckCardListViewModelFactory()
    {
      Notifier = new(),
      Model = [
        new(MTGCardInfoMocker.MockInfo(name: "1")),
        new(MTGCardInfoMocker.MockInfo(name: "2")),
        new(MTGCardInfoMocker.MockInfo(name: "3")),
      ],
      Confirmers = new()
      {
        ConfirmExport = async data => await Task.FromResult(data.Data)
      },
      Exporter = new() { Response = true }
    };
    var vm = factory.Build();

    await vm.ExportCardsCommand.ExecuteAsync("Id");

    Assert.AreEqual($"""
      {factory.Model[0].Info.ScryfallId}
      {factory.Model[1].Info.ScryfallId}
      {factory.Model[2].Info.ScryfallId}
      """, factory.Exporter.Result);
  }

  [TestMethod]
  public async Task Export_ByName()
  {
    var factory = new TestDeckCardListViewModelFactory()
    {
      Notifier = new(),
      Model = [
        new(MTGCardInfoMocker.MockInfo(name: "1")),
        new(MTGCardInfoMocker.MockInfo(name: "2")),
        new(MTGCardInfoMocker.MockInfo(name: "3")),
      ],
      Confirmers = new()
      {
        ConfirmExport = async data => await Task.FromResult(data.Data)
      },
      Exporter = new() { Response = true }
    };
    var vm = factory.Build();

    await vm.ExportCardsCommand.ExecuteAsync("Name");

    Assert.AreEqual($"""
      {factory.Model[0].Info.Name}
      {factory.Model[1].Info.Name}
      {factory.Model[2].Info.Name}
      """, factory.Exporter.Result);
  }

  [TestMethod]
  public async Task Export_Exception_NotificationShown()
  {
    var factory = new TestDeckCardListViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
    };
    var vm = factory.Build();

    await vm.ExportCardsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Error, factory.Notifier);
  }
}
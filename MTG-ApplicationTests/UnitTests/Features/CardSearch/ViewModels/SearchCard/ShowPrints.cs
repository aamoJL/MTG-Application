using MTGApplication.General.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardSearch.ViewModels.SearchCard;

[TestClass]
public class ShowPrints
{
  [TestMethod]
  public async Task Show_PrintsShown()
  {
    IEnumerable<MTGCard> prints = null;
    var model = new MTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestSearchCardViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([
          new(MTGCardInfoMocker.MockInfo()),
          new(MTGCardInfoMocker.MockInfo()),
          new(MTGCardInfoMocker.MockInfo()),
        ])
      },
      CardConfirmers = new()
      {
        ConfirmCardPrints = (data) => { prints = data.Data; return Task.CompletedTask; }
      }
    };
    var vm = factory.Build(model);

    await vm.ShowCardPrintsCommand.ExecuteAsync(null);

    Assert.AreEqual(3, prints.Count());
  }

  [TestMethod]
  public async Task Show_Exception_NotificationShown()
  {
    var model = new MTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestSearchCardViewModelFactory()
    {
      CardConfirmers = new()
      {
        ConfirmCardPrints = (data) => throw new()
      }
    };
    var vm = factory.Build(model);

    await vm.ShowCardPrintsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}

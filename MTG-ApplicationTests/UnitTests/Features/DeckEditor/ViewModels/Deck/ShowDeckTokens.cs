using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.Deck;

[TestClass]
public class ShowDeckTokens
{
  [TestMethod]
  public async Task Show_PrintsInConfirmation()
  {
    var prints = new MTGCardInfo[]
    {
      MTGCardInfoMocker.MockInfo(),
      MTGCardInfoMocker.MockInfo(),
      MTGCardInfoMocker.MockInfo(),
      MTGCardInfoMocker.MockInfo(),
    };
    var confirmationPrints = Array.Empty<MTGCardInfo>();
    var factory = new TestDeckViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([.. prints.Select(info => new CardImportResult.Card(info))])
      },
      Confirmers = new()
      {
        ConfirmDeckTokens = async data =>
        {
          confirmationPrints = [.. data.Data.Select(x => x.Info)];
          await Task.CompletedTask;
        }
      }
    };
    var vm = factory.Build();

    await vm.ShowDeckTokensCommand.ExecuteAsync(null);

    CollectionAssert.AreEqual(prints, confirmationPrints);
  }

  [TestMethod]
  public async Task Show_Exception_NotificationShown()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      Importer = new()
      {
        Result = TestMTGCardImporter.Failure()
      },
      Confirmers = new()
      {
        ConfirmDeckTokens = _ => throw new()
      }
    };
    var vm = factory.Build();

    await vm.ShowDeckTokensCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.Features.DeckEditor.Commanders.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class ImportTests
  {
    [TestMethod("Card should not change if the imported card is not legendary")]
    public async Task Import_NotLegendary_InvokedWithNull()
    {
      var import = new CardImportResult.Card(MTGCardInfoMocker.MockInfo(typeLine: "Creature"));
      DeckEditorMTGCard? result = null;
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter(), () => null)
      {
        OnChange = (card) => { result = card; }
      };

      JsonService.TrySerializeObject(import, out var json);

      await viewmodel.ImportCommanderCommand.ExecuteAsync(json);

      Assert.IsNull(result);
    }

    [TestMethod("Card should change if the imported card is legendary")]
    public async Task Import_Legendary_InvokedWithCard()
    {
      var import = new CardImportResult.Card(MTGCardInfoMocker.MockInfo(typeLine: "Legendary Creature"));
      DeckEditorMTGCard? result = null;
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter(), () => null)
      {
        OnChange = (card) => { result = card; }
      };

      JsonService.TrySerializeObject(import, out var json);

      await viewmodel.ImportCommanderCommand.ExecuteAsync(json);

      Assert.AreEqual(import.Info.Name, result?.Info.Name);
    }

    [TestMethod("Success notifications should be sent when the import was successfull")]
    public async Task Import_Success_SuccessNotificationSent()
    {
      var import = new CardImportResult.Card(MTGCardInfoMocker.MockInfo(typeLine: "Legendary Creature"));
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter(), () => null)
      {
        Notifier = new()
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      };

      JsonService.TrySerializeObject(import, out var json);

      await NotificationAssert.NotificationSent(CommanderNotifications.ImportSuccess, () => viewmodel.ImportCommanderCommand.ExecuteAsync(json));
    }

    [TestMethod("Error notification should be sent when the import fails")]
    public async Task Import_Failure_ErrorNotificationSent()
    {
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter(), () => null)
      {
        Notifier = new()
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      };

      await NotificationAssert.NotificationSent(CommanderNotifications.ImportError, () => viewmodel.ImportCommanderCommand.ExecuteAsync("Invalid json"));
    }

    [TestMethod("Error notification should be sent when the import fails")]
    public async Task Import_NotLegendary_LegendaryErrorNotificationSent()
    {
      var import = new CardImportResult.Card(MTGCardInfoMocker.MockInfo(typeLine: "Creature"));
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter(), () => null)
      {
        Notifier = new()
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      };

      JsonService.TrySerializeObject(import, out var json);

      await NotificationAssert.NotificationSent(CommanderNotifications.ImportNotLegendaryError, () => viewmodel.ImportCommanderCommand.ExecuteAsync(json));
    }
  }
}

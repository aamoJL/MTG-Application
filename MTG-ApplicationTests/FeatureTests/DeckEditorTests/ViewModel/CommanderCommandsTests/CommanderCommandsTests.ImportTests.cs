using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Commanders.Services;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests.DeckEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderCommandsTests
{
  [TestClass]
  public class ImportTests : DeckEditorViewModelTestsBase
  {
    [TestMethod("Card should not change if the imported card is not legendary")]
    public async Task Import_NotLegendary_InvokedWithNull()
    {
      var import = new CardImportResult.Card(MTGCardInfoMocker.MockInfo(typeLine: "Creature"));
      DeckEditorMTGCard result = null;
      var viewmodel = new CommanderCommands(new Mocker(_dependencies).MockVM(), CommanderCommands.CommanderType.Commander)
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
      DeckEditorMTGCard result = null;
      var viewmodel = new CommanderCommands(new Mocker(_dependencies).MockVM(), CommanderCommands.CommanderType.Commander)
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
      var notifier = new TestNotifier();
      var import = new CardImportResult.Card(MTGCardInfoMocker.MockInfo(typeLine: "Legendary Creature"));
      var viewmodel = new CommanderCommands(new Mocker(_dependencies).MockVM(), CommanderCommands.CommanderType.Commander)
      {
        Notifier = notifier
      };

      JsonService.TrySerializeObject(import, out var json);

      await viewmodel.ImportCommanderCommand.ExecuteAsync(json);

      NotificationAssert.NotificationSent(CommanderNotifications.ImportSuccess, notifier);
    }

    [TestMethod("Error notification should be sent when the import fails")]
    public async Task Import_Failure_ErrorNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new CommanderCommands(new Mocker(_dependencies).MockVM(), CommanderCommands.CommanderType.Commander)
      {
        Notifier = notifier
      };

      await viewmodel.ImportCommanderCommand.ExecuteAsync("Invalid json");

      NotificationAssert.NotificationSent(CommanderNotifications.ImportError, notifier);
    }

    [TestMethod("Error notification should be sent when the import fails")]
    public async Task Import_NotLegendary_LegendaryErrorNotificationSent()
    {
      var notifier = new TestNotifier();
      var import = new CardImportResult.Card(MTGCardInfoMocker.MockInfo(typeLine: "Creature"));
      var viewmodel = new CommanderCommands(new Mocker(_dependencies).MockVM(), CommanderCommands.CommanderType.Commander)
      {
        Notifier = notifier
      };

      JsonService.TrySerializeObject(import, out var json);

      await viewmodel.ImportCommanderCommand.ExecuteAsync(json);

      NotificationAssert.NotificationSent(CommanderNotifications.ImportNotLegendaryError, notifier);
    }
  }
}

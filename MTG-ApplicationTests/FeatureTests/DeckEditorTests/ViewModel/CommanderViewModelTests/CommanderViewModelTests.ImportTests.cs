using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Services.IOService;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class ImportTests
  {
    [TestMethod("Card should not change if the imported card is not legendary")]
    public async Task Import_NotLegendary_CardIsNull()
    {
      var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(typeLine: "Creature");
      var viewmodel = new CommanderViewModel(new TestCardAPI());

      JsonService.TrySerializeObject(card, out var json);

      await viewmodel.ImportCommand.ExecuteAsync(json);

      Assert.IsNull(viewmodel.Card);
    }

    [TestMethod("Card should change if the imported card is legendary")]
    public async Task Import_Legendary_CardIsCard()
    {
      var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(typeLine: "Legendary Creature");
      var viewmodel = new CommanderViewModel(new TestCardAPI());

      JsonService.TrySerializeObject(card, out var json);

      await viewmodel.ImportCommand.ExecuteAsync(json);

      Assert.AreEqual(card.Info.Name, viewmodel.Card.Info.Name);
    }

    [TestMethod("Success notifications should be sent when the import was successfull")]
    public async Task Import_Success_SuccessNotificationSent()
    {
      var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(typeLine: "Legendary Creature");
      var viewmodel = new CommanderViewModel(new TestCardAPI())
      {
        Notifier = new()
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      };

      JsonService.TrySerializeObject(card, out var json);

      await NotificationAssert.NotificationSent(CommanderViewModelNotifications.ImportSuccess, () => viewmodel.ImportCommand.ExecuteAsync(json));
    }

    [TestMethod("Error notification should be sent when the import fails")]
    public async Task Import_Failure_ErrorNotificationSent()
    {
      var viewmodel = new CommanderViewModel(new TestCardAPI())
      {
        Notifier = new()
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      };

      await NotificationAssert.NotificationSent(CommanderViewModelNotifications.ImportError, () => viewmodel.ImportCommand.ExecuteAsync("Invalid json"));
    }

    [TestMethod("Error notification should be sent when the import fails")]
    public async Task Import_NotLegendary_LegendaryErrorNotificationSent()
    {
      var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(typeLine: "Creature");
      var viewmodel = new CommanderViewModel(new TestCardAPI())
      {
        Notifier = new()
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      };

      JsonService.TrySerializeObject(card, out var json);

      await NotificationAssert.NotificationSent(CommanderViewModelNotifications.ImportNotLegendaryError, () => viewmodel.ImportCommand.ExecuteAsync(json));
    }
  }
}

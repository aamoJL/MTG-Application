using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionListViewModelTests
{
  [TestClass]
  public class ExportCardsTests : CardCollectionListViewModelTestsBase, ICanExecuteCommandAsyncTests
  {
    [TestMethod("Should not be able to execute if the list has no name")]
    public async Task InvalidState_CanNotExecute()
    {
      var viewmodel = await new Mocker(_dependencies).MockVM();

      Assert.IsFalse(viewmodel.ExportCardsCommand.CanExecute(null));
    }

    [TestMethod("Should be able to execute if the list has a name")]
    public async Task ValidState_CanExecute()
    {
      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList
      }.MockVM();

      Assert.IsTrue(viewmodel.ExportCardsCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task ExportCards_ExportConfirmationShown()
    {
      var confirmer = new TestConfirmer<string, string>();
      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList,
        Confirmers = new()
        {
          ExportCardsConfirmer = confirmer
        }
      }.MockVM();

      await viewmodel.ExportCardsCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task ExportCards_Cancel_NoCopy()
    {
      var clipboard = new TestClipboardService();
      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList,
        Confirmers = new()
        {
          ExportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string>(null),
          }
        },
        ClipboardService = clipboard
      }.MockVM();

      await viewmodel.ExportCardsCommand.ExecuteAsync(null);

      Assert.IsNull(clipboard.Content);
    }

    [TestMethod]
    public async Task ExportCards_Success_ContentCopied()
    {
      var exportText = "Export";
      var clipboard = new TestClipboardService();
      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList,
        Confirmers = new()
        {
          ExportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string>(exportText),
          }
        },
        ClipboardService = clipboard
      }.MockVM();

      await viewmodel.ExportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(exportText, clipboard.Content);
    }

    [TestMethod("Export confirmation data should be selected list's cards' ids")]
    public async Task ExportCards_ExportDataIsIds()
    {
      var exportData = string.Empty;
      var clipboard = new TestClipboardService();
      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList,
        Confirmers = new()
        {
          ExportCardsConfirmer = new()
          {
            OnConfirm = async msg =>
            {
              exportData = msg.Data;
              return await Task.FromResult<string>(msg.Data);
            },
          }
        },
        ClipboardService = clipboard
      }.MockVM();

      await viewmodel.ExportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(
        exportData,
        string.Join(Environment.NewLine, viewmodel.CollectionList.Cards.Select(x => x.Info.ScryfallId)));
    }

    [TestMethod]
    public async Task ExportCards_Success_InfoNotificationShown()
    {
      var exportText = "Export";
      var clipboard = new TestClipboardService();
      var notifier = new TestNotifier();
      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList,
        Confirmers = new()
        {
          ExportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string>(exportText),
          }
        },
        ClipboardService = clipboard,
        Notifier = notifier
      }.MockVM();

      await viewmodel.ExportCardsCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Info, notifier);
    }
  }
}

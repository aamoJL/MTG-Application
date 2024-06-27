using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests;

public partial class CardListViewModelTests
{
  [TestClass]
  public class ExportCardTests : ICanExecuteWithParameterCommandTests
  {
    [TestMethod("Should be able to execute with Name or Id parameters")]
    public void ValidParameter_CanExecute()
    {
      var viewmodel = new CardListViewModel(new TestMTGCardImporter());

      Assert.IsTrue(viewmodel.ExportCardsCommand.CanExecute("Name"));
      Assert.IsTrue(viewmodel.ExportCardsCommand.CanExecute("Id"));
    }

    [TestMethod("Should not be able to execute if parameter is not Name or Id")]
    public void InvalidParameter_CanNotExecute()
    {
      var viewmodel = new CardListViewModel(new TestMTGCardImporter());

      Assert.IsFalse(viewmodel.ExportCardsCommand.CanExecute("Invalid property name"));
    }

    [TestMethod]
    public async Task Execute_ByName_ConfirmationShown()
    {
      var cards = new DeckEditorMTGCard[]
      {
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
      };

      var viewmodel = new CardListViewModel(new TestMTGCardImporter())
      {
        Cards = new(cards),
        Confirmers = new()
        {
          ExportConfirmer = new TestExceptionConfirmer<string, string>()
        }
      };

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ExportCardsCommand.ExecuteAsync("Name"));
    }

    [TestMethod]
    public async Task Execute_ById_ConfirmationShown()
    {
      var cards = new DeckEditorMTGCard[]
      {
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
      };

      var viewmodel = new CardListViewModel(new TestMTGCardImporter())
      {
        Cards = new(cards),
        Confirmers = new()
        {
          ExportConfirmer = new TestExceptionConfirmer<string, string>()
        }
      };

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ExportCardsCommand.ExecuteAsync("Id"));
    }

    [TestMethod]
    public async Task Execute_ByName_ExportStringShown()
    {
      string? exportText = null;

      var cards = new DeckEditorMTGCard[]
      {
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
      };

      var viewmodel = new CardListViewModel(new TestMTGCardImporter())
      {
        Cards = new(cards),
        Confirmers = new()
        {
          ExportConfirmer = new() { OnConfirm = async (msg) => { exportText = msg.Data; return await Task.FromResult<string?>(default); } }
        }
      };

      await viewmodel.ExportCardsCommand.ExecuteAsync("Name");

      Assert.AreEqual(string.Join(Environment.NewLine, cards.Select(x => x.Info.Name)), exportText);
    }

    [TestMethod]
    public async Task Execute_ById_ExportStringShown()
    {
      string? exportText = null;

      var cards = new DeckEditorMTGCard[]
      {
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
      };

      var viewmodel = new CardListViewModel(new TestMTGCardImporter())
      {
        Cards = new(cards),
        Confirmers = new()
        {
          ExportConfirmer = new() { OnConfirm = async (msg) => { exportText = msg.Data; return await Task.FromResult<string?>(default); } }
        }
      };

      await viewmodel.ExportCardsCommand.ExecuteAsync("Id");

      Assert.AreEqual(string.Join(Environment.NewLine, cards.Select(x => x.Info.ScryfallId)), exportText);
    }

    [TestMethod]
    public async Task Execute_CopyToClipboard_CopiedToClipboard()
    {
      string? exportText = null;

      var cards = new DeckEditorMTGCard[]
      {
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
      };
      var clipboard = new TestClipboardService();

      var viewmodel = new CardListViewModel(new TestMTGCardImporter())
      {
        Cards = new(cards),
        Confirmers = new()
        {
          ExportConfirmer = new() { OnConfirm = async (msg) => { exportText = msg.Data; return await Task.FromResult(exportText); } }
        },
        ClipboardService = clipboard
      };

      await viewmodel.ExportCardsCommand.ExecuteAsync("Name");

      Assert.AreEqual(string.Join(Environment.NewLine, cards.Select(x => x.Info.Name)), clipboard.Content);
    }

    [TestMethod]
    public async Task Execute_CopyToClipboard_InfoNotificationSent()
    {
      var cards = new DeckEditorMTGCard[]
      {
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
      DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
      };
      var clipboard = new TestClipboardService();

      var viewmodel = new CardListViewModel(new TestMTGCardImporter())
      {
        Cards = new(cards),
        Confirmers = new()
        {
          ExportConfirmer = new() { OnConfirm = async (msg) => { return await Task.FromResult(msg.Data); } }
        },
        ClipboardService = clipboard,
        Notifier = new() { OnNotify = (arg) => throw new NotificationException(arg) }
      };

      await NotificationAssert.NotificationSent(NotificationType.Info, () => viewmodel.ExportCardsCommand.ExecuteAsync("Name"));
    }
  }
}

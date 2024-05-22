using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Models.Card;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests;

public partial class CardListViewModelTests
{
  [TestClass]
  public class ExportCardTests
  {
    [TestMethod]
    public void ByName_CanExecute()
    {
      var viewmodel = new CardListViewModel(new TestCardAPI());

      Assert.IsTrue(viewmodel.ExportCommand.CanExecute("Name"));
    }

    [TestMethod]
    public void ById_CanExecute()
    {
      var viewmodel = new CardListViewModel(new TestCardAPI());

      Assert.IsTrue(viewmodel.ExportCommand.CanExecute("Id"));
    }

    [TestMethod]
    public void ByInvalidProperty_CanExecute()
    {
      var viewmodel = new CardListViewModel(new TestCardAPI());

      Assert.IsFalse(viewmodel.ExportCommand.CanExecute("Invalid property name"));
    }

    [TestMethod]
    public async Task Execute_ByName_ConfirmationShown()
    {
      var cards = new MTGCard[]
      {
      MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
      MTGCardModelMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
      MTGCardModelMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
      };

      var viewmodel = new CardListViewModel(new TestCardAPI())
      {
        Cards = new(cards),
        Confirmers = new()
        {
          ExportConfirmer = new TestExceptionConfirmer<string, string>()
        }
      };

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ExportCommand.ExecuteAsync("Name"));
    }

    [TestMethod]
    public async Task Execute_ById_ConfirmationShown()
    {
      var cards = new MTGCard[]
      {
      MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
      MTGCardModelMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
      MTGCardModelMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
      };

      var viewmodel = new CardListViewModel(new TestCardAPI())
      {
        Cards = new(cards),
        Confirmers = new()
        {
          ExportConfirmer = new TestExceptionConfirmer<string, string>()
        }
      };

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ExportCommand.ExecuteAsync("Id"));
    }

    [TestMethod]
    public async Task Execute_ByName_ExportStringShown()
    {
      string? exportText = null;

      var cards = new MTGCard[]
      {
      MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
      MTGCardModelMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
      MTGCardModelMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
      };

      var viewmodel = new CardListViewModel(new TestCardAPI())
      {
        Cards = new(cards),
        Confirmers = new()
        {
          ExportConfirmer = new() { OnConfirm = async (msg) => { exportText = msg.Data; return await Task.FromResult<string?>(default); } }
        }
      };

      await viewmodel.ExportCommand.ExecuteAsync("Name");

      Assert.AreEqual(string.Join(Environment.NewLine, cards.Select(x => x.Info.Name)), exportText);
    }

    [TestMethod]
    public async Task Execute_ById_ExportStringShown()
    {
      string? exportText = null;

      var cards = new MTGCard[]
      {
      MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
      MTGCardModelMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
      MTGCardModelMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
      };

      var viewmodel = new CardListViewModel(new TestCardAPI())
      {
        Cards = new(cards),
        Confirmers = new()
        {
          ExportConfirmer = new() { OnConfirm = async (msg) => { exportText = msg.Data; return await Task.FromResult<string?>(default); } }
        }
      };

      await viewmodel.ExportCommand.ExecuteAsync("Id");

      Assert.AreEqual(string.Join(Environment.NewLine, cards.Select(x => x.Info.ScryfallId)), exportText);
    }

    [TestMethod]
    public async Task Execute_CopyToClipboard_CopiedToClipboard()
    {
      string? exportText = null;

      var cards = new MTGCard[]
      {
      MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
      MTGCardModelMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
      MTGCardModelMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
      };
      var clipboard = new TestClipboardService();

      var viewmodel = new CardListViewModel(new TestCardAPI())
      {
        Cards = new(cards),
        Confirmers = new()
        {
          ExportConfirmer = new() { OnConfirm = async (msg) => { exportText = msg.Data; return await Task.FromResult(exportText); } }
        },
        ClipboardService = clipboard
      };

      await viewmodel.ExportCommand.ExecuteAsync("Name");

      Assert.AreEqual(string.Join(Environment.NewLine, cards.Select(x => x.Info.Name)), clipboard.Content);
    }

    [TestMethod]
    public async Task Execute_CopyToClipboard_InfoNotificationSent()
    {
      var cards = new MTGCard[]
      {
      MTGCardModelMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
      MTGCardModelMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
      MTGCardModelMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
      };
      var clipboard = new TestClipboardService();

      var viewmodel = new CardListViewModel(new TestCardAPI())
      {
        Cards = new(cards),
        Confirmers = new()
        {
          ExportConfirmer = new() { OnConfirm = async (msg) => { return await Task.FromResult(msg.Data); } }
        },
        ClipboardService = clipboard,
        Notifier = new() { OnNotify = (arg) => throw new NotificationException(arg) }
      };

      await NotificationAssert.NotificationSent(NotificationType.Info, () => viewmodel.ExportCommand.ExecuteAsync("Name"));
    }
  }
}

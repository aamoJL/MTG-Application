namespace MTGApplicationTests.UnitTests.Features.DeckEditor.CardList.UseCases.NormalList;

[TestClass]
public class ExportCards
{
  //[TestMethod(DisplayName = "Should be able to execute with Name or Id parameters")]
  //public void ValidParameter_CanExecute()
  //{
  //  var viewmodel = new CardListViewModel([], new TestMTGCardImporter_old());

  //  Assert.IsTrue(viewmodel.ExportCardsCommand.CanExecute("Name"));
  //  Assert.IsTrue(viewmodel.ExportCardsCommand.CanExecute("Id"));
  //}

  //[TestMethod(DisplayName = "Should not be able to execute if parameter is not Name or Id")]
  //public void InvalidParameter_CanNotExecute()
  //{
  //  var viewmodel = new CardListViewModel([], new TestMTGCardImporter_old());

  //  Assert.IsFalse(viewmodel.ExportCardsCommand.CanExecute("Invalid property name"));
  //}

  //[TestMethod]
  //public async Task Execute_ByName_ConfirmationShown()
  //{
  //  var confirmer = new TestConfirmer<string, string>();
  //  var cards = new DeckEditorMTGCard[]
  //  {
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
  //  };

  //  var viewmodel = new CardListViewModel([.. cards], new TestMTGCardImporter_old())
  //  {
  //    Confirmers = new()
  //    {
  //      ExportConfirmer = confirmer
  //    }
  //  };

  //  await viewmodel.ExportCardsCommand.ExecuteAsync("Name");

  //  ConfirmationAssert.ConfirmationShown(confirmer);
  //}

  //[TestMethod]
  //public async Task Execute_ById_ConfirmationShown()
  //{
  //  var confirmer = new TestConfirmer<string, string>();
  //  var cards = new DeckEditorMTGCard[]
  //  {
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
  //  };

  //  var viewmodel = new CardListViewModel([.. cards], new TestMTGCardImporter_old())
  //  {
  //    Confirmers = new()
  //    {
  //      ExportConfirmer = confirmer
  //    }
  //  };

  //  await viewmodel.ExportCardsCommand.ExecuteAsync("Id");

  //  ConfirmationAssert.ConfirmationShown(confirmer);
  //}

  //[TestMethod]
  //public async Task Execute_ByName_ExportStringShown()
  //{
  //  string exportText = null;

  //  var cards = new DeckEditorMTGCard[]
  //  {
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
  //  };

  //  var viewmodel = new CardListViewModel([.. cards], new TestMTGCardImporter_old())
  //  {
  //    Confirmers = new()
  //    {
  //      ExportConfirmer = new() { OnConfirm = async (msg) => { exportText = msg.Data; return await Task.FromResult<string>(default); } }
  //    }
  //  };

  //  await viewmodel.ExportCardsCommand.ExecuteAsync("Name");

  //  Assert.AreEqual(string.Join(Environment.NewLine, cards.Select(x => x.Info.Name)), exportText);
  //}

  //[TestMethod]
  //public async Task Execute_ById_ExportStringShown()
  //{
  //  string exportText = null;

  //  var cards = new DeckEditorMTGCard[]
  //  {
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
  //  };

  //  var viewmodel = new CardListViewModel([.. cards], new TestMTGCardImporter_old())
  //  {
  //    Confirmers = new()
  //    {
  //      ExportConfirmer = new() { OnConfirm = async (msg) => { exportText = msg.Data; return await Task.FromResult<string>(default); } }
  //    }
  //  };

  //  await viewmodel.ExportCardsCommand.ExecuteAsync("Id");

  //  Assert.AreEqual(string.Join(Environment.NewLine, cards.Select(x => x.Info.ScryfallId)), exportText);
  //}

  //[TestMethod]
  //public async Task Execute_CopyToClipboard_CopiedToClipboard()
  //{
  //  string exportText = null;

  //  var cards = new DeckEditorMTGCard[]
  //  {
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
  //  };
  //  var clipboard = new();

  //  var viewmodel = new CardListViewModel([.. cards], new TestMTGCardImporter_old())
  //  {
  //    ClipboardService = clipboard,
  //    Confirmers = new()
  //    {
  //      ExportConfirmer = new() { OnConfirm = async (msg) => { exportText = msg.Data; return await Task.FromResult(exportText); } }
  //    }
  //  };

  //  await viewmodel.ExportCardsCommand.ExecuteAsync("Name");

  //  Assert.AreEqual(string.Join(Environment.NewLine, cards.Select(x => x.Info.Name)), clipboard.Content);
  //}

  //[TestMethod]
  //public async Task Execute_CopyToClipboard_InfoNotificationSent()
  //{
  //  var cards = new DeckEditorMTGCard[]
  //  {
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First", scryfallId: Guid.NewGuid()),
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Second", scryfallId: Guid.NewGuid()),
  //    DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Third", scryfallId: Guid.NewGuid()),
  //  };
  //  var clipboard = new TestClipboardService();
  //  var notifier = new TestNotifier();

  //  var viewmodel = new CardListViewModel([.. cards], new TestMTGCardImporter_old())
  //  {
  //    ClipboardService = clipboard,
  //    Notifier = notifier,
  //    Confirmers = new()
  //    {
  //      ExportConfirmer = new() { OnConfirm = async (msg) => { return await Task.FromResult(msg.Data); } }
  //    }
  //  };

  //  await viewmodel.ExportCardsCommand.ExecuteAsync("Name");

  //  NotificationAssert.NotificationSent(NotificationType.Info, notifier);
  //}
}

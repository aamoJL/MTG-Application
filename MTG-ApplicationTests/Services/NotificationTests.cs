using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using MTGApplicationTests.API;
using static MTGApplication.Services.Notifications;
using static MTGApplicationTests.Database.InMemoryMTGDeckRepositoryTests;
using static MTGApplicationTests.Services.TestDialogService;

namespace MTGApplicationTests.Services
{
  [TestClass]
  public class NotificationTests
  {
    public class NotificationAsserter : IDisposable
    {
      public bool WillFail { get; init; } // Assert will fail if the notification has been invoked
      public NotificationType ExpectedNotification { get; init; }

      public NotificationAsserter(NotificationType expectedNotification)
      {
        ExpectedNotification = expectedNotification;
        OnNotification += Notifications_OnNotification;
      }

      private void Notifications_OnNotification(object? sender, NotificationEventArgs e)
      {
        if(WillFail) { Assert.Fail(); }
        Assert.AreEqual(ExpectedNotification, e.Type);
      }

      public void Dispose()
      {
        OnNotification -= Notifications_OnNotification;
        GC.SuppressFinalize(this);
      }
    }

    [TestMethod]
    public void ClipboardCopyTest()
    {
      string text = "test";

      using var asserter = new NotificationAsserter(NotificationType.Info);

      var clipboard = new TestIO.TestClipboard();
      clipboard.Copy(text);

      Assert.AreEqual(text, clipboard.Content as string);
    }
    
    #region Cardlist Methods
    [TestMethod]
    public async Task CardImportTest_AllFound()
    {
      MTGCard[] expectedCards = new MTGCard[]
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
      };

      using var asserter = new NotificationAsserter(NotificationType.Success);

      var cardlist = new DeckBuilderViewModel.Cardlist(new(), 
        MTGApplication.Enums.CardlistType.Deck, new DeckBuilderViewModel.DeckBuilderViewDialogs()
        {
          ImportDialog = new TestTextAreaDialog(Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary, nameof(expectedCards)),
        },new TestCardAPI(expectedCards, 0));

      await cardlist.ImportToCardlistDialog();
    }

    [TestMethod]
    public async Task CardImportTest_SomeFound()
    {
      MTGCard[] expectedCards = new MTGCard[]
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
      };

      using var asserter = new NotificationAsserter(NotificationType.Warning);

      var cardlist = new DeckBuilderViewModel.Cardlist(new(),
        MTGApplication.Enums.CardlistType.Deck, new DeckBuilderViewModel.DeckBuilderViewDialogs()
        {
          ImportDialog = new TestTextAreaDialog(ContentDialogResult.Primary, nameof(expectedCards)),
        }, new TestCardAPI(expectedCards, 3));

      await cardlist.ImportToCardlistDialog();
    }

    [TestMethod]
    public async Task CardImportTest_NotFound()
    {
      using var asserter = new NotificationAsserter(NotificationType.Error);

      var cardlist = new DeckBuilderViewModel.Cardlist(new(),
        MTGApplication.Enums.CardlistType.Deck, new DeckBuilderViewModel.DeckBuilderViewDialogs()
        {
          ImportDialog = new TestTextAreaDialog(ContentDialogResult.Primary, "notFound"),
        }, new TestCardAPI(null, 3));

      await cardlist.ImportToCardlistDialog();
    }

    [TestMethod]
    public async Task CardImportTest_Empty()
    {
      using var asserter = new NotificationAsserter(NotificationType.Error);

      var cardlist = new DeckBuilderViewModel.Cardlist(new(),
        MTGApplication.Enums.CardlistType.Deck, new DeckBuilderViewModel.DeckBuilderViewDialogs()
        {
          ImportDialog = new TestTextAreaDialog(ContentDialogResult.Primary, string.Empty),
        }, new TestCardAPI(null, 0));

      await cardlist.ImportToCardlistDialog();
    }

    [TestMethod]
    public async Task CardImportTest_Canceled()
    {
      MTGCard[] expectedCards = new MTGCard[]
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
      };

      using var asserter = new NotificationAsserter(NotificationType.Success) { WillFail = true };

      var cardlist = new DeckBuilderViewModel.Cardlist(new(),
        MTGApplication.Enums.CardlistType.Deck, new DeckBuilderViewModel.DeckBuilderViewDialogs()
        {
          ImportDialog = new TestTextAreaDialog(ContentDialogResult.None, "notFound"), // Cancel dialog
        }, new TestCardAPI(expectedCards, 3));

      await cardlist.ImportToCardlistDialog();
    }

    [TestMethod]
    public async Task CardExportTest_Copy()
    {
      var cardlist = new DeckBuilderViewModel.Cardlist(new(),
        MTGApplication.Enums.CardlistType.Deck, new DeckBuilderViewModel.DeckBuilderViewDialogs()
        {
          ExportDialog = new TestTextAreaDialog(ContentDialogResult.Primary, "Text"),
        }, new TestCardAPI(), clipboardService: new TestIO.TestClipboard());

      using var asserter = new NotificationAsserter(NotificationType.Info);

      await cardlist.ExportDeckCardsDialog();
    }

    [TestMethod]
    public async Task CardExportTest_Empty()
    {
      var cardlist = new DeckBuilderViewModel.Cardlist(new(),
        MTGApplication.Enums.CardlistType.Deck, new DeckBuilderViewModel.DeckBuilderViewDialogs()
        {
          ExportDialog = new TestTextAreaDialog(ContentDialogResult.Primary, string.Empty),
        }, new TestCardAPI(), clipboardService: new TestIO.TestClipboard());

      using var asserter = new NotificationAsserter(NotificationType.Info);

      await cardlist.ExportDeckCardsDialog();
    }

    [TestMethod]
    public async Task CardExportTest_Canceled()
    {
      var cardlist = new DeckBuilderViewModel.Cardlist(new(),
        MTGApplication.Enums.CardlistType.Deck, new DeckBuilderViewModel.DeckBuilderViewDialogs()
        {
          ExportDialog = new TestTextAreaDialog(ContentDialogResult.None, null), // Cancel dialog
        }, new TestCardAPI(), clipboardService: new TestIO.TestClipboard());

      using var asserter = new NotificationAsserter(NotificationType.Error) { WillFail = true };

      await cardlist.ExportDeckCardsDialog();
    }
    #endregion

    #region Deck Methods
    [TestMethod]
    public async Task DeckSaveTest_Success()
    {
      var deckName = "First";

      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs() // Override dialogs
      {
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, deckName), // Save deck
      });

      using var asserter = new NotificationAsserter(NotificationType.Success);

      await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
      await vm.SaveDeckDialog();
    }

    [TestMethod]
    public async Task DeckSaveTest_Fail()
    {
      var deckName = "First";

      using TestInMemoryMTGDeckRepository repo = new() { WillFail = true };
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs() // Override dialogs
      {
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, deckName), // Save deck
      });

      using var asserter = new NotificationAsserter(NotificationType.Error);

      await vm.DeckCards.AddToCardlist(Mocker.MTGCardModelMocker.CreateMTGCardModel());
      await vm.SaveDeckDialog();
    }

    [TestMethod]
    public async Task DeckLoadingTest_Success()
    {
      var deckName = "First";

      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs() // Override dialogs
      {
        LoadDialog = new TestComboBoxDialog(ContentDialogResult.Primary, deckName), // Load deck
      });

      // Add deck with the same name to the database
      var dbDeck = new MTGCardDeck()
      {
        Name = deckName,
        DeckCards = new()
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second")
        }
      };
      await repo.Add(dbDeck);

      using var asserter = new NotificationAsserter(NotificationType.Success);

      await vm.LoadDeckDialog();
    }

    [TestMethod]
    public async Task DeckLoadingTest_Fail()
    {
      var deckName = "First";

      using TestInMemoryMTGDeckRepository repo = new() { WillFail = true };
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs() // Override dialogs
      {
        LoadDialog = new TestComboBoxDialog(ContentDialogResult.Primary, deckName), // Load deck
      });

      // Add deck with the same name to the database
      var dbDeck = new MTGCardDeck()
      {
        Name = deckName,
        DeckCards = new()
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second")
        }
      };
      await repo.Add(dbDeck);

      using var asserter = new NotificationAsserter(NotificationType.Error);

      await vm.LoadDeckDialog();
    }

    [TestMethod]
    public async Task DeckLoadingTest_Cancel()
    {
      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs() // Override dialogs
      {
        LoadDialog = new TestComboBoxDialog(ContentDialogResult.None, null), // Cancel
      });

      using var asserter = new NotificationAsserter(NotificationType.Success) { WillFail = true };

      await vm.LoadDeckDialog();
    }

    [TestMethod]
    public async Task DeckDeletionTest_Success()
    {
      var deckName = "First";

      using TestInMemoryMTGDeckRepository repo = new();
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, deckName), // Save deck
        DeleteDialog = new TestConfirmationDialog(ContentDialogResult.Primary),
      });

      using var asserter = new NotificationAsserter(NotificationType.Success);

      await vm.SaveDeckDialog();
      await vm.DeleteDeckDialog();
    }

    [TestMethod]
    public async Task DeckDeletionTest_Fail()
    {
      var deckName = "First";

      using TestInMemoryMTGDeckRepository repo = new() { WillFail = true };
      DeckBuilderViewModel vm = new(null, repo, dialogs: new DeckBuilderViewModel.DeckBuilderViewDialogs()
      {
        SaveDialog = new TestTextBoxDialog(ContentDialogResult.Primary, deckName), // Save deck
        DeleteDialog = new TestConfirmationDialog(ContentDialogResult.Primary),
      });

      using var asserter = new NotificationAsserter(NotificationType.Error);

      await vm.SaveDeckDialog();
      await vm.DeleteDeckDialog();
    }
    #endregion
  }
}

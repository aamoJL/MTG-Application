﻿using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using MTGApplicationTests.API;
using static MTGApplication.Services.NotificationService;
using static MTGApplicationTests.Database.InMemoryMTGDeckRepositoryTests;
using static MTGApplicationTests.ViewModels.DeckBuilderViewModelTests;
using static MTGApplicationTests.ViewModels.DeckCardlistViewModelTests;

namespace MTGApplicationTests.Services;

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
      if (WillFail) { Assert.Fail(); }
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
    var text = "test";

    using var asserter = new NotificationAsserter(NotificationType.Info);

    var clipboard = new TestIO.TestClipboard();
    clipboard.Copy(text);

    Assert.AreEqual(text, clipboard.Content as string);
  }

  #region Cardlist Methods
  [TestMethod]
  public async Task CardImportTest_AllFound()
  {
    var expectedCards = new MTGCard[]
    {
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
    };

    using var asserter = new NotificationAsserter(NotificationType.Success);

    var cardlist = new DeckCardlistViewModel(new(), new TestCardAPI(expectedCards, 0));
    cardlist.Dialogs = new TestDeckCardlistViewDialogs(cardlist)
    {
      ImportDialog = new() { Values = nameof(expectedCards) },
    };

    await cardlist.ImportToCardlistDialog();
  }

  [TestMethod]
  public async Task CardImportTest_SomeFound()
  {
    var expectedCards = new MTGCard[]
    {
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
    };

    using var asserter = new NotificationAsserter(NotificationType.Warning);

    var cardlist = new DeckCardlistViewModel(new(), new TestCardAPI(expectedCards, 3));
    cardlist.Dialogs = new TestDeckCardlistViewDialogs(cardlist)
    {
      ImportDialog = new() { Values = nameof(expectedCards) },
    };

    await cardlist.ImportToCardlistDialog();
  }

  [TestMethod]
  public async Task CardImportTest_NotFound()
  {
    using var asserter = new NotificationAsserter(NotificationType.Error);

    var cardlist = new DeckCardlistViewModel(new(), new TestCardAPI(null, 3));
    cardlist.Dialogs = new TestDeckCardlistViewDialogs(cardlist)
    {
      ImportDialog = new() { Values = "notFound" },
    };

    await cardlist.ImportToCardlistDialog();
  }

  [TestMethod]
  public async Task CardImportTest_Empty()
  {
    using var asserter = new NotificationAsserter(NotificationType.Error);

    var cardlist = new DeckCardlistViewModel(new(), new TestCardAPI(null, 0));
    cardlist.Dialogs = new TestDeckCardlistViewDialogs(cardlist)
    {
      ImportDialog = new() { Values = string.Empty },
    };

    await cardlist.ImportToCardlistDialog();
  }

  [TestMethod]
  public async Task CardImportTest_Canceled()
  {
    var expectedCards = new MTGCard[]
    {
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Third"),
    };

    using var asserter = new NotificationAsserter(NotificationType.Success) { WillFail = true };

    var cardlist = new DeckCardlistViewModel(new(), new TestCardAPI(expectedCards, 3));
    cardlist.Dialogs = new TestDeckCardlistViewDialogs(cardlist)
    {
      ImportDialog = new() { Result = ContentDialogResult.None, Values = "NotFound" }, // Cancel dialog
    };

    await cardlist.ImportToCardlistDialog();
  }

  [TestMethod]
  public async Task CardExportTest_Copy()
  {
    var cardlist = new DeckCardlistViewModel(new(), new TestCardAPI())
    { 
      ClipboardService = new TestIO.TestClipboard() 
    };
    cardlist.Dialogs = new TestDeckCardlistViewDialogs(cardlist)
    {
      ExportDialog = new() { Values = "Text" },
    };

    using var asserter = new NotificationAsserter(NotificationType.Info);

    await cardlist.ExportDeckCardsDialog();
  }

  [TestMethod]
  public async Task CardExportTest_Empty()
  {
    var cardlist = new DeckCardlistViewModel(new(), new TestCardAPI())
    { 
      ClipboardService = new TestIO.TestClipboard() 
    };
    cardlist.Dialogs = new TestDeckCardlistViewDialogs(cardlist)
    {
      ExportDialog = new() { Values = string.Empty },
    };

    using var asserter = new NotificationAsserter(NotificationType.Info);

    await cardlist.ExportDeckCardsDialog();
  }

  [TestMethod]
  public async Task CardExportTest_Canceled()
  {
    var cardlist = new DeckCardlistViewModel(new(), new TestCardAPI())
    { 
      ClipboardService = new TestIO.TestClipboard() 
    };
    cardlist.Dialogs = new TestDeckCardlistViewDialogs(cardlist)
    {
      ExportDialog = new() { Result = ContentDialogResult.None, Values = null }, // Cancel dialog
    };

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
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm) // Override dialogs
    {
      SaveDialog = new() { Values = deckName }, // Save deck
    };

    using var asserter = new NotificationAsserter(NotificationType.Success);

    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    await vm.SaveDeckDialog();
  }

  [TestMethod]
  public async Task DeckSaveTest_Fail()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new() { WillFail = true };
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm) // Override dialogs
    {
      SaveDialog = new() { Values = deckName }, // Save deck
    };

    using var asserter = new NotificationAsserter(NotificationType.Error);

    await vm.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
    await vm.SaveDeckDialog();
  }

  [TestMethod]
  public async Task DeckLoadingTest_Success()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm) // Override dialogs
    {
      LoadDialog = new() { Values = deckName }, // Load deck
    };

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
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm) // Override dialogs
    {
      LoadDialog = new() { Values = deckName }, // Load deck
    };

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
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm) // Override dialogs
    {
      LoadDialog = new() { Result = ContentDialogResult.None, Values = null }, // Cancel
    };

    using var asserter = new NotificationAsserter(NotificationType.Success) { WillFail = true };

    await vm.LoadDeckDialog();
  }

  [TestMethod]
  public async Task DeckDeletionTest_Success()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new();
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      SaveDialog = new() { Values = deckName }, // Save deck
      DeleteDialog = new(),
    };

    using var asserter = new NotificationAsserter(NotificationType.Success);

    await vm.SaveDeckDialog();
    await vm.DeleteDeckDialog();
  }

  [TestMethod]
  public async Task DeckDeletionTest_Fail()
  {
    var deckName = "First";

    using TestInMemoryMTGDeckRepository repo = new() { WillFail = true };
    DeckBuilderViewModel vm = new(null, repo);
    vm.Dialogs = new TestDeckBuilderViewDialogs(vm)
    {
      SaveDialog = new() { Values = deckName }, // Save deck
      DeleteDialog = new(),
    };

    using var asserter = new NotificationAsserter(NotificationType.Error);

    await vm.SaveDeckDialog();
    await vm.DeleteDeckDialog();
  }
  #endregion
}

using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests;

public partial class CardListViewModelTests
{
  [TestClass]
  public class ImportCardsTests
  {
    [TestMethod]
    public async Task ImportCards_SerializedCardData_CardAdded()
    {
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter());
      JsonExtensions.TrySerializeObject(new CardImportResult.Card(MTGCardInfoMocker.MockInfo()), out var json);

      await viewmodel.ImportCardsCommand.ExecuteAsync(json);

      Assert.HasCount(1, viewmodel.Cards);
    }

    [TestMethod]
    public async Task ImportCards_SerializedCardData_Undo_CardRemoved()
    {
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter());
      JsonExtensions.TrySerializeObject(new CardImportResult.Card(MTGCardInfoMocker.MockInfo()), out var json);

      await viewmodel.ImportCardsCommand.ExecuteAsync(json);
      viewmodel.UndoStack.Undo();

      Assert.IsEmpty(viewmodel.Cards);
    }

    [TestMethod]
    public async Task ImportCards_SerializedCardData_Redo_CardAddedAgain()
    {
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter());
      JsonExtensions.TrySerializeObject(new CardImportResult.Card(MTGCardInfoMocker.MockInfo()), out var json);

      await viewmodel.ImportCardsCommand.ExecuteAsync(json);
      viewmodel.UndoStack.Undo();
      viewmodel.UndoStack.Redo();

      Assert.HasCount(1, viewmodel.Cards);
    }

    [TestMethod]
    public async Task ImportCards_WithoutData_ImportConfirmationShown()
    {
      var confirmer = new TestConfirmer<string, string>();
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter())
      {
        Confirmers = new()
        {
          ImportConfirmer = confirmer
        }
      };

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task Importcards_WithData_NoConfirmationShown()
    {
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter())
      {
        Confirmers = new()
        {
          ImportConfirmer = new TestConfirmer<string, string>()
        }
      };

      await viewmodel.ImportCardsCommand.ExecuteAsync("Data");
    }

    [TestMethod]
    public async Task ImportCards_CardExists_ConflictImportConfirmationShown()
    {
      var confirmer = new TestConfirmer<(ConfirmationResult, bool)>();
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel([card], new TestMTGCardImporter()
      {
        ExpectedCards = [new(card.Info, card.Count)]
      })
      {
        Confirmers = new()
        {
          ImportConfirmer = new() { OnConfirm = async (arg) => { return await Task.FromResult(card.Info.Name); } },
          AddMultipleConflictConfirmer = confirmer,
        }
      };

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task ImportCards_CardDoesNotExist_NoConflictImportConfirmationShown()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter()
      {
        ExpectedCards = [new(card.Info, card.Count)]
      })
      {
        Confirmers = new()
        {
          ImportConfirmer = new() { OnConfirm = async (arg) => { return await Task.FromResult(card.Info.Name); } },
          AddMultipleConflictConfirmer = new TestConfirmer<(ConfirmationResult, bool)>(),
        }
      };

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);
    }

    [TestMethod]
    public async Task ImportMultipleCards_Conflicts_MultipleConflictImportConfirmationShown()
    {
      var conflictConfirmationCount = 0;
      var skipConflicts = false;

      var cards = new DeckEditorMTGCard[]
      {
        DeckEditorMTGCardMocker.CreateMTGCardModel(),
        DeckEditorMTGCardMocker.CreateMTGCardModel(),
        DeckEditorMTGCardMocker.CreateMTGCardModel(),
      };
      var viewmodel = new CardListViewModel([.. cards], new TestMTGCardImporter()
      {
        ExpectedCards = [.. cards.Select(x => new CardImportResult.Card(x.Info, x.Count))]
      })
      {
        Confirmers = new()
        {
          ImportConfirmer = new() { OnConfirm = async (arg) => { return await Task.FromResult("expcted cards"); } },
          AddMultipleConflictConfirmer = new()
          {
            OnConfirm = async (arg) =>
            {
              conflictConfirmationCount += 1;
              return await Task.FromResult((ConfirmationResult.Yes, skipConflicts));
            }
          }
        }
      };

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(cards.Length, conflictConfirmationCount);
    }

    [TestMethod]
    public async Task ImportMultipleCards_SkipConflicts_OneConflictImportConfirmationShown()
    {
      var conflictConfirmationCount = 0;
      var skipConflicts = true;

      var cards = new DeckEditorMTGCard[]
      {
      DeckEditorMTGCardMocker.CreateMTGCardModel(),
      DeckEditorMTGCardMocker.CreateMTGCardModel(),
      DeckEditorMTGCardMocker.CreateMTGCardModel(),
      };
      var viewmodel = new CardListViewModel([.. cards], new TestMTGCardImporter()
      {
        ExpectedCards = [.. cards.Select(x => new CardImportResult.Card(x.Info, x.Count))]
      })
      {
        Confirmers = new()
        {
          ImportConfirmer = new() { OnConfirm = async (arg) => { return await Task.FromResult("expcted cards"); } },
          AddMultipleConflictConfirmer = new()
          {
            OnConfirm = async (arg) =>
            {
              conflictConfirmationCount += 1;
              return await Task.FromResult((ConfirmationResult.Yes, skipConflicts));
            }
          }
        }
      };

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(1, conflictConfirmationCount);
    }

    [TestMethod]
    public async Task ImportCards_SerializedCardData_NoNotificationsSent()
    {
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter())
      {
        Notifier = new() { OnNotify = (arg) => throw new NotificationException(arg) }
      };

      JsonExtensions.TrySerializeObject(DeckEditorMTGCardMocker.CreateMTGCardModel(), out var json);

      await viewmodel.ImportCardsCommand.ExecuteAsync(json);
    }

    [TestMethod]
    public async Task ImportExternalCards_AllFound_SuccessNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter()
      {
        ExpectedCards = [new(MTGCardInfoMocker.MockInfo())]
      })
      {
        Notifier = notifier,
        Confirmers = new()
        {
          ImportConfirmer = new() { OnConfirm = async (arg) => { return await Task.FromResult("expcted cards"); } },
        }
      };

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationService.NotificationType.Success, notifier);
    }

    [TestMethod]
    public async Task ImportExternalCards_NotFound_ErrorNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter()
      {
        NotFoundCount = 1,
        ExpectedCards = []
      })
      {
        Notifier = notifier,
        Confirmers = new()
        {
          ImportConfirmer = new() { OnConfirm = async (arg) => { return await Task.FromResult("expcted cards"); } },
        }
      };

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task ImportExternalCards_SomeFound_WarningNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter()
      {
        NotFoundCount = 1,
        ExpectedCards = [new(MTGCardInfoMocker.MockInfo())]
      })
      {
        Notifier = notifier,
        Confirmers = new()
        {
          ImportConfirmer = new() { OnConfirm = async (arg) => { return await Task.FromResult("expcted cards"); } },
        }
      };

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationService.NotificationType.Warning, notifier);
    }

    [TestMethod]
    public async Task Import_SameCardTwice_CardsCombinedIntoOne()
    {
      var cardName = "Name";
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter()
      {
        ExpectedCards = [
          new(MTGCardInfoMocker.MockInfo(name: cardName)),
          new(MTGCardInfoMocker.MockInfo(name: cardName)),
        ]
      });

      await viewmodel.ImportCardsCommand.ExecuteAsync("API import");

      Assert.AreEqual(2, viewmodel.Cards.FirstOrDefault(x => x.Info.Name == cardName).Count);
    }
  }
}

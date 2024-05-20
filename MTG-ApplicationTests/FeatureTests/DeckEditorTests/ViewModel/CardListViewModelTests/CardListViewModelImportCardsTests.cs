using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.IOService;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class CardListViewModelImportCardsTests
{
  [TestMethod]
  public async Task ImportCards_SerializedCardData_CardAdded()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI());
    JsonService.TrySerializeObject(Mocker.MTGCardModelMocker.CreateMTGCardModel(), out var json);

    await viewmodel.ImportCardsCommand.ExecuteAsync(json);

    Assert.AreEqual(1, viewmodel.Cards.Count);
  }

  [TestMethod]
  public async Task ImportCards_SerializedCardData_Undo_CardRemoved()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI());
    JsonService.TrySerializeObject(Mocker.MTGCardModelMocker.CreateMTGCardModel(), out var json);

    await viewmodel.ImportCardsCommand.ExecuteAsync(json);
    viewmodel.UndoStack.Undo();

    Assert.AreEqual(0, viewmodel.Cards.Count);
  }

  [TestMethod]
  public async Task ImportCards_SerializedCardData_Redo_CardAddedAgain()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI());
    JsonService.TrySerializeObject(Mocker.MTGCardModelMocker.CreateMTGCardModel(), out var json);

    await viewmodel.ImportCardsCommand.ExecuteAsync(json);
    viewmodel.UndoStack.Undo();
    viewmodel.UndoStack.Redo();

    Assert.AreEqual(1, viewmodel.Cards.Count);
  }

  [TestMethod]
  public async Task ImportCards_WithoutData_ImportConfirmationShown()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI())
    {
      Confirmers = new()
      {
        ImportConfirmer = new TestExceptionConfirmer<string, string>()
      }
    };

    await ConfirmationAssert.ConfirmationShown(() => viewmodel.ImportCardsCommand.ExecuteAsync(null));
  }

  [TestMethod]
  public async Task Importcards_WithData_NoConfirmationShown()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI())
    {
      Confirmers = new()
      {
        ImportConfirmer = new TestExceptionConfirmer<string, string>()
      }
    };

    await viewmodel.ImportCardsCommand.ExecuteAsync("Data");
  }

  [TestMethod]
  public async Task ImportCards_CardExists_ConflictImportConfirmationShown()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var viewmodel = new CardListViewModel(new TestCardAPI()
    {
      ExpectedCards = [card]
    })
    {
      Cards = new([card]),
      Confirmers = new()
      {
        ImportConfirmer = new() { OnConfirm = async (arg) => { return await Task.FromResult(card.Info.Name); } },
        AddMultipleConflictConfirmer = new TestExceptionConfirmer<(ConfirmationResult, bool)>(),
      }
    };

    await ConfirmationAssert.ConfirmationShown(() => viewmodel.ImportCardsCommand.ExecuteAsync(null));
  }

  [TestMethod]
  public async Task ImportCards_CardDoesNotExist_NoConflictImportConfirmationShown()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var viewmodel = new CardListViewModel(new TestCardAPI()
    {
      ExpectedCards = [card]
    })
    {
      Confirmers = new()
      {
        ImportConfirmer = new() { OnConfirm = async (arg) => { return await Task.FromResult(card.Info.Name); } },
        AddMultipleConflictConfirmer = new TestExceptionConfirmer<(ConfirmationResult, bool)>(),
      }
    };

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);
  }

  [TestMethod]
  public async Task ImportMultipleCards_Conflicts_MultipleConflictImportConfirmationShown()
  {
    var conflictConfirmationCount = 0;
    var skipConflicts = false;

    var cards = new MTGCard[]
    {
      Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(),
    };
    var viewmodel = new CardListViewModel(new TestCardAPI()
    {
      ExpectedCards = [.. cards]
    })
    {
      Cards = new([.. cards]),
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

    var cards = new MTGCard[]
    {
      Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(),
    };
    var viewmodel = new CardListViewModel(new TestCardAPI()
    {
      ExpectedCards = [.. cards]
    })
    {
      Cards = new([.. cards]),
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
    var viewmodel = new CardListViewModel(new TestCardAPI())
    {
      Notifier = new() { OnNotify = (arg) => throw new NotificationException(arg) }
    };

    JsonService.TrySerializeObject(Mocker.MTGCardModelMocker.CreateMTGCardModel(), out var json);

    await viewmodel.ImportCardsCommand.ExecuteAsync(json);
  }

  [TestMethod]
  public async Task ImportExternalCards_AllFound_SuccessNotificationSent()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI()
    {
      ExpectedCards = [Mocker.MTGCardModelMocker.CreateMTGCardModel()]
    })
    {
      Confirmers = new()
      {
        ImportConfirmer = new() { OnConfirm = async (arg) => { return await Task.FromResult("expcted cards"); } },
      },
      Notifier = new() { OnNotify = (arg) => throw new NotificationException(arg) }
    };

    await NotificationAssert.NotificationSent(NotificationService.NotificationType.Success,
      () => viewmodel.ImportCardsCommand.ExecuteAsync(null));
  }

  [TestMethod]
  public async Task ImportExternalCards_NotFound_ErrorNotificationSent()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI()
    {
      NotFoundCount = 1,
      ExpectedCards = []
    })
    {
      Confirmers = new()
      {
        ImportConfirmer = new() { OnConfirm = async (arg) => { return await Task.FromResult("expcted cards"); } },
      },
      Notifier = new() { OnNotify = (arg) => throw new NotificationException(arg) }
    };

    await NotificationAssert.NotificationSent(NotificationService.NotificationType.Error,
      () => viewmodel.ImportCardsCommand.ExecuteAsync(null));
  }

  [TestMethod]
  public async Task ImportExternalCards_SomeFound_WarningNotificationSent()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI()
    {
      NotFoundCount = 1,
      ExpectedCards = [Mocker.MTGCardModelMocker.CreateMTGCardModel()]
    })
    {
      Confirmers = new()
      {
        ImportConfirmer = new() { OnConfirm = async (arg) => { return await Task.FromResult("expcted cards"); } },
      },
      Notifier = new() { OnNotify = (arg) => throw new NotificationException(arg) }
    };

    await NotificationAssert.NotificationSent(NotificationService.NotificationType.Warning,
      () => viewmodel.ImportCardsCommand.ExecuteAsync(null));
  }

  [TestMethod]
  public async Task Import_SameCardTwice_CardsCombinedIntoOne()
  {
    var cardName = "Name";
    var viewmodel = new CardListViewModel(new TestCardAPI()
    {
      ExpectedCards = [
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: cardName),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: cardName),
        ]
    });

    await viewmodel.ImportCardsCommand.ExecuteAsync("API import");

    Assert.AreEqual(2, viewmodel.Cards.FirstOrDefault(x => x.Info.Name == cardName)?.Count);
  }
}
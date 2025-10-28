using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.ViewModels;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.UseCases;

[TestClass]
public class SaveDeck : DeckEditorViewModelTestBase, ISaveCommandTests, IWorkerTests
{
  [TestMethod(DisplayName = "Should show save confirmation when saving the deck")]
  public async Task Save_SaveConfirmationShown()
  {
    var confirmer = new TestConfirmer<string, string>();
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      Confirmers = new()
      {
        SaveDeckConfirmer = confirmer

      }
    }.MockVM();

    await viewmodel.SaveDeckCommand.ExecuteAsync(null);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod(DisplayName = "Should not save if the confirmation was canceled")]
  public async Task Save_Cancel_NotSaved()
  {
    var newDeck = MTGCardDeckMocker.Mock("New Deck");
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = newDeck,
      Confirmers = new()
      {
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult<string>(null) }
      }
    }.MockVM();

    await viewmodel.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(await _dependencies.Repository.Exists(newDeck.Name));
    Assert.IsFalse(await _dependencies.Repository.Exists(string.Empty));
  }

  [TestMethod(DisplayName = "Should not save the deck if the given name is empty")]
  public async Task Save_WithEmptyName_NotSaved()
  {
    var newDeck = MTGCardDeckMocker.Mock("New Deck");
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = newDeck,
      Confirmers = new()
      {
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(string.Empty) }
      }
    }.MockVM();

    await viewmodel.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(await _dependencies.Repository.Exists(newDeck.Name));
    Assert.IsFalse(await _dependencies.Repository.Exists(string.Empty));
  }

  [TestMethod(DisplayName = "Should save the deck if a name was given to the confirmation")]
  public async Task Save_NewName_Saved()
  {
    var newDeck = MTGCardDeckMocker.Mock("New Deck");
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = newDeck,
      Confirmers = new()
      {
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(newDeck.Name) }
      }
    }.MockVM();

    await viewmodel.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsTrue(await _dependencies.Repository.Exists(newDeck.Name));
  }

  [TestMethod(DisplayName = "Should not show override confirmation if saving with the same name")]
  public async Task Save_SameName_NoOverrideConfirmationShown()
  {
    var confirmer = new TestConfirmer<ConfirmationResult>();
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      Confirmers = new()
      {
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) },
        OverrideDeckConfirmer = confirmer,
      }
    }.MockVM();

    await viewmodel.SaveDeckCommand.ExecuteAsync(null);

    ConfirmationAssert.ConfirmationNotShown(confirmer);
  }

  [TestMethod(DisplayName = "Should show override confirmation when trying to save over an existing deck")]
  public async Task Save_Override_OverrideConfirmationShown()
  {
    var confirmer = new TestConfirmer<ConfirmationResult>();
    var newDeck = MTGCardDeckMocker.Mock("New Deck");
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = newDeck,
      Confirmers = new()
      {
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) },
        OverrideDeckConfirmer = confirmer,
      }
    }.MockVM();

    await viewmodel.SaveDeckCommand.ExecuteAsync(null);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod(DisplayName = "Saving should be canceled if overriding was canceled")]
  public async Task Save_Override_Cancel_NotSaved()
  {
    var newDeck = MTGCardDeckMocker.Mock("New Deck");
    newDeck.Commander = DeckEditorMTGCardMocker.CreateMTGCardModel(name: "New Commander");

    var viewmodel = new Mocker(_dependencies)
    {
      Deck = newDeck,
      Confirmers = new()
      {
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) },
        OverrideDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Cancel) },
      }
    }.MockVM();

    await viewmodel.SaveDeckCommand.ExecuteAsync(null);

    var dbDeck = await _dependencies.Repository.Get(_savedDeck.Name);
    Assert.AreEqual(_savedDeck.Commander.Info.Name, dbDeck.Commander.Name);
  }

  [TestMethod(DisplayName = "Existing deck should be overridden if override was accepted")]
  public async Task Save_Override_Accept_Overridden()
  {
    var newDeck = MTGCardDeckMocker.Mock("New Deck");
    newDeck.Commander = DeckEditorMTGCardMocker.CreateMTGCardModel(name: "New Commander");

    var viewmodel = new Mocker(_dependencies)
    {
      Deck = newDeck,
      Confirmers = new()
      {
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) },
        OverrideDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) },
      }
    }.MockVM();

    await viewmodel.SaveDeckCommand.ExecuteAsync(null);

    var dbDeck = await _dependencies.Repository.Get(_savedDeck.Name);
    Assert.AreEqual(newDeck.Commander.Info.Name, dbDeck.Commander.Name);
  }

  [TestMethod]
  public async Task Save_Renamed_OldDeleted()
  {
    var newName = "New Name";
    var oldName = _savedDeck.Name;
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      Confirmers = new()
      {
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(newName) }
      }
    }.MockVM();

    await viewmodel.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsNull(await _dependencies.Repository.Get(oldName));
  }

  [TestMethod(DisplayName = "Decks name should be changed to the given name when the deck has been saved with a different name")]
  public async Task Save_Renamed_NameChanged()
  {
    var newName = "New Name";
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      Confirmers = new()
      {
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(newName) }
      }
    }.MockVM();

    await viewmodel.SaveDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(newName, viewmodel.Name);
  }

  [TestMethod(DisplayName = "Should have no unsaved changes if the deck was saved")]
  public async Task Save_Success_NoUnsavedChanges()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      }
    }.MockVM();

    await viewmodel.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(viewmodel.HasUnsavedChanges);
  }

  [TestMethod(DisplayName = "Success notification should be sent when the deck was saved")]
  public async Task Save_Success_SuccessNotificationSent()
  {
    var notifier = new TestNotifier();
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      },
      Notifier = notifier
    }.MockVM();

    await viewmodel.SaveDeckCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Success, notifier);
  }

  [TestMethod(DisplayName = "Error notification should be sent when there are failure on saving")]
  public async Task Save_Error_ErrorNotificationSent()
  {
    _dependencies.Repository.UpdateFailure = true;

    var notifier = new TestNotifier();
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      },
      Notifier = notifier
    }.MockVM();

    await viewmodel.SaveDeckCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Error, notifier);
  }

  [TestMethod(DisplayName = "ViewModel should be busy when saving the deck")]
  public async Task Execute_IsBusy()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      }
    }.MockVM();

    await WorkerAssert.IsBusy(viewmodel, () => viewmodel.SaveDeckCommand.ExecuteAsync(null));
  }

  [TestMethod]
  public async Task Save_SaveEmpty_Success()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = new() { DeckCards = [] },
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
        SaveDeckConfirmer = new() { OnConfirm = async msg => await Task.FromResult("New") },
      },
    }.MockVM();

    var args = new ISavable.ConfirmArgs();
    await viewmodel.SaveDeckCommand.ExecuteAsync(args);

    Assert.IsFalse(args.Cancelled);
  }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class SaveDeckTests : DeckEditorViewModelTestsBase, ISaveCommandTests, IWorkerTests
  {
    [TestMethod("Should show save confirmation when saving the deck")]
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

    [TestMethod("Should not save if the confirmation was canceled")]
    public async Task Save_Cancel_NotSaved()
    {
      var newDeck = MTGCardDeckMocker.Mock("New Deck");
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = newDeck,
        Confirmers = new()
        {
          SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult<string?>(null) }
        }
      }.MockVM();

      await viewmodel.SaveDeckCommand.ExecuteAsync(null);

      Assert.IsFalse(await _dependencies.Repository.Exists(newDeck.Name));
      Assert.IsFalse(await _dependencies.Repository.Exists(string.Empty));
    }

    [TestMethod("Should not save the deck if the given name is empty")]
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

    [TestMethod("Should save the deck if a name was given to the confirmation")]
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

    [TestMethod("Should not show override confirmation if saving with the same name")]
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

    [TestMethod("Should show override confirmation when trying to save over an existing deck")]
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

    [TestMethod("Saving should be canceled if overriding was canceled")]
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
      Assert.AreEqual(_savedDeck.Commander?.Info.Name, dbDeck?.Commander?.Name);
    }

    [TestMethod("Existing deck should be overridden if override was accepted")]
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
      Assert.AreEqual(newDeck.Commander.Info.Name, dbDeck?.Commander?.Name);
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

    [TestMethod("Decks name should be changed to the given name when the deck has been saved with a different name")]
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

    [TestMethod("Should have no unsaved changes if the deck was saved")]
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

    [TestMethod("Success notification should be sent when the deck was saved")]
    public async Task Save_Success_SuccessNotificationSent()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
        },
        Notifier =
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      }.MockVM();

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => viewmodel.SaveDeckCommand.ExecuteAsync(null));
    }

    [TestMethod("Error notification should be sent when there are failure on saving")]
    public async Task Save_Error_ErrorNotificationSent()
    {
      _dependencies.Repository.UpdateFailure = true;

      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
        },
        Notifier =
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      }.MockVM();

      await NotificationAssert.NotificationSent(NotificationType.Error,
        () => viewmodel.SaveDeckCommand.ExecuteAsync(null));
    }

    [TestMethod("ViewModel should be busy when saving the deck")]
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
      await viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(args);

      Assert.IsFalse(args.Cancelled);
    }
  }
}

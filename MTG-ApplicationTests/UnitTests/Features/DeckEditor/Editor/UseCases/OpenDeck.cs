using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.ViewModels;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.UseCases;

[TestClass]
public class OpenDeck : DeckEditorViewModelTestBase,
  ICanExecuteWithParameterCommandTests, IUnsavedChangesCheckTests, IOpenCommandTests, IWorkerTests
{
  [TestMethod]
  public void ValidParameter_CanExecute()
  {
    var viewmodel = new Mocker(_dependencies).MockVM();

    Assert.IsTrue(viewmodel.OpenDeckCommand.CanExecute(null));
    Assert.IsTrue(viewmodel.OpenDeckCommand.CanExecute("Name"));
  }

  [TestMethod]
  public void InvalidParameter_CanNotExecute()
  {
    var viewmodel = new Mocker(_dependencies).MockVM();

    Assert.IsFalse(viewmodel.OpenDeckCommand.CanExecute(string.Empty));
  }

  [TestMethod]
  public async Task Execute_WithValidParameter_Executed()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        LoadDeckConfirmer = new TestConfirmer<string, string[]>()
      }
    }.MockVM();

    await viewmodel.OpenDeckCommand.ExecuteAsync(_savedDeck.Name);

    Assert.AreEqual(_savedDeck.Name, viewmodel.Name);
  }

  [TestMethod(DisplayName = "Load confirmation should not be shown when executing with a name parameter")]
  public async Task Open_WithParameter_NoLoadConfirmationShown()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        LoadDeckConfirmer = new TestConfirmer<string, string[]>()
      }
    }.MockVM();

    await viewmodel.OpenDeckCommand.ExecuteAsync(_savedDeck.Name);
  }

  [TestMethod(DisplayName = "Deck should be the same when executing with an empty name parameter")]
  public async Task Execute_WithInvalidParameter_Canceled()
  {
    var unsavedDeck = new DeckEditorMTGDeck() { Name = "Unsaved Deck" };
    var viewmodel = new Mocker(_dependencies) { Deck = unsavedDeck }.MockVM();

    await viewmodel.OpenDeckCommand.ExecuteAsync(string.Empty);

    Assert.AreEqual(unsavedDeck.Name, viewmodel.Name);
  }

  [TestMethod]
  public async Task Execute_HasUnsavedChanges_UnsavedChangesConfirmationShown()
  {
    var confirmer = new TestConfirmer<ConfirmationResult>();
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        SaveUnsavedChangesConfirmer = confirmer
      }
    }.MockVM();

    await viewmodel.OpenDeckCommand.ExecuteAsync(null);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod]
  public async Task Execute_HasUnsavedChanges_Cancel_HasUnsavedChanges()
  {
    var unsavedDeck = new DeckEditorMTGDeck() { Name = "Unsaved Deck" };
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = unsavedDeck,
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Cancel) },
      }
    }.MockVM();

    await viewmodel.OpenDeckCommand.ExecuteAsync(null);

    Assert.IsTrue(viewmodel.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Execute_HasUnsavedChanges_Decline_NoUnsavedChanges()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) },
        LoadDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      }
    }.MockVM();

    await viewmodel.OpenDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(viewmodel.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Execute_HasUnsavedChanges_Accept_SaveConfirmationShown()
  {
    var confirmer = new TestConfirmer<string, string>();
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) },
        SaveDeckConfirmer = confirmer
      }
    }.MockVM();

    await viewmodel.OpenDeckCommand.ExecuteAsync(null);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod(DisplayName = "Load confirmation should be shown when loading a deck without a name")]
  public async Task Open_OpenConfirmationShown()
  {
    var confirmer = new TestConfirmer<string, string[]>();
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        LoadDeckConfirmer = confirmer
      }
    }.MockVM();

    await viewmodel.OpenDeckCommand.ExecuteAsync(null);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod(DisplayName = "Deck should be the same if the loading was canceled when loading a deck")]
  public async Task Open_Cancel_NoChanges()
  {
    var unsavedDeck = new DeckEditorMTGDeck() { Name = "Unsaved Deck" };
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = unsavedDeck,
      Confirmers = new()
      {
        LoadDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult<string>(null) }
      }
    }.MockVM();

    await viewmodel.OpenDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(unsavedDeck.Name, viewmodel.Name);
  }

  [TestMethod(DisplayName = "Deck should be the loaded deck if the loading was successful")]
  public async Task Open_Success_Changed()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        LoadDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      }
    }.MockVM();

    await viewmodel.OpenDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(_savedDeck.Name, viewmodel.Name);
  }

  [TestMethod(DisplayName = "Success notification should be sent when deck has been loaded")]
  public async Task Open_Success_SuccessNotificationSent()
  {
    var notifier = new TestNotifier();
    var viewmodel = new Mocker(_dependencies)
    {
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) },
        LoadDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      },
      Notifier = notifier
    }.MockVM();

    await viewmodel.OpenDeckCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Success, notifier);
  }

  [TestMethod(DisplayName = "Error notification should be sent when there are failure on loading")]
  public async Task Open_Error_ErrorNotificationSent()
  {
    _dependencies.Repository.GetFailure = true;

    var notifier = new TestNotifier();
    var viewmodel = new Mocker(_dependencies)
    {
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) },
        LoadDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      },
      Notifier = notifier
    }.MockVM();

    await viewmodel.OpenDeckCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Error, notifier);
  }

  [TestMethod]
  public async Task Execute_IsBusy()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        LoadDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      }
    }.MockVM();

    await WorkerAssert.IsBusy(viewmodel, () => viewmodel.OpenDeckCommand.ExecuteAsync(null));
  }
}

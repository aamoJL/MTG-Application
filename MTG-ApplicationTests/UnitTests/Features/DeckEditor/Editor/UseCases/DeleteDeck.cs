using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.ViewModels;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.UseCases;

[TestClass]
public class DeleteDeck : DeckEditorViewModelTestBase, ICanExecuteCommandTests, IDeleteCommandTests
{
  [TestMethod(DisplayName = "Should be able to execute if the deck has a name")]
  public void ValidState_CanExecute()
  {
    var viewmodel = new Mocker(_dependencies) { Deck = new() { Name = "New Deck" } }.MockVM();

    Assert.IsTrue(viewmodel.DeleteDeckCommand.CanExecute(null));
  }

  [TestMethod(DisplayName = "Should not be able to execute if the deck has no name")]
  public void InvalidState_CanNotExecute()
  {
    var viewmodel = new Mocker(_dependencies) { Deck = new() }.MockVM();

    Assert.IsFalse(viewmodel.DeleteDeckCommand.CanExecute(null));
  }

  [TestMethod]
  public async Task Delete_DeleteConfirmationShown()
  {
    var confirmer = new TestConfirmer<ConfirmationResult>();
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      Confirmers = new()
      {
        DeleteDeckConfirmer = confirmer
      }
    }.MockVM();

    await viewmodel.DeleteDeckCommand.ExecuteAsync(null);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod(DisplayName = "Deck should not be deleted if the deletion was canceled")]
  public async Task Delete_Cancel_NotDeleted()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      Confirmers = new()
      {
        DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Cancel) }
      }
    }.MockVM();

    await viewmodel.DeleteDeckCommand.ExecuteAsync(null);

    Assert.IsTrue(await _dependencies.Repository.Exists(_savedDeck.Name));
  }

  [TestMethod(DisplayName = "Deck should be deleted if the deletion was confirmed")]
  public async Task Delete_Accept_Deleted()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      Confirmers = new()
      {
        DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
      }
    }.MockVM();

    await viewmodel.DeleteDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(await _dependencies.Repository.Exists(_savedDeck.Name));
  }

  [TestMethod(DisplayName = "Deck should reset when the deck has been deleted")]
  public async Task Delete_Success_Reset()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      Confirmers = new()
      {
        DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
      }
    }.MockVM();

    await viewmodel.DeleteDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, viewmodel.Name);
    Assert.IsFalse(viewmodel.HasUnsavedChanges);
  }

  [TestMethod(DisplayName = "Success notification should be sent when the deck was deleted")]
  public async Task Delete_Success_SuccessNotificationSent()
  {
    var notifier = new TestNotifier();
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      Confirmers = new()
      {
        DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
      },
      Notifier = notifier
    }.MockVM();

    await viewmodel.DeleteDeckCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Success, notifier);
  }

  [TestMethod(DisplayName = "Error notification should be sent when there are failure on deletion")]
  public async Task Delete_Error_ErrorNotificationSent()
  {
    _dependencies.Repository.DeleteFailure = true;

    var notifier = new TestNotifier();
    var viewmodel = new Mocker(_dependencies)
    {
      Deck = _savedDeck,
      Confirmers = new()
      {
        DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
      },
      Notifier = notifier
    }.MockVM();

    await viewmodel.DeleteDeckCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Error, notifier);
  }
}


using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class DeleteDeckTests : DeckEditorViewModelTestsBase
  {
    [TestMethod("Should be able to execute if the deck has a name")]
    public void DeleteDeck_HasName_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies) { Deck = new() { Name = "New Deck" } }.MockVM();

      Assert.IsTrue(viewmodel.DeleteDeckCommand.CanExecute(null));
    }

    [TestMethod("Should not be able to execute if the deck has no name")]
    public void DeleteDeck_NoName_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies) { Deck = new() }.MockVM();

      Assert.IsFalse(viewmodel.DeleteDeckCommand.CanExecute(null));
    }

    [TestMethod("Deck should be deleted if the deletion was confirmed")]
    public async Task DeleteDeck_Accept_DeckDeleted()
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

    [TestMethod("Deck should reset when the deck has been deleted")]
    public async Task DeleteDeck_Accept_DeckReset()
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

      Assert.AreEqual(string.Empty, viewmodel.DeckName);
    }

    [TestMethod("Deck should not reset if there are a failure when deleting the deck")]
    public async Task DeleteDeck_Failure_DeckNotReset()
    {
      _dependencies.Repository.DeleteFailure = true;

      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        Confirmers = new()
        {
          DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
        }
      }.MockVM();

      await viewmodel.DeleteDeckCommand.ExecuteAsync(null);

      Assert.AreEqual(_savedDeck.Name, viewmodel.DeckName);
    }

    [TestMethod("Deck should not be deleted if the deletion was canceled")]
    public async Task DeleteDeck_Cancel_DeckNotDeleted()
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

    [TestMethod("ViewModel should have no unsaved changes if the deck was deleted")]
    public async Task DeleteDeck_Deleted_NoUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
        }
      }.MockVM();

      await viewmodel.DeleteDeckCommand.ExecuteAsync(null);

      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }

    [TestMethod("Success notification should be sent when the deck was deleted")]
    public async Task DeleteDeck_Deleted_SuccessNotificationSent()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        Confirmers = new()
        {
          DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
        },
        Notifier = new()
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      }.MockVM();

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => viewmodel.DeleteDeckCommand.ExecuteAsync(null));
    }

    [TestMethod("Error notification should be sent when there are failure on deletion")]
    public async Task DeleteDeck_Failure_ErrorNotificationSent()
    {
      _dependencies.Repository.DeleteFailure = true;

      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        Confirmers = new()
        {
          DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
        },
        Notifier = new()
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      }.MockVM();

      await NotificationAssert.NotificationSent(NotificationType.Error,
        () => viewmodel.DeleteDeckCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task DeleteDeck_ConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        Confirmers = new()
        {
          DeleteDeckConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
        }
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.DeleteDeckCommand.ExecuteAsync(null));
    }
  }
}

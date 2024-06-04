using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardCollection;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class DeleteCollectionTests : CardCollectionViewModelTestsBase, ICanExecuteCommandTests, IDeleteCommandTests
  {
    [TestMethod("Should be able to execute if the collection has a name")]
    public void ValidState_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies) { Collection = new() { Name = "Collection" } }.MockVM();

      Assert.IsTrue(viewmodel.DeleteCollectionCommand.CanExecute(null));
    }

    [TestMethod("Should not be able to execute if the collection has no name")]
    public void InvalidState_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies) { Collection = new() { Name = string.Empty } }.MockVM();

      Assert.IsFalse(viewmodel.DeleteCollectionCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task Delete_DeleteConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
        }
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.DeleteCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod("Collection should not be deleted if the deletion was canceled")]
    public async Task Delete_Cancel_NotDeleted()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Cancel) }
        }
      }.MockVM();

      await viewmodel.DeleteCollectionCommand.ExecuteAsync(null);

      Assert.IsTrue(await _dependencies.Repository.Exists(_savedCollection.Name));
    }

    [TestMethod("Collection should be deleted if the deletion was confirmed")]
    public async Task Delete_Accept_Deleted()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
        }
      }.MockVM();

      await viewmodel.DeleteCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(await _dependencies.Repository.Exists(_savedCollection.Name));
    }

    [TestMethod("Collection should reset when the collection has been deleted")]
    public async Task Delete_Success_Reset()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
        }
      }.MockVM();

      await viewmodel.DeleteCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(string.Empty, viewmodel.Collection.Name);
      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Delete_Success_QueryCardsReset()
    {
      var viewmodel = new CardCollectionViewModel(new TestCardAPI())
      {
        Collection = _savedCollection
      };

      await viewmodel.DeleteCollectionCommand.ExecuteAsync(null);
      await viewmodel.QueryCards.Collection.LoadMoreItemsAsync(10);

      Assert.AreEqual(0, viewmodel.QueryCards.TotalCardCount);
    }

    [TestMethod("Success notification should be sent when the collection was deleted")]
    public async Task Delete_Success_SuccessNotificationSent()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
        },
        Notifier = new()
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      }.MockVM();

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => viewmodel.DeleteCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod("Error notification should be sent when there are failure on deletion")]
    public async Task Delete_Error_ErrorNotificationSent()
    {
      _dependencies.Repository.DeleteFailure = true;

      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
        },
        Notifier = new()
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      }.MockVM();

      await NotificationAssert.NotificationSent(NotificationType.Error,
        () => viewmodel.DeleteCollectionCommand.ExecuteAsync(null));
    }
  }
}

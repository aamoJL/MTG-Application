using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class DeleteListTests : CardCollectionViewModelTestsBase, ICanExecuteWithParameterCommandTests
  {
    [TestMethod("Should be able to execute if the given list is in the collection")]
    public void ValidParameter_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies) { Model = _savedCollection }.MockVM();

      Assert.IsTrue(viewmodel.DeleteListCommand.CanExecute(_savedCollection.CollectionLists.First()));
    }

    [TestMethod("Should not be able to execute if the given list is not in the collection")]
    public void InvalidParameter_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies) { Model = _savedCollection }.MockVM();

      Assert.IsFalse(viewmodel.DeleteListCommand.CanExecute(null));
      Assert.IsFalse(viewmodel.DeleteListCommand.CanExecute(new MTGCardCollectionList()));
    }

    [TestMethod]
    public async Task DeleteList_DeleteConfirmationShown()
    {
      var confirmer = new TestConfirmer<ConfirmationResult>();
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = confirmer
        }
      }.MockVM();

      await viewmodel.DeleteListCommand.ExecuteAsync(_savedCollection.CollectionLists.First());

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task DeleteList_Cancel_NoChanges()
    {
      var list = _savedCollection.CollectionLists.First();
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel)
          }
        }
      }.MockVM();

      await viewmodel.DeleteListCommand.ExecuteAsync(list);

      Assert.IsTrue(viewmodel.CollectionLists.Contains(list));
    }

    [TestMethod]
    public async Task DeleteList_Accept_ListDeleted()
    {
      var list = _savedCollection.CollectionLists.First();
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        }
      }.MockVM();

      await viewmodel.DeleteListCommand.ExecuteAsync(list);

      Assert.IsFalse(viewmodel.CollectionLists.Contains(list));
    }

    [TestMethod]
    public async Task DeleteList_Success_HasUnsavedChanges()
    {
      var list = _savedCollection.CollectionLists.First();
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        }
      }.MockVM();

      await viewmodel.DeleteListCommand.ExecuteAsync(list);

      Assert.IsTrue(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task DeleteList_Success_SuccessNotificationSent()
    {
      var list = _savedCollection.CollectionLists.First();
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        },
        Notifier = notifier
      }.MockVM();

      await viewmodel.DeleteListCommand.ExecuteAsync(list);

      NotificationAssert.NotificationSent(NotificationType.Success, notifier);
    }

    [TestMethod]
    public async Task DeleteList_Error_ErrorNotificationSent()
    {
      var list = _savedCollection.CollectionLists.First();
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        },
        Notifier = notifier
      }.MockVM();

      // Remove list manually, so the command will fail to remove it
      viewmodel.CollectionLists.Remove(list);

      await viewmodel.DeleteListCommand.ExecuteAsync(list);

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task DeleteList_Success_OnListRemovedInvoked()
    {
      MTGCardCollectionList invoked = null;
      var list = _savedCollection.CollectionLists.First();
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        },
        OnListRemoved = async (list) => { invoked = list; await Task.Yield(); }
      }.MockVM();

      await viewmodel.DeleteListCommand.ExecuteAsync(list);

      Assert.IsNotNull(invoked);
      Assert.AreEqual(list, invoked);
    }
  }
}

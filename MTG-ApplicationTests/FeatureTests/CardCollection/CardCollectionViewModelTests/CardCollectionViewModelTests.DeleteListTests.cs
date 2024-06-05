using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class DeleteListTests : CardCollectionViewModelTestsBase, ICanExecuteCommandTests
  {
    [TestMethod("Should not be able to execute if a list is not selected")]
    public void InvalidState_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM();

      Assert.IsFalse(viewmodel.DeleteListCommand.CanExecute(null));
    }

    [TestMethod("Should be able to execute if a list is selected")]
    public void ValidState_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies) { Collection = _savedCollection }.MockVM();

      viewmodel.SelectListCommand.Execute(_savedCollection.CollectionLists.First().Name);

      Assert.IsTrue(viewmodel.DeleteListCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task DeleteList_DeleteConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
        }
      }.MockVM();

      await viewmodel.SelectListCommand.ExecuteAsync(_savedCollection.CollectionLists.First().Name);

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.DeleteListCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task DeleteList_Cancel_NoChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel)
          }
        }
      }.MockVM();

      await viewmodel.SelectListCommand.ExecuteAsync(_savedCollection.CollectionLists.First().Name);
      await viewmodel.DeleteListCommand.ExecuteAsync(null);

      Assert.AreEqual(viewmodel.SelectedList, _savedCollection.CollectionLists.First());
    }

    [TestMethod]
    public async Task DeleteList_Accept_ListDeleted()
    {
      var name = _savedCollection.CollectionLists.First().Name;
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        }
      }.MockVM();

      await viewmodel.SelectListCommand.ExecuteAsync(name);
      await viewmodel.DeleteListCommand.ExecuteAsync(null);

      Assert.IsNull(viewmodel.Collection.CollectionLists.FirstOrDefault(x => x.Name == name));
    }

    [TestMethod]
    public async Task DeleteList_Success_HasUnsavedChanges()
    {
      var name = _savedCollection.CollectionLists.First().Name;
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        }
      }.MockVM();

      await viewmodel.SelectListCommand.ExecuteAsync(name);
      await viewmodel.DeleteListCommand.ExecuteAsync(null);

      Assert.IsTrue(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task DeleteList_Success_SelectedListChanged()
    {
      var name = _savedCollection.CollectionLists.First().Name;
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        }
      }.MockVM();

      await viewmodel.SelectListCommand.ExecuteAsync(name);
      await viewmodel.DeleteListCommand.ExecuteAsync(null);

      Assert.IsNotNull(viewmodel.SelectedList);
      Assert.AreNotEqual(name, viewmodel.SelectedList?.Name);
    }

    [TestMethod]
    public async Task DeleteList_Success_SuccessNotificationSent()
    {
      var name = _savedCollection.CollectionLists.First().Name;
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();

      await viewmodel.SelectListCommand.ExecuteAsync(name);

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => viewmodel.DeleteListCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task DeleteList_Error_ErrorNotificationSent()
    {
      var name = _savedCollection.CollectionLists.First().Name;
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();

      await viewmodel.SelectListCommand.ExecuteAsync(name);

      // Remove list manually, so the command will fail to remove it
      viewmodel.Collection.CollectionLists.Remove(viewmodel.SelectedList);

      await NotificationAssert.NotificationSent(NotificationType.Error,
        () => viewmodel.DeleteListCommand.ExecuteAsync(null));
    }
  }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;
using static MTGApplicationTests.FeatureTests.CardCollectionEditor.CardCollectionEditorViewModelTests.CardCollectionEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;

public partial class CardCollectionEditorViewModelTests
{
  [TestClass]
  public class DeleteCollectionTests : CardCollectionEditorViewModelTestsBase, ICanExecuteCommandTests, IDeleteCommandTests
  {
    [TestMethod("Should be able to execute if the collection has a name")]
    public void ValidState_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM(new() { Name = "Collection" });

      Assert.IsTrue(viewmodel.DeleteCollectionCommand.CanExecute(null));
    }

    [TestMethod("Should not be able to execute if the collection has no name")]
    public void InvalidState_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM(new() { Name = string.Empty });

      Assert.IsFalse(viewmodel.DeleteCollectionCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task Delete_DeleteConfirmationShown()
    {
      var confirmer = new TestConfirmer<ConfirmationResult>();
      var viewmodel = new Mocker(_dependencies)
      {

        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            DeleteCollectionConfirmer = confirmer
          }
        }
      }.MockVM(_savedCollection);

      await viewmodel.DeleteCollectionCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod("Collection should not be deleted if the deletion was canceled")]
    public async Task Delete_Cancel_NotDeleted()
    {
      var viewmodel = new Mocker(_dependencies)
      {

        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            DeleteCollectionConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Cancel) }
          }
        }
      }.MockVM(_savedCollection);

      await viewmodel.DeleteCollectionCommand.ExecuteAsync(null);

      Assert.IsTrue(await _dependencies.Repository.Exists(_savedCollection.Name));
    }

    [TestMethod("Collection should be deleted if the deletion was confirmed")]
    public async Task Delete_Accept_Deleted()
    {
      var viewmodel = new Mocker(_dependencies)
      {

        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            DeleteCollectionConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
          }
        }
      }.MockVM(_savedCollection);

      await viewmodel.DeleteCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(await _dependencies.Repository.Exists(_savedCollection.Name));
    }

    [TestMethod("Success notification should be sent when the collection was deleted")]
    public async Task Delete_Success_SuccessNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {

        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            DeleteCollectionConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
          }
        },
        Notifier = notifier
      }.MockVM(_savedCollection);

      await viewmodel.DeleteCollectionCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Success, notifier);
    }

    [TestMethod("Error notification should be sent when there are failure on deletion")]
    public async Task Delete_Error_ErrorNotificationSent()
    {
      _dependencies.Repository.DeleteFailure = true;

      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {

        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            DeleteCollectionConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
          }
        },
        Notifier = notifier
      }.MockVM(_savedCollection);

      await viewmodel.DeleteCollectionCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }
  }
}

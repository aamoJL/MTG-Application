using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollectionEditor.CardCollectionEditorViewModelTests;

public partial class CardCollectionEditorViewModelTests
{
  [TestClass]
  public class OpenCollectionTests : CardCollectionEditorViewModelTestsBase, IUnsavedChangesCheckTests, IOpenCommandTests
  {
    [TestMethod]
    public async Task Execute_HasUnsavedChanges_UnsavedChangesConfirmationShown()
    {
      var confirmer = new TestConfirmer<ConfirmationResult>();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            SaveUnsavedChangesConfirmer = confirmer
          }
        },
        HasUnsavedChanges = true
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Cancel_HasUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel) },
          }
        },
        HasUnsavedChanges = true
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.IsTrue(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Decline_NoUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) },
            LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
          }
        },
        HasUnsavedChanges = true
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Accept_SaveConfirmationShown()
    {
      var confirmer = new TestConfirmer<string, string>();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
            SaveCollectionConfirmer = confirmer
          }
        },
        HasUnsavedChanges = true
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task Open_OpenConfirmationShown()
    {
      var confirmer = new TestConfirmer<string, IEnumerable<string>>();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            LoadCollectionConfirmer = confirmer
          }
        },
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task Open_Cancel_NoChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult<string>(null) }
          }
        },
      }.MockVM(_savedCollection);

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(_savedCollection.Name, viewmodel.Collection.Name);
    }

    [TestMethod]
    public async Task Open_Success_Changed()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
          }
        },
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(_savedCollection.Name, viewmodel.Collection.Name);
      CollectionAssert.AreEqual(
        _savedCollection.CollectionLists.Select(x => x.Name).ToArray(),
        viewmodel.Collection.CollectionLists.Select(x => x.Name).ToArray());
    }

    [TestMethod]
    public async Task Open_Success_SuccessNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
          }
        },
        Notifier = notifier
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Success, notifier);
    }

    [TestMethod]
    public async Task Open_Error_ErrorNotificationSent()
    {
      _dependencies.Repository.GetFailure = true;

      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
          }
        },
        Notifier = notifier
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task Open_Success_HasNoUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) },
            LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
          }
        },
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }
  }
}

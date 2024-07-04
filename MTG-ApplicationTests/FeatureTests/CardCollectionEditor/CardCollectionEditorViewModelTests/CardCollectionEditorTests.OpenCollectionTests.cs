using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollectionEditor.CardCollectionEditorViewModelTests;

public partial class CardCollectionEditorTests
{
  [TestClass]
  public class OpenCollectionTests : CardCollectionEditorViewModelTestsBase, IUnsavedChangesCheckTests, IOpenCommandTests
  {
    [TestMethod]
    public async Task Execute_HasUnsavedChanges_UnsavedChangesConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
        },
        HasUnsavedChanges = true
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.OpenCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Cancel_HasUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel) },
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
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) },
          LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult<string?>(_savedCollection.Name) }
        },
        HasUnsavedChanges = true
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Accept_SaveConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          CardCollectionConfirmers = new()
          {
            SaveCollectionConfirmer = new TestExceptionConfirmer<string, string>()
          }
        },
        HasUnsavedChanges = true
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.OpenCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Open_OpenConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          LoadCollectionConfirmer = new TestExceptionConfirmer<string, IEnumerable<string>>()
        },
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.OpenCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Open_Cancel_NoChanges()
    {
      var viewmodel = await new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult<string?>(null) }
        },
      }.MockVM(_savedCollection);

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(_savedCollection.Name, viewmodel.CardCollectionViewModel.Name);
    }

    [TestMethod]
    public async Task Open_Success_Changed()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
        },
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(_savedCollection.Name, viewmodel.CardCollectionViewModel.Name);
      Assert.AreEqual(_savedCollection.CollectionLists.Count, viewmodel.CardCollectionViewModel.CollectionLists.Count);
      CollectionAssert.AreEquivalent(
        _savedCollection.CollectionLists.SelectMany(l => l.Cards.Select(c => c.Info.Name)).ToList(),
        viewmodel.CardCollectionViewModel.CollectionLists.SelectMany(l => l.Cards.Select(c => c.Info.Name)).ToList());
    }

    [TestMethod]
    public async Task Open_Success_SuccessNotificationSent()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
        },
        Notifier = new()
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      }.MockVM();

      await NotificationAssert.NotificationSent(NotificationType.Success, () => viewmodel.OpenCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Open_Error_ErrorNotificationSent()
    {
      _dependencies.Repository.GetFailure = true;

      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
        },
        Notifier = new()
        {
          OnNotify = (arg) => throw new NotificationException(arg)
        }
      }.MockVM();

      await NotificationAssert.NotificationSent(NotificationType.Error, () => viewmodel.OpenCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Open_Success_HasNoUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) },
          LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
        },
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }
  }
}

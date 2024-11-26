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
      var confirmer = new TestConfirmer<ConfirmationResult>();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = confirmer
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
      var confirmer = new TestConfirmer<string, string>();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          CardCollectionConfirmers = new()
          {
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
          LoadCollectionConfirmer = confirmer
        },
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
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
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
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
          LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
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
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) },
          LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
        },
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }
  }
}

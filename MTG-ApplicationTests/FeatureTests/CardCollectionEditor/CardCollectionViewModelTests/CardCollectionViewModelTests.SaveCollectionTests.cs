using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class SaveCollectionTests : CardCollectionViewModelTestsBase, ISaveCommandTests
  {
    [TestMethod]
    public async Task Save_SaveConfirmationShown()
    {
      var confirmer = new TestConfirmer<string, string>();
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        Confirmers = new()
        {
          SaveCollectionConfirmer = confirmer
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task Save_Cancel_NotSaved()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Model = new() { Name = "New", CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult<string>(null) },
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(await _dependencies.Repository.Exists(viewmodel.Name));
    }

    [TestMethod]
    public async Task Save_WithEmptyName_NotSaved()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(string.Empty) },
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(_savedCollection.Name, viewmodel.Name);
    }

    [TestMethod]
    public async Task Save_NewName_Saved()
    {
      var collection = new MTGCardCollection() { Name = "New", CollectionLists = [new()] };
      var viewmodel = new Mocker(_dependencies)
      {
        Model = collection,
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(collection.Name) },
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      Assert.IsTrue(await _dependencies.Repository.Exists(collection.Name));
      Assert.AreEqual(1, (await _dependencies.Repository.Get(collection.Name))?.CollectionLists.Count);
    }

    [TestMethod]
    public async Task Save_SameName_NoOverrideConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
          OverrideCollectionConfirmer = new() { OnConfirm = async msg => { Assert.Fail(); return await Task.FromResult(ConfirmationResult.Failure); } }
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);
    }

    [TestMethod]
    public async Task Save_Override_OverrideConfirmationShown()
    {
      var confirmer = new TestConfirmer<ConfirmationResult>();
      var viewmodel = new Mocker(_dependencies)
      {
        Model = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
          OverrideCollectionConfirmer = confirmer
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task Save_Override_Cancel_NotSaved()
    {
      var collection = new MTGCardCollection() { Name = "New", CollectionLists = [new()] };
      var viewmodel = new Mocker(_dependencies)
      {
        Model = collection,
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
          OverrideCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel) }
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(await _dependencies.Repository.Exists(collection.Name));
    }

    [TestMethod]
    public async Task Save_Override_Accept_Overridden()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Model = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
          OverrideCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) }
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      Assert.IsTrue(await _dependencies.Repository.Exists(_savedCollection.Name));
      Assert.AreEqual(0, (await _dependencies.Repository.Get(_savedCollection.Name))?.CollectionLists?.FirstOrDefault()?.Cards.Count);
    }

    [TestMethod]
    public async Task Save_Renamed_OldDeleted()
    {
      var newName = "New";
      var viewmodel = new Mocker(_dependencies)
      {
        Model = new() { Name = _savedCollection.Name, CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(newName) },
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      Assert.IsTrue(await _dependencies.Repository.Exists(newName));
      Assert.IsFalse(await _dependencies.Repository.Exists(_savedCollection.Name));
    }

    [TestMethod]
    public async Task Save_Renamed_NameChanged()
    {
      var newName = "New";
      var viewmodel = new Mocker(_dependencies)
      {
        Model = new() { Name = _savedCollection.Name, CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(newName) },
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(newName, viewmodel.Name);
    }

    [TestMethod]
    public async Task Save_Success_NoUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Save_Success_SuccessNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
        },
        Notifier = notifier
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Success, notifier);
    }

    [TestMethod]
    public async Task Save_Error_ErrorNotificationSent()
    {
      _dependencies.Repository.UpdateFailure = true;

      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
        },
        Notifier = notifier
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task Save_SaveEmpty_Success()
    {
      var newName = "Name";
      var viewmodel = new Mocker(_dependencies)
      {
        Model = new() { CollectionLists = [] },
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(newName) }
        },
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(viewmodel.Name, newName);
    }
  }
}

using MTGApplication.Features.CardCollectionEditor.CardCollection.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Editor.ViewModels;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Editor.UseCases;

[TestClass]
public class SaveCollection : CardCollectionEditorViewModelTestBase
{
  [TestMethod]
  public async Task Save_SaveConfirmationShown()
  {
    var confirmer = new TestConfirmer<string, string>();
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = confirmer
        }
      }
    }.MockVM(_savedCollection);

    await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod]
  public async Task Save_Cancel_NotSaved()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult<string>(null) },
        }
      }
    }.MockVM(new() { Name = "New", CollectionLists = [new()] });

    await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

    Assert.IsFalse(await _dependencies.Repository.Exists(viewmodel.Collection.Name));
  }

  [TestMethod]
  public async Task Save_WithEmptyName_NotSaved()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(string.Empty) },
        }
      }
    }.MockVM(_savedCollection);

    await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

    Assert.AreEqual(_savedCollection.Name, viewmodel.Collection.Name);
  }

  [TestMethod]
  public async Task Save_NewName_Saved()
  {
    var collection = new CardCollectionEditorCardCollection() { Name = "New", CollectionLists = [new()] };
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(collection.Name) },
        }
      }
    }.MockVM(collection);

    await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

    Assert.IsTrue(await _dependencies.Repository.Exists(collection.Name));
    Assert.HasCount(1, (await _dependencies.Repository.Get(collection.Name)).CollectionLists);
  }

  [TestMethod]
  public async Task Save_SameName_NoOverrideConfirmationShown()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
          OverrideCollectionConfirmer = new() { OnConfirm = async msg => { Assert.Fail(); return await Task.FromResult(ConfirmationResult.Failure); } }
        }
      }
    }.MockVM(_savedCollection);

    await viewmodel.SaveCollectionCommand.ExecuteAsync(null);
  }

  [TestMethod]
  public async Task Save_Override_OverrideConfirmationShown()
  {
    var confirmer = new TestConfirmer<ConfirmationResult>();
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
          OverrideCollectionConfirmer = confirmer
        }
      }
    }.MockVM(new() { CollectionLists = [new()] });

    await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod]
  public async Task Save_Override_Cancel_NotSaved()
  {
    var collection = new CardCollectionEditorCardCollection() { Name = "New", CollectionLists = [new()] };
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
          OverrideCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel) }
        }
      }
    }.MockVM(collection);

    await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

    Assert.IsFalse(await _dependencies.Repository.Exists(collection.Name));
  }

  [TestMethod]
  public async Task Save_Override_Accept_Overridden()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
          OverrideCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) }
        }
      }
    }.MockVM(new() { CollectionLists = [new()] });

    await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

    Assert.IsTrue(await _dependencies.Repository.Exists(_savedCollection.Name));
    Assert.IsEmpty((await _dependencies.Repository.Get(_savedCollection.Name)).CollectionLists.First().Cards);
  }

  [TestMethod]
  public async Task Save_Renamed_OldDeleted()
  {
    var newName = "New";
    var viewmodel = new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(newName) },
        }
      }
    }.MockVM(new() { Name = _savedCollection.Name, CollectionLists = [new()] });

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
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(newName) },
        }
      }
    }.MockVM(new() { Name = _savedCollection.Name, CollectionLists = [new()] });

    await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

    Assert.AreEqual(newName, viewmodel.Collection.Name);
  }

  [TestMethod]
  public async Task Save_Success_NoUnsavedChanges()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
        }
      }
    }.MockVM(_savedCollection);

    await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

    Assert.IsFalse(viewmodel.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Save_Success_SuccessNotificationSent()
  {
    var notifier = new TestNotifier();
    var viewmodel = new Mocker(_dependencies)
    {
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
        }
      },
      Notifier = notifier
    }.MockVM(_savedCollection);

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
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
        }
      },
      Notifier = notifier
    }.MockVM(_savedCollection);

    await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Error, notifier);
  }

  [TestMethod]
  public async Task Save_SaveEmpty_Success()
  {
    var newName = "Name";
    var viewmodel = new Mocker(_dependencies)
    {
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(newName) }
        }
      },
    }.MockVM(new() { CollectionLists = [] });

    await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

    Assert.AreEqual(viewmodel.Collection.Name, newName);
  }
}
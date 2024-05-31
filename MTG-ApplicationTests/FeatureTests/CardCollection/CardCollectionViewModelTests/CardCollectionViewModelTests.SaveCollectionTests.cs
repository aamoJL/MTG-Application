using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.CardCollection;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class SaveCollectionTests : CardCollectionViewModelTestsBase
  {
    [TestMethod]
    public void Save_HasNoLists_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM();

      Assert.IsFalse(viewmodel.SaveCollectionCommand.CanExecute(null));
    }

    [TestMethod]
    public void Save_HasLists_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection
      }.MockVM();

      Assert.IsTrue(viewmodel.SaveCollectionCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task Save_SaveConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          SaveCollectionConfirmer = new TestExceptionConfirmer<string, string>()
        }
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.SaveCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Save_WithEmptyName_NotSaved()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(string.Empty) },
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(_savedCollection.Name, viewmodel.Collection.Name);
    }

    [TestMethod]
    public async Task Save_Cancel_NotSaved()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { Name = "New", CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult<string?>(null) },
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(await _dependencies.Repository.Exists(viewmodel.Collection.Name));
    }

    [TestMethod]
    public async Task Save_NameExists_OverrideConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) },
          OverrideCollectionConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
        }
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.SaveCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Save_CancelOverride_NotSaved()
    {
      var collection = new MTGCardCollection() { Name = "New", CollectionLists = [new()] };
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = collection,
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
    public async Task Save_AcceptOverride_Saved()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
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
    public async Task Save_DoesNotExist_Saved()
    {
      var collection = new MTGCardCollection() { Name = "New", CollectionLists = [new()] };
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = collection,
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
    public async Task Save_Rename_OldDeleted()
    {
      var newName = "New";
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { Name = _savedCollection.Name, CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(newName) },
        }
      }.MockVM();

      await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(newName, viewmodel.Collection.Name);
      Assert.IsTrue(await _dependencies.Repository.Exists(newName));
      Assert.IsFalse(await _dependencies.Repository.Exists(_savedCollection.Name));
    }
  }
}

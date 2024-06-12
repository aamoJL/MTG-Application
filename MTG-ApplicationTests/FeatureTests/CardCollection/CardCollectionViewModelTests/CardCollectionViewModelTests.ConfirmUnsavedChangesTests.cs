using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class ConfirmUnsavedChangesTests : CardCollectionViewModelTestsBase, IConfirmUnsavedChangesTests
  {
    [TestMethod]
    public async Task NoUnsavedChanges_Success()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        HasUnsavedChanges = false,
      }.MockVM();

      var args = new ISavable.ConfirmArgs();
      await viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(args);

      Assert.IsFalse(args.Cancelled);
    }

    [TestMethod]
    public async Task ConfirmUnsavedChanges_UnsavedChangesConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new TestExceptionConfirmer<ConfirmationResult>(),
        },
      }.MockVM();

      var args = new ISavable.ConfirmArgs();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(args));
    }

    [TestMethod]
    public async Task ConfirmUnsavedChanges_Accept_SaveConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveCollectionConfirmer = new TestExceptionConfirmer<string, string>(),
        },
      }.MockVM();

      var args = new ISavable.ConfirmArgs();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(args));
    }

    [TestMethod]
    public async Task ConfirmUnsavedChanges_Decline_Success()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) },
        },
      }.MockVM();

      var args = new ISavable.ConfirmArgs();
      await viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(args);

      Assert.IsFalse(args.Cancelled);
    }

    [TestMethod]
    public async Task ConfirmUnsavedChanges_Cancel_Canceled()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel) },
        },
      }.MockVM();

      var args = new ISavable.ConfirmArgs();
      await viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(args);

      Assert.IsTrue(args.Cancelled);
    }

    [TestMethod]
    public async Task ConfirmUnsavedChanges_Accept_Save_Success()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult("New") },
        },
      }.MockVM();

      var args = new ISavable.ConfirmArgs();
      await viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(args);

      Assert.IsFalse(args.Cancelled);
    }

    [TestMethod]
    public async Task ConfirmUnsavedChanges_Accept_DeclineSave_Canceled()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(string.Empty) },
        },
      }.MockVM();

      var args = new ISavable.ConfirmArgs();
      await viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(args);

      Assert.IsTrue(args.Cancelled);
    }

    [TestMethod]
    public async Task ConfirmUnsavedChanges_Accept_CancelSave_Canceled()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult<string?>(null) },
        },
      }.MockVM();

      var args = new ISavable.ConfirmArgs();
      await viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(args);

      Assert.IsTrue(args.Cancelled);
    }
  }
}

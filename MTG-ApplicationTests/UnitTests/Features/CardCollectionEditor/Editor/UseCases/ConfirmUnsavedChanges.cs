using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Editor.ViewModels;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Editor.UseCases;

[TestClass]
public class ConfirmUnsavedChanges : CardCollectionEditorViewModelTestBase, IConfirmUnsavedChangesTests
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
    var confirmer = new TestConfirmer<ConfirmationResult>();
    var viewmodel = new Mocker(_dependencies)
    {
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveUnsavedChangesConfirmer = confirmer,
        }
      },
    }.MockVM();

    var args = new ISavable.ConfirmArgs();

    await viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(args);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod]
  public async Task ConfirmUnsavedChanges_Accept_SaveConfirmationShown()
  {
    var confirmer = new TestConfirmer<string, string>();
    var viewmodel = new Mocker(_dependencies)
    {
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveCollectionConfirmer = confirmer,
        }
      },
    }.MockVM();

    var args = new ISavable.ConfirmArgs();

    await viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(args);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod]
  public async Task ConfirmUnsavedChanges_Decline_Success()
  {
    var viewmodel = new Mocker(_dependencies)
    {
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) },
        }
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
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel) },
        }
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
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult("New") },
        }
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
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(string.Empty) },
        }
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
      HasUnsavedChanges = true,
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult<string>(null) },
        }
      },
    }.MockVM();

    var args = new ISavable.ConfirmArgs();
    await viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(args);

    Assert.IsTrue(args.Cancelled);
  }
}
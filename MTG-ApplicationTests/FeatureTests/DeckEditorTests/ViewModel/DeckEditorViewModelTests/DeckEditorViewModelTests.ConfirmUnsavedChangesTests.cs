using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class ConfirmUnsavedChangesTests : DeckEditorViewModelTestsBase, IConfirmUnsavedChangesTests
  {
    [TestMethod]
    public async Task NoUnsavedChanges_ReturnTrue()
    {
      var viewmodel = new Mocker(_dependencies) { HasUnsavedChanges = false }.MockVM();

      Assert.IsTrue(await viewmodel.ConfirmUnsavedChanges());
    }

    [TestMethod]
    public async Task SaveCommandCanNotExecute_ReturnTrue()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = new() { DeckCards = [] },
        HasUnsavedChanges = true,
      }.MockVM();

      Assert.IsTrue(await viewmodel.ConfirmUnsavedChanges());
    }

    [TestMethod]
    public async Task CanSave_UnsavedChangesConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new TestExceptionConfirmer<ConfirmationResult>(),
        },
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(viewmodel.ConfirmUnsavedChanges);
    }

    [TestMethod]
    public async Task CanSave_AcceptUnsavedSave_SaveConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveDeckConfirmer = new TestExceptionConfirmer<string, string>(),
        },
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(viewmodel.ConfirmUnsavedChanges);
    }

    [TestMethod]
    public async Task CanSave_DeclineUnsavedSave_ReturnTrue()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) },
        },
      }.MockVM();

      Assert.IsTrue(await viewmodel.ConfirmUnsavedChanges());
    }

    [TestMethod]
    public async Task CanSave_CancelUnsavedSave_ReturnFalse()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel) },
        },
      }.MockVM();

      Assert.IsFalse(await viewmodel.ConfirmUnsavedChanges());
    }

    [TestMethod]
    public async Task CanSave_AcceptUnsavedSave_Save_ReturnTrue()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveDeckConfirmer = new() { OnConfirm = async msg => await Task.FromResult("New") },
        },
      }.MockVM();

      Assert.IsTrue(await viewmodel.ConfirmUnsavedChanges());
    }

    [TestMethod]
    public async Task CanSave_AcceptUnsavedSave_DeclineSave_ReturnFalse()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveDeckConfirmer = new() { OnConfirm = async msg => await Task.FromResult(string.Empty) },
        },
      }.MockVM();

      Assert.IsFalse(await viewmodel.ConfirmUnsavedChanges());
    }

    [TestMethod]
    public async Task CanSave_AcceptUnsavedSave_CancelSave_ReturnFalse()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveDeckConfirmer = new() { OnConfirm = async msg => await Task.FromResult<string?>(null) },
        },
      }.MockVM();

      Assert.IsFalse(await viewmodel.ConfirmUnsavedChanges());
    }
  }
}


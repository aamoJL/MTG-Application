using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.FeatureTests.CardCollectionEditor.CardCollectionEditorViewModelTests;

public partial class CardCollectionEditorTests
{
  [TestClass]
  public class NewCollectionTests : CardCollectionEditorViewModelTestsBase, IUnsavedChangesCheckTests, INewCommandTests
  {
    [TestMethod]
    public async Task Execute_HasUnsavedChanges_UnsavedChangesConfirmationShown()
    {
      var confirmer = new TestConfirmer<ConfirmationResult>();
      var viewmodel = new Mocker(_dependencies)
      {
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = confirmer
        }
      }.MockVM();

      await viewmodel.NewCollectionCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Cancel_HasUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel) },
        }
      }.MockVM();

      await viewmodel.NewCollectionCommand.ExecuteAsync(null);

      Assert.IsTrue(viewmodel.HasUnsavedChanges);
    }

    [TestMethod("Should not have unsaved changes if the collection has been changed to a new collection")]
    public async Task Execute_HasUnsavedChanges_Decline_NoUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) },
        }
      }.MockVM();

      await viewmodel.NewCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Accept_SaveConfirmationShown()
    {
      var confirmer = new TestConfirmer<string, string>();
      var viewmodel = new Mocker(_dependencies)
      {
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          CardCollectionConfirmers = new()
          {
            SaveCollectionConfirmer = confirmer
          }
        }
      }.MockVM();

      await viewmodel.NewCollectionCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod("Collection should be changed to a new collection when executed successfully")]
    public async Task New_Success_Reset()
    {
      var viewmodel = await new Mocker(_dependencies).MockVM(collection: new()
      {
        Name = "Collection",
        CollectionLists = [new() { Name = "List" }]
      });

      Assert.IsTrue(viewmodel.CardCollectionViewModel.Name != string.Empty);
      Assert.IsTrue(viewmodel.CardCollectionViewModel.CollectionLists.First().Name != string.Empty);

      await viewmodel.NewCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(string.Empty, viewmodel.CardCollectionViewModel.Name);
      Assert.AreEqual(0, viewmodel.CardCollectionViewModel.CollectionLists.Count);
    }
  }
}

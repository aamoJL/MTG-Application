using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardCollection;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class NewCollectionTests
  {
    [TestMethod]
    public async Task NewCollection_HasUnsavedChanges_UnsavedChangesConfirmationShown()
    {
      var viewmodel = new CardCollectionViewModel(new TestCardAPI())
      {
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
        }
      };

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.NewCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod("ViewModel should not have unsaved changes if the collection has been changed to new collection")]
    public async Task NewCollection_HasUnsavedChanges_NoSave_HasNoUnsavedChanges()
    {
      var viewmodel = new CardCollectionViewModel(new TestCardAPI())
      {
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) },
        }
      };

      await viewmodel.NewCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task NewCollection_HasNoUnsavedChanges_CollectionReset()
    {
      var viewmodel = new CardCollectionViewModel(new TestCardAPI())
      {
        Collection = new()
        {
          Name = "Collection",
          CollectionLists = [new() { Name = "List" }]
        }
      };

      Assert.IsTrue(viewmodel.Collection.Name != string.Empty);
      Assert.IsTrue(viewmodel.Collection.CollectionLists.First().Name != string.Empty);

      await viewmodel.NewCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(string.Empty, viewmodel.Collection.Name);
      Assert.AreEqual(0, viewmodel.Collection.CollectionLists.Count);
    }
  }
}

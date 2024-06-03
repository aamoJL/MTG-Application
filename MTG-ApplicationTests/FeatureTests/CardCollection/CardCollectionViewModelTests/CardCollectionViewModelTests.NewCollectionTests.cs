using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardCollection;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class NewCollectionTests : IUnsavedChangesCheckTests, INewCommandTests
  {
    [TestMethod]
    public async Task Execute_HasUnsavedChanges_UnsavedChangesConfirmationShown()
    {
      var viewmodel = new CardCollectionViewModel(new TestCardAPI())
      {
        HasUnsavedChanges = true,
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
        }
      };

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.NewCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Cancel_HasUnsavedChanges()
    {
      var viewmodel = new CardCollectionViewModel(new TestCardAPI())
      {
        HasUnsavedChanges = true,
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel) },
        }
      };

      await viewmodel.NewCollectionCommand.ExecuteAsync(null);

      Assert.IsTrue(viewmodel.HasUnsavedChanges);
    }

    [TestMethod("Should not have unsaved changes if the collection has been changed to a new collection")]
    public async Task Execute_HasUnsavedChanges_Decline_NoUnsavedChanges()
    {
      var viewmodel = new CardCollectionViewModel(new TestCardAPI())
      {
        HasUnsavedChanges = true,
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) },
        }
      };

      await viewmodel.NewCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Accept_SaveConfirmationShown()
    {
      var viewmodel = new CardCollectionViewModel(new TestCardAPI())
      {
        HasUnsavedChanges = true,
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveCollectionConfirmer = new TestExceptionConfirmer<string, string>()
        }
      };

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.NewCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod("Collection should be changed to a new collection when executed successfully")]
    public async Task Execute_Reset()
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

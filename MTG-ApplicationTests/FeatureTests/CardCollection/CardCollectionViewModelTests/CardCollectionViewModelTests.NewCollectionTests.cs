using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardCollection;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;
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
    public async Task New_Success_Reset()
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

    [TestMethod]
    public async Task New_Success_QueryCardsReset()
    {
      var expectedCards = new MTGCard[]
      {
        MTGCardModelMocker.CreateMTGCardModel(name: "1"),
        MTGCardModelMocker.CreateMTGCardModel(name: "2"),
        MTGCardModelMocker.CreateMTGCardModel(name: "3"),
        MTGCardModelMocker.CreateMTGCardModel(name: "4"),
      };
      var viewmodel = new CardCollectionViewModel(new TestCardAPI() { ExpectedCards = expectedCards })
      {
        Collection = new()
        {
          CollectionLists = [new() { Name = "List", Cards = [.. expectedCards], SearchQuery = "asd" }]
        }
      };

      await viewmodel.SelectListCommand.ExecuteAsync(viewmodel.Collection.CollectionLists[0].Name);
      await viewmodel.QueryCards.Collection.LoadMoreItemsAsync((uint)expectedCards.Length);

      CollectionAssert.AreEquivalent(
        expectedCards.Select(x => x.Info.Name).ToList(),
        viewmodel.QueryCards.Collection.Select(x => x.Info.Name).ToList());
    }
  }
}

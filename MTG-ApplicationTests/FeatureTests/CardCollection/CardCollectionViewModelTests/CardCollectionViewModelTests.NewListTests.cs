using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class NewListTests : CardCollectionViewModelTestsBase
  {
    [TestMethod]
    public async Task NewList_NewListConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          NewCollectionListConfirmer = new TestExceptionConfirmer<(string, string)?>(),
        }
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.NewListCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task NewList_Cancel_NoChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(null) },
        }
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      Assert.AreEqual(0, viewmodel.Collection.CollectionLists.Count);
    }

    [TestMethod]
    public async Task NewList_NoName_NoChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>((string.Empty, "Query")) },
        }
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      Assert.AreEqual(0, viewmodel.Collection.CollectionLists.Count);
    }

    [TestMethod]
    public async Task NewList_NoQuery_NoChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(("Name", string.Empty)) },
        }
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      Assert.AreEqual(0, viewmodel.Collection.CollectionLists.Count);
    }

    [TestMethod]
    public async Task NewList_Exists_NoChanges()
    {
      var list = _savedCollection.CollectionLists[0];
      var newQuery = "New Query";
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>((list.Name, newQuery)) },
        }
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      Assert.IsNull(viewmodel.Collection.CollectionLists.FirstOrDefault(x => x.SearchQuery == newQuery));
    }

    [TestMethod]
    public async Task NewList_Success_ListAdded()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(("Name", "Query")) },
        }
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      Assert.AreEqual(1, viewmodel.Collection.CollectionLists.Count);
    }

    [TestMethod]
    public async Task NewList_EmptyName_ErrorNotificationSent()
    {
      throw new NotImplementedException();
    }

    [TestMethod]
    public async Task NewList_EmptyQuery_ErrorNotificationSent()
    {
      throw new NotImplementedException();
    }

    [TestMethod]
    public async Task NewList_Cancel_NoNotificationSent()
    {
      throw new NotImplementedException();
    }

    [TestMethod]
    public async Task NewList_Exists_ErrorNotificationSent()
    {
      throw new NotImplementedException();
    }

    [TestMethod]
    public async Task NewList_Success_SuccessNotificationSent()
    {
      throw new NotImplementedException();
    }
  }
}

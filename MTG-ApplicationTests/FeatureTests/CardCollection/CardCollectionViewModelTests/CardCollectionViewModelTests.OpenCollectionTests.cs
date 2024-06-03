using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class OpenCollectionTests : CardCollectionViewModelTestsBase,
    IUnsavedChangesCheckTests, IOpenCommandTests
  {
    [TestMethod]
    public async Task Execute_HasUnsavedChanges_UnsavedChangesConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
        },
        HasUnsavedChanges = true
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.OpenCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Cancel_HasUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel) },
        },
        HasUnsavedChanges = true
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.IsTrue(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Decline_NoUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) },
          LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult<string?>(_savedCollection.Name) }
        },
        HasUnsavedChanges = true
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Accept_SaveConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveCollectionConfirmer = new TestExceptionConfirmer<string, string>()
        },
        HasUnsavedChanges = true
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.OpenCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Open_OpenConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          LoadCollectionConfirmer = new TestExceptionConfirmer<string, IEnumerable<string>>()
        },
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.OpenCollectionCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Open_Cancel_NoChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult<string?>(null) }
        },
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(_savedCollection, viewmodel.Collection);
    }

    [TestMethod]
    public async Task Open_Success_Changed()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
        },
      }.MockVM();

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);

      Assert.AreEqual(_savedCollection.Name, viewmodel.Collection.Name);
      Assert.AreEqual(_savedCollection.CollectionLists.Count, viewmodel.Collection.CollectionLists.Count);
      CollectionAssert.AreEquivalent(
        _savedCollection.CollectionLists.SelectMany(l => l.Cards.Select(c => c.Info.Name)).ToList(),
        viewmodel.Collection.CollectionLists.SelectMany(l => l.Cards.Select(c => c.Info.Name)).ToList());
    }

    [TestMethod]
    public async Task Open_Valid_QueryCardsUpdated()
    {
      var expectedCards = new MTGCard[]
      {
        MTGCardModelMocker.CreateMTGCardModel(name: "1"),
        MTGCardModelMocker.CreateMTGCardModel(name: "2"),
        MTGCardModelMocker.CreateMTGCardModel(name: "3"),
      };
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          LoadCollectionConfirmer = new() { OnConfirm = async msg => await Task.FromResult(_savedCollection.Name) }
        },
      }.MockVM();

      _dependencies.CardAPI.ExpectedCards = expectedCards;

      await viewmodel.OpenCollectionCommand.ExecuteAsync(null);
      await viewmodel.QueryCards.Collection.LoadMoreItemsAsync((uint)expectedCards.Length);

      CollectionAssert.AreEquivalent(
        expectedCards.Select(c => c.Info.Name).ToList(),
        viewmodel.QueryCards.Collection.Select(c => c.Info.Name).ToList());
    }

    [TestMethod]
    public Task Open_Success_SuccessNotificationSent()
    {
      throw new NotImplementedException();
    }

    [TestMethod]
    public Task Open_Error_ErrorNotificationSent()
    {
      throw new NotImplementedException();
    }
  }
}

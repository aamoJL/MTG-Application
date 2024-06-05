using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.CardCollection;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class EditListTests : CardCollectionViewModelTestsBase, ICanExecuteCommandTests
  {
    [TestMethod("Should not be able to execute if the selected list is null")]
    public void InvalidState_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM();

      Assert.IsFalse(viewmodel.EditListCommand.CanExecute(null));
    }

    [TestMethod("Should be able to execute if the selected list is not null")]
    public void ValidState_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
      }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      Assert.IsTrue(viewmodel.EditListCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task Edit_EditConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          EditCollectionListConfirmer = new TestExceptionConfirmer<(string, string)?, (string, string)>(),
        }
      }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.EditListCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Edit_Cancel_NoChanges()
    {
      var searchQuery = _savedCollection.CollectionLists.First().SearchQuery;
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          EditCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<(string, string)?>(null)
          }
        }
      }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await viewmodel.EditListCommand.ExecuteAsync(null);

      Assert.AreEqual(searchQuery, viewmodel.Collection.CollectionLists.First().SearchQuery);
    }

    [TestMethod]
    public async Task Edit_Success_NameChanged()
    {
      var name = "New Name";
      var query = "New Query";
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          EditCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult((name, query))
          }
        }
      }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await viewmodel.EditListCommand.ExecuteAsync(null);

      Assert.AreEqual(name, viewmodel.SelectedList.Name);
    }

    [TestMethod]
    public async Task Edit_Success_QueryChanged()
    {
      var name = "New Name";
      var query = "New Query";
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          EditCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult((name, query))
          }
        }
      }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await viewmodel.EditListCommand.ExecuteAsync(null);

      Assert.AreEqual(query, viewmodel.SelectedList.SearchQuery);
    }

    [TestMethod]
    public async Task Edit_Success_QueryCardsUpdated()
    {
      var name = "New Name";
      var query = "New Query";
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          EditCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult((name, query))
          }
        }
      }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();
      await viewmodel.QueryCards.Collection.LoadMoreItemsAsync(10);

      Assert.AreEqual(0, viewmodel.QueryCards.Collection.Count);

      _dependencies.CardAPI.ExpectedCards = [MTGCardModelMocker.CreateMTGCardModel(name: "Card")];

      await viewmodel.EditListCommand.ExecuteAsync(null);
      await viewmodel.QueryCards.Collection.LoadMoreItemsAsync(10);

      Assert.AreEqual(1, viewmodel.QueryCards.Collection.Count);
    }

    [TestMethod]
    public async Task Edit_NoName_ErrorNotificationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          EditCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult((string.Empty, "New Query"))
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await NotificationAssert.NotificationSent(NotificationType.Error,
        () => viewmodel.EditListCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Edit_NoQuery_ErrorNotificationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          EditCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(("New Name", string.Empty))
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await NotificationAssert.NotificationSent(NotificationType.Error,
        () => viewmodel.EditListCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Edit_Exists_ErrorNotificationShown()
    {
      var collection = new MTGCardCollection()
      {
        Name = "Collection",
        CollectionLists = [
            new() { Name = "List" },
            new() { Name = "List 2" },
          ]
      };
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = collection,
        Confirmers = new()
        {
          EditCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult((collection.CollectionLists[1].Name, "New Query"))
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await NotificationAssert.NotificationSent(NotificationType.Error,
        () => viewmodel.EditListCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Edit_SameName_NoErrorNotificationShown()
    {
      var name = _savedCollection.CollectionLists.First().Name;
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          EditCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult((name, "New Query"))
          }
        },
        Notifier = new()
        {
          OnNotify = msg =>
          {
            if (msg.NotificationType == NotificationType.Error)
              Assert.Fail();
          }
        }
      }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await viewmodel.EditListCommand.ExecuteAsync(null);
    }

    [TestMethod]
    public async Task Edit_Success_SuccessNotificationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          EditCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(("New Name", "New Query"))
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => viewmodel.EditListCommand.ExecuteAsync(null));
    }
  }
}

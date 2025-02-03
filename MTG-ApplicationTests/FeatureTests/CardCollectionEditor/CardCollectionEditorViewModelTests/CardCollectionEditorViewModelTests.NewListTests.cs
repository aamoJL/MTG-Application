using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;
using static MTGApplicationTests.FeatureTests.CardCollectionEditor.CardCollectionEditorViewModelTests.CardCollectionEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionEditorViewModelTests
{
  [TestClass]
  public class NewListTests : CardCollectionEditorViewModelTestsBase
  {
    [TestMethod]
    public async Task NewList_NewListConfirmationShown()
    {
      var confirmer = new TestConfirmer<(string, string)?>();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            NewCollectionListConfirmer = confirmer,
          }
        }
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task NewList_Cancel_NoChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(null) },
          }
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
          CardCollectionConfirmers = new()
          {
            NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>((string.Empty, "Query")) },
          }
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
          CardCollectionConfirmers = new()
          {
            NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(("Name", string.Empty)) },
          }
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
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>((list.Name, newQuery)) },
          }
        }
      }.MockVM(_savedCollection);

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
          CardCollectionConfirmers = new()
          {
            NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(("Name", "Query")) },
          }
        }
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      Assert.AreEqual(1, viewmodel.Collection.CollectionLists.Count);
    }

    [TestMethod]
    public async Task NewList_Success_HasUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(("Name", "Query")) },
          }
        }
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      Assert.IsTrue(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task NewList_NoName_ErrorNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>((string.Empty, "Query")) },
          }
        },
        Notifier = notifier
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task NewList_NoQuery_ErrorNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(("Name", string.Empty)) },
          }
        },
        Notifier = notifier
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task NewList_Cancel_NoNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(null) },
          }
        },
        Notifier = notifier
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      NotificationAssert.NotificationNotSent(notifier);
    }

    [TestMethod]
    public async Task NewList_Exists_ErrorNotificationSent()
    {
      var list = _savedCollection.CollectionLists[0];
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            NewCollectionListConfirmer = new()
            {
              OnConfirm = async msg => await Task.FromResult<(string, string)?>((list.Name, "New Query"))
            },
          }
        },
        Notifier = notifier
      }.MockVM(_savedCollection);

      await viewmodel.NewListCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task NewList_Success_SuccessNotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionConfirmers = new()
          {
            NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(("Name", "Query")) },
          }
        },
        Notifier = notifier
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Success, notifier);
    }
  }
}

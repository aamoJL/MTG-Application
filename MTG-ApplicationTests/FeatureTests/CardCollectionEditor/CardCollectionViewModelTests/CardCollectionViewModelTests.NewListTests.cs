﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class NewListTests : CardCollectionViewModelTestsBase
  {
    [TestMethod]
    public async Task NewList_NewListConfirmationShown()
    {
      var confirmer = new TestConfirmer<(string, string)?>();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          NewCollectionListConfirmer = confirmer,
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
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(null) },
        }
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      Assert.AreEqual(0, viewmodel.CollectionLists.Count);
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

      Assert.AreEqual(0, viewmodel.CollectionLists.Count);
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

      Assert.AreEqual(0, viewmodel.CollectionLists.Count);
    }

    [TestMethod]
    public async Task NewList_Exists_NoChanges()
    {
      var list = _savedCollection.CollectionLists[0];
      var newQuery = "New Query";
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        Confirmers = new()
        {
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>((list.Name, newQuery)) },
        }
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      Assert.IsNull(viewmodel.CollectionLists.FirstOrDefault(x => x.SearchQuery == newQuery));
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

      Assert.AreEqual(1, viewmodel.CollectionLists.Count);
    }

    [TestMethod]
    public async Task NewList_Success_HasUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(("Name", "Query")) },
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
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>((string.Empty, "Query")) },
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
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(("Name", string.Empty)) },
        },
        Notifier = notifier
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task NewList_Cancel_NoNotificationSent()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(null) },
        },
        Notifier = new()
        {
          OnNotify = (arg) => Assert.Fail(),
        }
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);
    }

    [TestMethod]
    public async Task NewList_Exists_ErrorNotificationSent()
    {
      var list = _savedCollection.CollectionLists[0];
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Model = _savedCollection,
        Confirmers = new()
        {
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>((list.Name, "New Query")) },
        },
        Notifier = notifier
      }.MockVM();

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
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(("Name", "Query")) },
        },
        Notifier = notifier
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Success, notifier);
    }

    [TestMethod]
    public async Task NewList_Success_OnListAddedInvoked()
    {
      MTGCardCollectionList invoked = null;
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          NewCollectionListConfirmer = new() { OnConfirm = async msg => await Task.FromResult<(string, string)?>(("Name", "Query")) },
        },
        OnListAdded = async (list) => { invoked = list; await Task.Yield(); }
      }.MockVM();

      await viewmodel.NewListCommand.ExecuteAsync(null);

      Assert.IsNotNull(invoked);
    }
  }
}

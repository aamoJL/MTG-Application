using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.UnitTests.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.CardCollectionList.UseCases;

[TestClass]
public class EditList : CardCollectionListViewModelTestBase
{
  public async Task InvalidState_CanNotExecute()
  {
    var viewmodel = await new Mocker(_dependencies).MockVM();

    viewmodel.Name = string.Empty;

    Assert.IsFalse(viewmodel.EditListCommand.CanExecute(null));
  }

  public async Task ValidState_CanExecute()
  {
    var viewmodel = await new Mocker(_dependencies).MockVM();

    viewmodel.Name = "Name";

    Assert.IsTrue(viewmodel.EditListCommand.CanExecute(null));
  }

  [TestMethod]
  public async Task EditList_EditConfirmationShown()
  {
    var confirmer = new TestConfirmer<(string, string)?, (string, string)>();
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        EditCollectionListConfirmer = confirmer,
      }
    }.MockVM();

    var list = viewmodel.CollectionList;

    await viewmodel.EditListCommand.ExecuteAsync(list);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod]
  public async Task EditList_Cancel_NoChanges()
  {
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        EditCollectionListConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult<(string, string)?>(null)
        }
      }
    }.MockVM();

    var list = viewmodel.CollectionList;
    var query = list.SearchQuery;

    await viewmodel.EditListCommand.ExecuteAsync(list);

    Assert.AreEqual(query, list.SearchQuery);
  }

  [TestMethod]
  public async Task EditList_Success_NameChanged()
  {
    var name = "New Name";
    var query = "New Query";
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        EditCollectionListConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult((name, query))
        }
      }
    }.MockVM();

    var list = viewmodel.CollectionList;

    await viewmodel.EditListCommand.ExecuteAsync(list);

    Assert.AreEqual(name, list.Name);
  }

  [TestMethod]
  public async Task EditList_Success_QueryChanged()
  {
    var name = "New Name";
    var query = "New Query";
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        EditCollectionListConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult((name, query))
        }
      }
    }.MockVM();

    var list = viewmodel.CollectionList;

    await viewmodel.EditListCommand.ExecuteAsync(list);

    Assert.AreEqual(query, list.SearchQuery);
  }

  [TestMethod]
  public async Task EditList_Success_QueryCardsUpdated()
  {
    var name = "New Name";
    var query = "New Query";
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        EditCollectionListConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult((name, query))
        }
      }
    }.MockVM();

    var list = viewmodel.CollectionList;

    Assert.AreEqual(0, viewmodel.TotalCount);

    _dependencies.Importer.ExpectedCards = [new(MTGCardInfoMocker.MockInfo(name: "Card"))];

    await viewmodel.EditListCommand.ExecuteAsync(list);

    Assert.AreEqual(1, viewmodel.TotalCount);
  }

  [TestMethod]
  public async Task EditList_NoName_ErrorNotificationShown()
  {
    var notifier = new TestNotifier();
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        EditCollectionListConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult((string.Empty, "New Query"))
        }
      },
      Notifier = notifier
    }.MockVM();

    var list = viewmodel.CollectionList;

    await viewmodel.EditListCommand.ExecuteAsync(list);

    NotificationAssert.NotificationSent(NotificationType.Error, notifier);
  }

  [TestMethod]
  public async Task EditList_NoQuery_ErrorNotificationShown()
  {
    var notifier = new TestNotifier();
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        EditCollectionListConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult(("New Name", string.Empty))
        }
      },
      Notifier = notifier
    }.MockVM();

    var list = viewmodel.CollectionList;

    await viewmodel.EditListCommand.ExecuteAsync(list);

    NotificationAssert.NotificationSent(NotificationType.Error, notifier);
  }

  [TestMethod]
  public async Task EditList_Exists_ErrorNotificationShown()
  {
    var name = _savedList.Name;

    var notifier = new TestNotifier();
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = new() { Name = "Name" },
      Confirmers = new()
      {
        EditCollectionListConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult((name, "New Query"))
        }
      },
      Notifier = notifier,
      NameValidator = (name) => false
    }.MockVM();

    var list = viewmodel.CollectionList;

    await viewmodel.EditListCommand.ExecuteAsync(list);

    NotificationAssert.NotificationSent(NotificationType.Error, notifier);
  }

  [TestMethod]
  public async Task EditList_SameName_NoErrorNotificationShown()
  {
    var name = _savedList.Name;
    var notifier = new TestNotifier();
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        EditCollectionListConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult((name, "New Query"))
        }
      },
      Notifier = notifier
    }.MockVM();

    await viewmodel.EditListCommand.ExecuteAsync(null);

    NotificationAssert.NotificationNotSent(NotificationType.Error, notifier);
  }

  [TestMethod]
  public async Task EditList_Success_SuccessNotificationShown()
  {
    var notifier = new TestNotifier();
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        EditCollectionListConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult(("New Name", "New Query"))
        }
      },
      Notifier = notifier
    }.MockVM();

    var list = viewmodel.CollectionList;

    await viewmodel.EditListCommand.ExecuteAsync(list);

    NotificationAssert.NotificationSent(NotificationType.Success, notifier);
  }

  [TestMethod]
  public async Task EditList_Conflict_ConflictConfirmationShown()
  {
    var confirmer = new TestConfirmer<ConfirmationResult>();
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        EditCollectionListConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult(("New Name", "New Query"))
        },
        EditCollectionListQueryConflictConfirmer = confirmer,
      }
    }.MockVM();

    var list = viewmodel.CollectionList;

    await viewmodel.EditListCommand.ExecuteAsync(list);

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod]
  public async Task EditList_Conflict_Cancel_NoChanges()
  {
    var list = _savedList;

    var oldName = list.Name;
    var oldQuery = list.SearchQuery;
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        EditCollectionListConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult(("New Name", "New Query"))
        },
        EditCollectionListQueryConflictConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel)
        },
      }
    }.MockVM();

    await viewmodel.EditListCommand.ExecuteAsync(list);

    Assert.AreEqual(oldName, list.Name);
    Assert.AreEqual(oldQuery, list.SearchQuery);
  }

  [TestMethod]
  public async Task EditList_Conflict_Accept_QueryAndNameChanged()
  {
    var name = "New Name";
    var query = "New Query";
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        EditCollectionListConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult((name, query))
        },
        EditCollectionListQueryConflictConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
        },
      }
    }.MockVM();

    var list = viewmodel.CollectionList;

    await viewmodel.EditListCommand.ExecuteAsync(list);

    Assert.AreEqual(name, list.Name);
    Assert.AreEqual(query, list.SearchQuery);
  }

  [TestMethod]
  public async Task EditList_Conflict_Accept_OwnedCardsChanged()
  {
    var list = _savedList;

    var expectedCards = new CardImportResult.Card[]
    {
        new(list.Cards[0].Info),
        new(MTGCardInfoMocker.MockInfo())
    };

    _dependencies.Importer.ExpectedCards = expectedCards;

    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        EditCollectionListConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult(("New Name", "New Query"))
        },
        EditCollectionListQueryConflictConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
        },
      }
    }.MockVM();

    CollectionAssert.AreNotEquivalent(
      new Guid[] { expectedCards.First().Info.ScryfallId },
      viewmodel.CollectionList.Cards.Select(x => x.Info.ScryfallId).ToArray());

    await viewmodel.EditListCommand.ExecuteAsync(list);

    CollectionAssert.AreEquivalent(
      new Guid[] { expectedCards.First().Info.ScryfallId },
      viewmodel.CollectionList.Cards.Select(x => x.Info.ScryfallId).ToArray());
  }
}

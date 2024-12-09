using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionListViewModelTests
{
  [TestClass]
  public class EditListTests : CardCollectionListViewModelTestsBase, ICanExecuteCommandAsyncTests
  {
    [TestMethod("Should not be able to execute if the list does not have a name")]
    public async Task InvalidState_CanNotExecute()
    {
      var viewmodel = await new Mocker(_dependencies).MockVM();

      Assert.IsFalse(viewmodel.EditListCommand.CanExecute(null));
    }

    [TestMethod("Should be able to execute if the list has a name")]
    public async Task ValidState_CanExecute()
    {
      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList,
      }.MockVM();

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

      await viewmodel.EditListCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task EditList_Cancel_NoChanges()
    {
      var searchQuery = _savedList.SearchQuery;
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

      await viewmodel.EditListCommand.ExecuteAsync(null);

      Assert.AreEqual(searchQuery, viewmodel.Query);
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

      await viewmodel.EditListCommand.ExecuteAsync(null);

      Assert.AreEqual(name, viewmodel.Name);
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

      await viewmodel.EditListCommand.ExecuteAsync(null);

      Assert.AreEqual(query, viewmodel.Query);
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

      Assert.AreEqual(0, viewmodel.QueryCardsViewModel.TotalCardCount);

      _dependencies.Importer.ExpectedCards = [new(MTGCardInfoMocker.MockInfo(name: "Card"))];

      await viewmodel.EditListCommand.ExecuteAsync(null);

      Assert.AreEqual(1, viewmodel.QueryCardsViewModel.TotalCardCount);
    }

    [TestMethod]
    public async Task EditList_Success_HasUnsavedChanges()
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

      await viewmodel.EditListCommand.ExecuteAsync(null);

      Assert.IsTrue(viewmodel.HasUnsavedChanges);
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

      await viewmodel.EditListCommand.ExecuteAsync(null);

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

      await viewmodel.EditListCommand.ExecuteAsync(null);

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task EditList_Exists_ErrorNotificationShown()
    {
      var notifier = new TestNotifier();
      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList,
        Confirmers = new()
        {
          EditCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(("Name", "New Query"))
          }
        },
        Notifier = notifier,
        ExistsValidator = (name) => true
      }.MockVM();

      await viewmodel.EditListCommand.ExecuteAsync(null);

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

      await viewmodel.EditListCommand.ExecuteAsync(null);

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

      await viewmodel.EditListCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task EditList_Conflict_Cancel_NoChanges()
    {
      var oldName = _savedList.Name;
      var oldQuery = _savedList.SearchQuery;
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

      await viewmodel.EditListCommand.ExecuteAsync(null);

      Assert.AreEqual(oldName, viewmodel.Name);
      Assert.AreEqual(oldQuery, viewmodel.Query);
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

      await viewmodel.EditListCommand.ExecuteAsync(null);

      Assert.AreEqual(name, viewmodel.Name);
      Assert.AreEqual(query, viewmodel.Query);
    }

    [TestMethod]
    public async Task EditList_Conflict_Accept_OwnedCardsChanged()
    {
      var expectedCards = new CardImportResult.Card[]
      {
        new(_savedList.Cards[0].Info),
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
        viewmodel.OwnedCards.Select(x => x.Info.ScryfallId).ToArray());

      await viewmodel.EditListCommand.ExecuteAsync(null);

      CollectionAssert.AreEquivalent(
        new Guid[] { expectedCards.First().Info.ScryfallId },
        viewmodel.OwnedCards.Select(x => x.Info.ScryfallId).ToArray());
    }
  }
}

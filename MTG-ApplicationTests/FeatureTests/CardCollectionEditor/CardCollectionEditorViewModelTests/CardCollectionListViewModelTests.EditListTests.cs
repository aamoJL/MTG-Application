using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;
using static MTGApplicationTests.FeatureTests.CardCollectionEditor.CardCollectionEditorViewModelTests.CardCollectionEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionEditorViewModelTests
{
  [TestClass]
  public class EditListTests : CardCollectionEditorViewModelTestsBase
  {
    public void InvalidState_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM();

      Assert.IsFalse(viewmodel.EditListCommand.CanExecute(null));
    }

    public void ValidState_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM(_savedCollection);

      Assert.IsTrue(viewmodel.EditListCommand.CanExecute(viewmodel.SelectedCardCollectionListViewModel.CollectionList));
    }

    [TestMethod]
    public async Task EditList_EditConfirmationShown()
    {
      var confirmer = new TestConfirmer<(string, string)?, (string, string)>();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
          {
            EditCollectionListConfirmer = confirmer,
          }
        }
      }.MockVM(_savedCollection);

      var list = viewmodel.SelectedCardCollectionListViewModel.CollectionList;

      await viewmodel.EditListCommand.ExecuteAsync(list);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task EditList_Cancel_NoChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
          {
            EditCollectionListConfirmer = new()
            {
              OnConfirm = async msg => await Task.FromResult<(string, string)?>(null)
            }
          }
        }
      }.MockVM(_savedCollection);

      var list = viewmodel.SelectedCardCollectionListViewModel.CollectionList;
      var query = list.SearchQuery;

      await viewmodel.EditListCommand.ExecuteAsync(list);

      Assert.AreEqual(query, list.SearchQuery);
    }

    [TestMethod]
    public async Task EditList_Success_NameChanged()
    {
      var name = "New Name";
      var query = "New Query";
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
          {
            EditCollectionListConfirmer = new()
            {
              OnConfirm = async msg => await Task.FromResult((name, query))
            }
          }
        }
      }.MockVM(_savedCollection);

      var list = viewmodel.SelectedCardCollectionListViewModel.CollectionList;

      await viewmodel.EditListCommand.ExecuteAsync(list);

      Assert.AreEqual(name, list.Name);
    }

    [TestMethod]
    public async Task EditList_Success_QueryChanged()
    {
      var name = "New Name";
      var query = "New Query";
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
          {
            EditCollectionListConfirmer = new()
            {
              OnConfirm = async msg => await Task.FromResult((name, query))
            }
          }
        }
      }.MockVM(_savedCollection);

      var list = viewmodel.SelectedCardCollectionListViewModel.CollectionList;

      await viewmodel.EditListCommand.ExecuteAsync(list);

      Assert.AreEqual(query, list.SearchQuery);
    }

    [TestMethod]
    public async Task EditList_Success_QueryCardsUpdated()
    {
      var name = "New Name";
      var query = "New Query";
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
          {
            EditCollectionListConfirmer = new()
            {
              OnConfirm = async msg => await Task.FromResult((name, query))
            }
          }
        }
      }.MockVM(_savedCollection);

      var list = viewmodel.SelectedCardCollectionListViewModel.CollectionList;

      Assert.AreEqual(0, viewmodel.SelectedCardCollectionListViewModel.TotalCount);

      _dependencies.Importer.ExpectedCards = [new(MTGCardInfoMocker.MockInfo(name: "Card"))];

      await viewmodel.EditListCommand.ExecuteAsync(list);

      await viewmodel.SelectedCardCollectionListViewModel.WaitForCardUpdate();

      Assert.AreEqual(1, viewmodel.SelectedCardCollectionListViewModel.TotalCount);
    }

    [TestMethod]
    public async Task EditList_Success_HasUnsavedChanges()
    {
      var name = "New Name";
      var query = "New Query";
      var viewmodel = new Mocker(_dependencies)
      {

        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
          {
            EditCollectionListConfirmer = new()
            {
              OnConfirm = async msg => await Task.FromResult((name, query))
            }
          }
        }
      }.MockVM(_savedCollection);

      var list = viewmodel.SelectedCardCollectionListViewModel.CollectionList;

      await viewmodel.EditListCommand.ExecuteAsync(list);

      Assert.IsTrue(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task EditList_NoName_ErrorNotificationShown()
    {
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
          {
            EditCollectionListConfirmer = new()
            {
              OnConfirm = async msg => await Task.FromResult((string.Empty, "New Query"))
            }
          }
        },
        Notifier = notifier
      }.MockVM(_savedCollection);

      var list = viewmodel.SelectedCardCollectionListViewModel.CollectionList;

      await viewmodel.EditListCommand.ExecuteAsync(list);

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task EditList_NoQuery_ErrorNotificationShown()
    {
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
          {
            EditCollectionListConfirmer = new()
            {
              OnConfirm = async msg => await Task.FromResult(("New Name", string.Empty))
            }
          }
        },
        Notifier = notifier
      }.MockVM(_savedCollection);

      var list = viewmodel.SelectedCardCollectionListViewModel.CollectionList;

      await viewmodel.EditListCommand.ExecuteAsync(list);

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task EditList_Exists_ErrorNotificationShown()
    {
      var name = _savedCollection.CollectionLists[1].Name;

      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {

        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
          {
            EditCollectionListConfirmer = new()
            {
              OnConfirm = async msg => await Task.FromResult((name, "New Query"))
            }
          }
        },
        Notifier = notifier,
      }.MockVM(_savedCollection);

      var list = viewmodel.SelectedCardCollectionListViewModel.CollectionList;

      await viewmodel.EditListCommand.ExecuteAsync(list);

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task EditList_SameName_NoErrorNotificationShown()
    {
      var list = _savedCollection.CollectionLists.First();

      var name = list.Name;
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {

        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
          {
            EditCollectionListConfirmer = new()
            {
              OnConfirm = async msg => await Task.FromResult((name, "New Query"))
            }
          }
        },
        Notifier = notifier
      }.MockVM(_savedCollection);

      await viewmodel.EditListCommand.ExecuteAsync(list);

      NotificationAssert.NotificationNotSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task EditList_Success_SuccessNotificationShown()
    {
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {

        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
          {
            EditCollectionListConfirmer = new()
            {
              OnConfirm = async msg => await Task.FromResult(("New Name", "New Query"))
            }
          }
        },
        Notifier = notifier
      }.MockVM(_savedCollection);

      var list = viewmodel.SelectedCardCollectionListViewModel.CollectionList;

      await viewmodel.EditListCommand.ExecuteAsync(list);

      NotificationAssert.NotificationSent(NotificationType.Success, notifier);
    }

    [TestMethod]
    public async Task EditList_Conflict_ConflictConfirmationShown()
    {
      var confirmer = new TestConfirmer<ConfirmationResult>();
      var viewmodel = new Mocker(_dependencies)
      {

        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
          {
            EditCollectionListConfirmer = new()
            {
              OnConfirm = async msg => await Task.FromResult(("New Name", "New Query"))
            },
            EditCollectionListQueryConflictConfirmer = confirmer,

          }
        }
      }.MockVM(_savedCollection);

      var list = viewmodel.SelectedCardCollectionListViewModel.CollectionList;

      await viewmodel.EditListCommand.ExecuteAsync(list);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task EditList_Conflict_Cancel_NoChanges()
    {
      var list = _savedCollection.CollectionLists.First();

      var oldName = list.Name;
      var oldQuery = list.SearchQuery;
      var viewmodel = new Mocker(_dependencies)
      {

        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
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
        }
      }.MockVM(_savedCollection);

      await viewmodel.EditListCommand.ExecuteAsync(list);

      Assert.AreEqual(oldName, list.Name);
      Assert.AreEqual(oldQuery, list.SearchQuery);
    }

    [TestMethod]
    public async Task EditList_Conflict_Accept_QueryAndNameChanged()
    {
      var name = "New Name";
      var query = "New Query";
      var viewmodel = new Mocker(_dependencies)
      {

        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
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
        }
      }.MockVM(_savedCollection);

      var list = viewmodel.SelectedCardCollectionListViewModel.CollectionList;

      await viewmodel.EditListCommand.ExecuteAsync(list);

      Assert.AreEqual(name, list.Name);
      Assert.AreEqual(query, list.SearchQuery);
    }

    [TestMethod]
    public async Task EditList_Conflict_Accept_OwnedCardsChanged()
    {
      var list = _savedCollection.CollectionLists.First();

      var expectedCards = new CardImportResult.Card[]
      {
        new(list.Cards[0].Info),
        new(MTGCardInfoMocker.MockInfo())
      };

      _dependencies.Importer.ExpectedCards = expectedCards;

      var viewmodel = new Mocker(_dependencies)
      {

        Confirmers = new()
        {
          CardCollectionListConfirmers = new()
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
        }
      }.MockVM(_savedCollection);

      CollectionAssert.AreNotEquivalent(
        new Guid[] { expectedCards.First().Info.ScryfallId },
        viewmodel.SelectedCardCollectionListViewModel.CollectionList.Cards.Select(x => x.Info.ScryfallId).ToArray());

      await viewmodel.EditListCommand.ExecuteAsync(list);

      CollectionAssert.AreEquivalent(
        new Guid[] { expectedCards.First().Info.ScryfallId },
        viewmodel.SelectedCardCollectionListViewModel.CollectionList.Cards.Select(x => x.Info.ScryfallId).ToArray());
    }
  }
}

using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Editor.ViewModels;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Editor.UseCases;

[TestClass]
public class DeleteList : CardCollectionEditorViewModelTestBase
{
  [TestMethod]
  public async Task DeleteList_DeleteConfirmationShown()
  {
    var confirmer = new TestConfirmer<ConfirmationResult>();
    var viewmodel = await new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          DeleteCollectionListConfirmer = confirmer
        },
      }
    }.MockVM(_savedCollection);

    await viewmodel.DeleteSelectedListCommand.ExecuteAsync(_savedCollection.CollectionLists.First());

    ConfirmationAssert.ConfirmationShown(confirmer);
  }

  [TestMethod]
  public async Task DeleteList_Cancel_NoChanges()
  {
    var list = _savedCollection.CollectionLists.First();
    var viewmodel = await new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel)
          }
        }
      }
    }.MockVM(_savedCollection);

    await viewmodel.DeleteSelectedListCommand.ExecuteAsync(list);

    Assert.Contains(list, viewmodel.Collection.CollectionLists);
  }

  [TestMethod]
  public async Task DeleteList_Accept_ListDeleted()
  {
    var list = _savedCollection.CollectionLists.First();
    var viewmodel = await new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        }
      }
    }.MockVM(_savedCollection);

    await viewmodel.DeleteSelectedListCommand.ExecuteAsync(list);

    Assert.DoesNotContain(list, viewmodel.Collection.CollectionLists);
  }

  [TestMethod]
  public async Task DeleteList_Success_HasUnsavedChanges()
  {
    var list = _savedCollection.CollectionLists.First();
    var viewmodel = await new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        }
      }
    }.MockVM(_savedCollection);

    await viewmodel.DeleteSelectedListCommand.ExecuteAsync(list);

    Assert.IsTrue(viewmodel.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task DeleteList_Success_SuccessNotificationSent()
  {
    var list = _savedCollection.CollectionLists.First();
    var notifier = new TestNotifier();
    var viewmodel = await new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        }
      },
      Notifier = notifier
    }.MockVM(_savedCollection);

    await viewmodel.DeleteSelectedListCommand.ExecuteAsync(list);

    NotificationAssert.NotificationSent(NotificationType.Success, notifier);
  }

  [TestMethod]
  public async Task DeleteList_Error_ErrorNotificationSent()
  {
    var list = _savedCollection.CollectionLists.First();
    var notifier = new TestNotifier();
    var viewmodel = await new Mocker(_dependencies)
    {
      Confirmers = new()
      {
        CardCollectionConfirmers = new()
        {
          DeleteCollectionListConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes)
          }
        }
      },
      Notifier = notifier
    }.MockVM(_savedCollection);

    // Remove list manually, so the command will fail to remove it
    viewmodel.Collection.CollectionLists.Remove(list);

    await viewmodel.DeleteSelectedListCommand.ExecuteAsync(list);

    NotificationAssert.NotificationSent(NotificationType.Error, notifier);
  }
}
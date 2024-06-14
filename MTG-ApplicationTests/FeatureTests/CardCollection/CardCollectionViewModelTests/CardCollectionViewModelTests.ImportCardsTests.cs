using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class ImportCardsTests : CardCollectionViewModelTestsBase, ICanExecuteCommandTests
  {
    [TestMethod("Should not be able to execute if a list is not selected")]
    public void InvalidState_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies) { Collection = new() }.MockVM();

      Assert.IsFalse(viewmodel.ImportCardsCommand.CanExecute(null));
    }

    [TestMethod("Should be able to execute if a list is selected")]
    public void ValidState_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies) { Collection = _savedCollection }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      Assert.IsTrue(viewmodel.ImportCardsCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task ImportCards_ImportConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection,
        Confirmers = new()
        {
          ImportCardsConfirmer = new TestExceptionConfirmer<string>(),
        }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ImportCardsCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task ImportCards_Cancel_NoChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>(null),
          }
        }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(0, viewmodel.SelectedList.Cards.Count);
    }

    [TestMethod]
    public async Task ImportCards_Empty_NoChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>(string.Empty),
          }
        }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(0, viewmodel.SelectedList.Cards.Count);
    }

    [TestMethod]
    public async Task ImportCards_NotFound_NoChanges()
    {
      _dependencies.CardAPI.ExpectedCards = [];
      _dependencies.CardAPI.NotFoundCount = 1;

      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(0, viewmodel.SelectedList.Cards.Count);
    }

    [TestMethod]
    public async Task ImportCards_Found_CardsAdded()
    {
      _dependencies.CardAPI.ExpectedCards = [new CardImportResult.Card(MTGCardInfoMocker.MockInfo())];

      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(1, viewmodel.SelectedList.Cards.Count);
    }

    [TestMethod]
    public async Task ImportCards_Exists_CardNotAdded()
    {
      var card = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

      _dependencies.CardAPI.ExpectedCards = [card];

      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new() { Cards = [new(card.Info)] }] },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(1, viewmodel.SelectedList.Cards.Count);
    }

    [TestMethod]
    public async Task ImportCards_AllFound_SuccessNotificationSent()
    {
      _dependencies.CardAPI.ExpectedCards = [new CardImportResult.Card(MTGCardInfoMocker.MockInfo())];

      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => viewmodel.ImportCardsCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task ImportCards_PartialFound_WarningNotificationSent()
    {
      _dependencies.CardAPI.ExpectedCards = [new CardImportResult.Card(MTGCardInfoMocker.MockInfo())];
      _dependencies.CardAPI.NotFoundCount = 1;

      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await NotificationAssert.NotificationSent(NotificationType.Warning,
        () => viewmodel.ImportCardsCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task ImportCards_AllSkipped_SuccessNotificationSent()
    {
      var card = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());
      _dependencies.CardAPI.ExpectedCards = [card];

      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new() { Cards = [new(card.Info)] }] },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => viewmodel.ImportCardsCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task ImportCards_SomeSkipped_SuccessNotificationSent()
    {
      var existingCard = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

      _dependencies.CardAPI.ExpectedCards = [
        existingCard,
        new CardImportResult.Card(MTGCardInfoMocker.MockInfo())
      ];

      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new() { Cards = [new(existingCard.Info)] }] },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => viewmodel.ImportCardsCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task ImportCards_NotFound_ErrorNotificationSent()
    {
      _dependencies.CardAPI.ExpectedCards = [];
      _dependencies.CardAPI.NotFoundCount = 1;

      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new()] },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      await NotificationAssert.NotificationSent(NotificationType.Error,
        () => viewmodel.ImportCardsCommand.ExecuteAsync(null));
    }
  }
}

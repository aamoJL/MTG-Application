using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using Windows.ApplicationModel.Search;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionListViewModelTests
{
  [TestClass]
  public class ImportCardsTests : CardCollectionListViewModelTestsBase, ICanExecuteCommandAsyncTests
  {
    [TestMethod("Should not be able to execute if the list has no name")]
    public async Task InvalidState_CanNotExecute()
    {
      var viewmodel = await new Mocker(_dependencies)
      {
        Model = new()
      }.MockVM();

      Assert.IsFalse(viewmodel.ImportCardsCommand.CanExecute(null));
    }

    [TestMethod("Should be able to execute if the list has a name")]
    public async Task ValidState_CanExecute()
    {
      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList
      }.MockVM();

      Assert.IsTrue(viewmodel.ImportCardsCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task ImportCards_ImportConfirmationShown()
    {
      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList,
        Confirmers = new()
        {
          ImportCardsConfirmer = new TestExceptionConfirmer<string>(),
        }
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.ImportCardsCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task ImportCards_Cancel_NoChanges()
    {
      var viewmodel = await new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>(null),
          }
        }
      }.MockVM();

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(0, viewmodel.OwnedCards.Count);
    }

    [TestMethod]
    public async Task ImportCards_Empty_NoChanges()
    {
      var viewmodel = await new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>(string.Empty),
          }
        }
      }.MockVM();

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(0, viewmodel.OwnedCards.Count);
    }

    [TestMethod]
    public async Task ImportCards_NotFound_NoChanges()
    {
      _dependencies.Importer.ExpectedCards = [];
      _dependencies.Importer.NotFoundCount = 1;

      var viewmodel = await new Mocker(_dependencies)
      {
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        }
      }.MockVM();

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(0, viewmodel.OwnedCards.Count);
    }

    [TestMethod]
    public async Task ImportCards_Found_CardsAdded()
    {
      _dependencies.Importer.ExpectedCards = [new CardImportResult.Card(MTGCardInfoMocker.MockInfo())];

      var viewmodel = await new Mocker(_dependencies)
      {
        Model = new() { Name = "Asd", SearchQuery = "asd", Cards = [] },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        }
      }.MockVM();

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(1, viewmodel.OwnedCards.Count);
    }

    [TestMethod]
    public async Task ImportCards_Exists_CardNotAdded()
    {
      var card = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

      _dependencies.Importer.ExpectedCards = [card];

      var viewmodel = await new Mocker(_dependencies)
      {
        Model = new()
        {
          Cards = [new(card.Info)]
        },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        }
      }.MockVM();

      await viewmodel.ImportCardsCommand.ExecuteAsync(null);

      Assert.AreEqual(1, viewmodel.OwnedCards.Count);
    }

    [TestMethod]
    public async Task ImportCards_AllFound_SuccessNotificationSent()
    {
      _dependencies.Importer.ExpectedCards = [new CardImportResult.Card(MTGCardInfoMocker.MockInfo())];

      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList,
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => viewmodel.ImportCardsCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task ImportCards_PartialFound_WarningNotificationSent()
    {
      _dependencies.Importer.ExpectedCards = [new CardImportResult.Card(MTGCardInfoMocker.MockInfo())];
      _dependencies.Importer.NotFoundCount = 1;

      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList,
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();

      await NotificationAssert.NotificationSent(NotificationType.Warning,
        () => viewmodel.ImportCardsCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task ImportCards_AllSkipped_SuccessNotificationSent()
    {
      var card = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

      _dependencies.Importer.ExpectedCards = [card];

      var viewmodel = await new Mocker(_dependencies)
      {
        Model = new() { Name = "asd", Cards = [new(card.Info)] },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => viewmodel.ImportCardsCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task ImportCards_SomeSkipped_SuccessNotificationSent()
    {
      var existingCard = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

      _dependencies.Importer.ExpectedCards = [
        existingCard,
        new CardImportResult.Card(MTGCardInfoMocker.MockInfo())
      ];

      var viewmodel = await new Mocker(_dependencies)
      {
        Model = new() { Name = "asd", Cards = [new(existingCard.Info)] },
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => viewmodel.ImportCardsCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task ImportCards_NotFound_ErrorNotificationSent()
    {
      _dependencies.Importer.ExpectedCards = [];
      _dependencies.Importer.NotFoundCount = 1;

      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList,
        Confirmers = new()
        {
          ImportCardsConfirmer = new()
          {
            OnConfirm = async msg => await Task.FromResult<string?>("Import"),
          }
        },
        Notifier = new() { OnNotify = msg => throw new NotificationException(msg) }
      }.MockVM();

      await NotificationAssert.NotificationSent(NotificationType.Error,
        () => viewmodel.ImportCardsCommand.ExecuteAsync(null));
    }
  }
}

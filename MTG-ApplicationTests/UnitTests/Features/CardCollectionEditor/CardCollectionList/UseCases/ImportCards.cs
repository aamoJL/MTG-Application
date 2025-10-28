using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using MTGApplicationTests.UnitTests.Features.CardCollectionEditor.CardCollectionList.ViewModels;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.CardCollectionList.UseCases;

[TestClass]
public class ImportCards : CardCollectionListViewModelTestBase, ICanExecuteCommandAsyncTests
{
  [TestMethod(DisplayName = "Should not be able to execute if the list has no name")]
  public async Task InvalidState_CanNotExecute()
  {
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = new()
    }.MockVM();

    Assert.IsFalse(viewmodel.ImportCardsCommand.CanExecute(null));
  }

  [TestMethod(DisplayName = "Should be able to execute if the list has a name")]
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
    var confirmer = new TestConfirmer<string>();
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        ImportCardsConfirmer = confirmer,
      }
    }.MockVM();

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    ConfirmationAssert.ConfirmationShown(confirmer);
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
          OnConfirm = async msg => await Task.FromResult<string>(null),
        }
      }
    }.MockVM();

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    Assert.IsEmpty(viewmodel.CollectionList.Cards);
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
          OnConfirm = async msg => await Task.FromResult(string.Empty),
        }
      }
    }.MockVM();

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    Assert.IsEmpty(viewmodel.CollectionList.Cards);
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
          OnConfirm = async msg => await Task.FromResult("Import"),
        }
      }
    }.MockVM();

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    Assert.IsEmpty(viewmodel.CollectionList.Cards);
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
          OnConfirm = async msg => await Task.FromResult("Import"),
        }
      }
    }.MockVM();

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(1, viewmodel.CollectionList.Cards);
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
          OnConfirm = async msg => await Task.FromResult("Import"),
        }
      }
    }.MockVM();

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(1, viewmodel.CollectionList.Cards);
  }

  [TestMethod]
  public async Task ImportCards_AllFound_SuccessNotificationSent()
  {
    _dependencies.Importer.ExpectedCards = [new CardImportResult.Card(MTGCardInfoMocker.MockInfo())];

    var notifier = new TestNotifier();
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        ImportCardsConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult("Import"),
        }
      },
      Notifier = notifier
    }.MockVM();

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Success, notifier);
  }

  [TestMethod]
  public async Task ImportCards_PartialFound_WarningNotificationSent()
  {
    _dependencies.Importer.ExpectedCards = [new CardImportResult.Card(MTGCardInfoMocker.MockInfo())];
    _dependencies.Importer.NotFoundCount = 1;

    var notifier = new TestNotifier();
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        ImportCardsConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult("Import"),
        }
      },
      Notifier = notifier
    }.MockVM();

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Warning, notifier);
  }

  [TestMethod]
  public async Task ImportCards_AllSkipped_SuccessNotificationSent()
  {
    var card = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

    _dependencies.Importer.ExpectedCards = [card];

    var notifier = new TestNotifier();
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = new() { Name = "asd", Cards = [new(card.Info)] },
      Confirmers = new()
      {
        ImportCardsConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult("Import"),
        }
      },
      Notifier = notifier
    }.MockVM();

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Success, notifier);
  }

  [TestMethod]
  public async Task ImportCards_SomeSkipped_SuccessNotificationSent()
  {
    var existingCard = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

    _dependencies.Importer.ExpectedCards = [
      existingCard,
        new CardImportResult.Card(MTGCardInfoMocker.MockInfo())
    ];

    var notifier = new TestNotifier();
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = new() { Name = "asd", Cards = [new(existingCard.Info)] },
      Confirmers = new()
      {
        ImportCardsConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult("Import"),
        }
      },
      Notifier = notifier
    }.MockVM();

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Success, notifier);
  }

  [TestMethod]
  public async Task ImportCards_NotFound_ErrorNotificationSent()
  {
    _dependencies.Importer.ExpectedCards = [];
    _dependencies.Importer.NotFoundCount = 1;

    var notifier = new TestNotifier();
    var viewmodel = await new Mocker(_dependencies)
    {
      Model = _savedList,
      Confirmers = new()
      {
        ImportCardsConfirmer = new()
        {
          OnConfirm = async msg => await Task.FromResult("Import"),
        }
      },
      Notifier = notifier
    }.MockVM();

    await viewmodel.ImportCardsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Error, notifier);
  }
}

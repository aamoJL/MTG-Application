using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.EditorPage;

[TestClass]
public class OpenDeck
{
  [TestMethod]
  public async Task Open_IsDirty_CancelSave_Cancelled()
  {
    var factory = new TestDeckEditorPageViewModelFactory()
    {
      Notifier = new(),
      Confirmers = new()
      {
        DeckConfirmers = new()
        {
          ConfirmUnsavedChanges = _ => Task.FromResult(ConfirmationResult.Cancel)
        }
      }
    };
    var vm = factory.Build();

    vm.DeckViewModel.SaveStatus.HasUnsavedChanges = true;

    await vm.OpenDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.DeckName);
  }

  [TestMethod]
  public async Task Open_Cancel_Cancelled()
  {
    var factory = new TestDeckEditorPageViewModelFactory()
    {
      Notifier = new(),
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>([new("Deck")])
      },
      Confirmers = new()
      {
        ConfirmDeckOpen = _ => Task.FromResult<string?>(null)
      }
    };
    var vm = factory.Build();

    await vm.OpenDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.DeckName);
  }

  [TestMethod]
  public async Task Open_DeckChanged()
  {
    var factory = new TestDeckEditorPageViewModelFactory()
    {
      Notifier = new(),
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>([new("Deck")]),
        GetResult = _ => Task.FromResult<MTGCardDeckDTO?>(new("Deck")),
      },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success()
      },
      Confirmers = new()
      {
        ConfirmDeckOpen = _ => Task.FromResult<string?>("Deck")
      }
    };
    var vm = factory.Build();

    await vm.OpenDeckCommand.ExecuteAsync(null);

    Assert.AreEqual("Deck", vm.DeckName);
  }

  [TestMethod]
  public async Task Open_DeckNamesInConfirmation_OrderedByName()
  {
    var names = Array.Empty<string>();
    var factory = new TestDeckEditorPageViewModelFactory()
    {
      Notifier = new(),
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>([new("Deck 3"), new("Deck 1"), new("Deck 2")]),
        GetResult = _ => Task.FromResult<MTGCardDeckDTO?>(new("Deck")),
      },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success()
      },
      Confirmers = new()
      {
        ConfirmDeckOpen = async data =>
        {
          names = [.. data.Data];
          return await Task.FromResult<string?>("Deck");
        }
      }
    };
    var vm = factory.Build();

    await vm.OpenDeckCommand.ExecuteAsync(null);

    var expected = new string[] { "Deck 1", "Deck 2", "Deck 3" };

    CollectionAssert.AreEqual(expected, names);
  }

  [TestMethod]
  public async Task Open_WithName_DeckChanged()
  {
    var factory = new TestDeckEditorPageViewModelFactory()
    {
      Notifier = new(),
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>([new("Deck")]),
        GetResult = _ => Task.FromResult<MTGCardDeckDTO?>(new("Deck")),
      },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success()
      },
    };
    var vm = factory.Build();

    await vm.OpenDeckCommand.ExecuteAsync("Deck");

    Assert.AreEqual("Deck", vm.DeckName);
  }

  [TestMethod]
  public async Task Open_Failure_NotificationShown()
  {
    var factory = new TestDeckEditorPageViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>([new("Deck")]),
        GetResult = _ => Task.FromResult<MTGCardDeckDTO?>(null),
      },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success()
      },
      Confirmers = new()
      {
        ConfirmDeckOpen = _ => Task.FromResult<string?>("Deck")
      }
    };
    var vm = factory.Build();

    await vm.OpenDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.DeckName);
    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Open_Exception_NotificationShown()
  {
    var factory = new TestDeckEditorPageViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>([new("Deck")]),
        GetResult = _ => Task.FromResult<MTGCardDeckDTO?>(new("Deck")),
      },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success()
      },
      Confirmers = new()
      {
        ConfirmDeckOpen = _ => throw new()
      }
    };
    var vm = factory.Build();

    await vm.OpenDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.DeckName);
    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}

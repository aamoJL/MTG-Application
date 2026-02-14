using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.EditorPage;

[TestClass]
public class OpenCollection
{
  [TestMethod]
  public async Task Open_IsDirty_CancelSave_Return()
  {
    var factory = new TestEditorPageViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([])
      },
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardCollectionDTO>>([]),
        GetResult = _ => Task.FromResult<MTGCardCollectionDTO>(new("Name", []))
      },
      EditorPageConfirmers = new()
      {
        ConfirmCollectionOpen = _ => Task.FromResult("Name"),
        CollectionConfirmers = new()
        {
          ConfirmUnsavedChanges = _ => Task.FromResult(ConfirmationResult.Cancel)
        }
      }
    };
    var vm = factory.Build();

    vm.CollectionViewModel.SaveStatus.HasUnsavedChanges = true;

    await vm.OpenCollectionCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.CollectionName);
  }

  [TestMethod]
  public async Task Open_IsDirty_DeclineSave_CollectionChanged()
  {
    var factory = new TestEditorPageViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([])
      },
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardCollectionDTO>>([]),
        GetResult = _ => Task.FromResult<MTGCardCollectionDTO>(new("Name", []))
      },
      EditorPageConfirmers = new()
      {
        ConfirmCollectionOpen = _ => Task.FromResult("Name"),
        CollectionConfirmers = new()
        {
          ConfirmUnsavedChanges = _ => Task.FromResult(ConfirmationResult.No)
        }
      }
    };
    var vm = factory.Build();

    vm.CollectionViewModel.SaveStatus.HasUnsavedChanges = true;

    await vm.OpenCollectionCommand.ExecuteAsync(null);

    Assert.AreEqual("Name", vm.CollectionName);
  }

  [TestMethod]
  public async Task Open_InvalidName_Return()
  {
    var factory = new TestEditorPageViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([])
      },
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardCollectionDTO>>([]),
        GetResult = _ => Task.FromResult<MTGCardCollectionDTO>(new("Name", []))
      },
      EditorPageConfirmers = new()
      {
        ConfirmCollectionOpen = _ => Task.FromResult<string>(null),
      }
    };
    var vm = factory.Build();

    await vm.OpenCollectionCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.CollectionName);
  }

  [TestMethod]
  public async Task Open_EmptyName_Return()
  {
    var factory = new TestEditorPageViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([])
      },
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardCollectionDTO>>([]),
        GetResult = _ => Task.FromResult<MTGCardCollectionDTO>(new("Name", []))
      },
      EditorPageConfirmers = new()
      {
        ConfirmCollectionOpen = _ => Task.FromResult(string.Empty),
      }
    };
    var vm = factory.Build();

    await vm.OpenCollectionCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.CollectionName);
  }

  [TestMethod]
  public async Task Open_Success_CollectionChanged()
  {
    var factory = new TestEditorPageViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([])
      },
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardCollectionDTO>>([]),
        GetResult = _ => Task.FromResult<MTGCardCollectionDTO>(new("Name", []))
      },
      EditorPageConfirmers = new()
      {
        ConfirmCollectionOpen = _ => Task.FromResult("Name"),
      }
    };
    var vm = factory.Build();

    await vm.OpenCollectionCommand.ExecuteAsync(null);

    Assert.AreEqual("Name", vm.CollectionName);
  }

  [TestMethod]
  public async Task Open_Success_NotificationShown()
  {
    var factory = new TestEditorPageViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([])
      },
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardCollectionDTO>>([]),
        GetResult = _ => Task.FromResult<MTGCardCollectionDTO>(new("Name", []))
      },
      EditorPageConfirmers = new()
      {
        ConfirmCollectionOpen = _ => Task.FromResult("Name"),
      }
    };
    var vm = factory.Build();

    await vm.OpenCollectionCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Success, factory.Notifier);
  }

  [TestMethod]
  public async Task Open_Failure_NotificationShown()
  {
    var factory = new TestEditorPageViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([])
      },
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardCollectionDTO>>([]),
        GetResult = _ => Task.FromResult<MTGCardCollectionDTO>(null)
      },
      EditorPageConfirmers = new()
      {
        ConfirmCollectionOpen = _ => Task.FromResult("Name"),
      }
    };
    var vm = factory.Build();

    await vm.OpenCollectionCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Open_Exception_NotificationShown()
  {
    var factory = new TestEditorPageViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([])
      },
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardCollectionDTO>>([]),
        GetResult = _ => Task.FromResult<MTGCardCollectionDTO>(new("Name", []))
      },
      EditorPageConfirmers = new()
      {
        ConfirmCollectionOpen = _ => throw new(),
      }
    };
    var vm = factory.Build();

    await vm.OpenCollectionCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Error, factory.Notifier);
  }
}
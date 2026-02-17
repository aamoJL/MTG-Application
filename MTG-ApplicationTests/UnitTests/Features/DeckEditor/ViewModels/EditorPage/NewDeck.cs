using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplicationTests.TestUtility.Importers;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.EditorPage;

[TestClass]
public class NewDeck
{
  [TestMethod]
  public async Task New_IsNotDirty_DeckChanged()
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

    await vm.NewDeckCommand.ExecuteAsync(null);
    Assert.AreEqual(string.Empty, vm.DeckName);
  }

  [TestMethod]
  public async Task New_IsDirty_DeclineSave_DeckChanged()
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
        DeckConfirmers = new()
        {
          ConfirmUnsavedChanges = _ => Task.FromResult(ConfirmationResult.No)
        }
      }
    };
    var vm = factory.Build();

    await vm.OpenDeckCommand.ExecuteAsync("Deck");
    Assert.AreEqual("Deck", vm.DeckName);

    vm.DeckViewModel.SaveStatus.HasUnsavedChanges = true;

    await vm.NewDeckCommand.ExecuteAsync(null);
    Assert.AreEqual(string.Empty, vm.DeckName);
  }

  [TestMethod]
  public async Task New_IsDirty_CancelSave_Cancelled()
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
        DeckConfirmers = new()
        {
          ConfirmUnsavedChanges = _ => Task.FromResult(ConfirmationResult.Cancel)
        }
      }
    };
    var vm = factory.Build();

    await vm.OpenDeckCommand.ExecuteAsync("Deck");
    Assert.AreEqual("Deck", vm.DeckName);

    vm.DeckViewModel.SaveStatus.HasUnsavedChanges = true;

    await vm.NewDeckCommand.ExecuteAsync(null);

    Assert.AreEqual("Deck", vm.DeckName);
  }
}
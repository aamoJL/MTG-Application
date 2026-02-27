using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplicationTests.TestUtility.Importers;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.EditorPage;

[TestClass]
public class OnDeckDeleted
{
  [TestMethod]
  public async Task DeckDeleted_DeckChanged()
  {
    var factory = new TestDeckEditorPageViewModelFactory()
    {
      Notifier = new(),
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>([new("Deck")]),
        GetResult = _ => Task.FromResult<MTGCardDeckDTO?>(new("Deck")),
        DeleteResult = _ => Task.FromResult(true)
      },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success()
      },
      DeckConfirmers = new()
      {
        ConfirmDeckDelete = _ => Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    await vm.OpenDeckCommand.ExecuteAsync("Deck");
    Assert.AreEqual("Deck", vm.DeckName);

    await vm.DeckViewModel.DeleteDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.DeckName);
  }
}
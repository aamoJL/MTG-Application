using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckSelection.ViewModels.SelectionPage;

[TestClass]
public class RefreshDecks
{
  [TestMethod]
  public async Task Refresh_DecksAdded()
  {
    var factory = new TestDeckSelectionPageViewModelFactory()
    {
      Repository = new()
      {
        GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>([
          new("Deck 3"),
          new("Deck 1"),
          new("Deck 2"),
        ])
      },
      Importer = new() { Result = TestMTGCardImporter.Success([]) }
    };
    var vm = factory.Build();

    await vm.RefreshDecksCommand.ExecuteAsync(null);

    CollectionAssert.AreEqual(
      expected: new DeckSelectionDeck[] {
        new(){ Name = "Deck 1" },
        new(){ Name = "Deck 2" },
        new(){ Name = "Deck 3" },
      },
      actual: vm.DeckItems);
  }

  [TestMethod]
  public async Task Refresh_Exception_NotificationShown()
  {
    var factory = new TestDeckSelectionPageViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      Repository = new()
      {
        GetAllResult = () => throw new()
      }
    };
    var vm = factory.Build();

    await vm.RefreshDecksCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}
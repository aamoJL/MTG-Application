using MTGApplication.Features.DeckSelection.ViewModels;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.ViewModel;

namespace MTGApplicationTests.UnitTests.Features.DeckSelection.UseCases;

[TestClass]
public class GetDeckSelectionDecks
{
  private readonly DeckRepositoryDependencies _dependensies = new();

  [TestMethod(DisplayName = "Deck items should be populated when decks has been loaded")]
  public async Task LoadDecks_DeckItemsPopulated()
  {
    var itemCount = 3;
    _dependensies.ContextFactory.Populate([.. MTGCardDeckDTOMocker.MockList(itemCount)]);
    var vm = new DeckSelectionPageViewModel(_dependensies.Repository, _dependensies.Importer);

    await vm.RefreshDecksCommand.ExecuteAsync(null);

    Assert.HasCount(itemCount, vm.DeckItems, "Item counts does not match");
  }

  [TestMethod(DisplayName = "Deck items should have names when loaded")]
  public async Task LoadDecks_DeckItemsHasNames()
  {
    _dependensies.ContextFactory.Populate([.. MTGCardDeckDTOMocker.MockList(3)]);
    var vm = new DeckSelectionPageViewModel(_dependensies.Repository, _dependensies.Importer);

    await vm.RefreshDecksCommand.ExecuteAsync(null);

    foreach (var item in vm.DeckItems)
      Assert.IsFalse(string.IsNullOrEmpty(item.Title), "Deck should have a name");
  }

  [TestMethod(DisplayName = "Deck items should have image URI when loaded if deck has an image")]
  public async Task LoadDecks_DeckItemsHasImageUris()
  {
    _dependensies.ContextFactory.Populate([.. MTGCardDeckDTOMocker.MockList(3)]);

    var vm = new DeckSelectionPageViewModel(_dependensies.Repository, _dependensies.Importer);

    await vm.RefreshDecksCommand.ExecuteAsync(null);

    foreach (var item in vm.DeckItems)
      Assert.IsFalse(string.IsNullOrEmpty(item.ImageUri), "Deck should have an image URI");
  }

  [TestMethod(DisplayName = "ViewModel should be busy when loading decks")]
  public async Task LoadDecks_IsBusy()
  {
    _dependensies.ContextFactory.Populate([.. MTGCardDeckDTOMocker.MockList(3)]);

    _dependensies.Importer.Delay = TimeSpan.FromMilliseconds(50);

    var vm = new DeckSelectionPageViewModel(_dependensies.Repository, _dependensies.Importer);

    await WorkerAssert.IsBusy(vm.Worker, () => vm.RefreshDecksCommand.ExecuteAsync(null));
  }
}

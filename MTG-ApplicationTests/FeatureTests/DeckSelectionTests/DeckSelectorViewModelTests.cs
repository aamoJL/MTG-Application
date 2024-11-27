using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckSelection.ViewModels;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.ViewModel;

namespace MTGApplicationTests.FeatureTests.DeckSelectionTests;
[TestClass]
public class DeckSelectionViewModelTests
{
  private readonly DeckRepositoryDependencies _dependensies = new();

  [TestMethod("Deck items should be populated when decks has been loaded")]
  public async Task LoadDecks_DeckItemsPopulated()
  {
    var itemCount = 3;
    _dependensies.ContextFactory.Populate(MTGCardDeckDTOMocker.MockList(itemCount).ToArray());
    var vm = new DeckSelectionViewModel(_dependensies.Repository, _dependensies.Importer);

    await vm.LoadDecksCommand.ExecuteAsync(null);

    Assert.AreEqual(itemCount, vm.DeckItems.Count, "Item counts does not match");
  }

  [TestMethod("Deck items should have names when loaded")]
  public async Task LoadDecks_DeckItemsHasNames()
  {
    _dependensies.ContextFactory.Populate(MTGCardDeckDTOMocker.MockList(3).ToArray());
    var vm = new DeckSelectionViewModel(_dependensies.Repository, _dependensies.Importer);

    await vm.LoadDecksCommand.ExecuteAsync(null);

    foreach (var item in vm.DeckItems)
      Assert.IsTrue(!string.IsNullOrEmpty(item.Title), "Deck should have a name");
  }

  [TestMethod("Deck items should have image URI when loaded if deck has an image")]
  public async Task LoadDecks_DeckItemsHasImageUris()
  {
    _dependensies.ContextFactory.Populate(MTGCardDeckDTOMocker.MockList(3).ToArray());

    var vm = new DeckSelectionViewModel(_dependensies.Repository, _dependensies.Importer);

    await vm.LoadDecksCommand.ExecuteAsync(null);

    foreach (var item in vm.DeckItems)
      Assert.IsTrue(!string.IsNullOrEmpty(item.ImageUri), "Deck should have an image URI");
  }

  [TestMethod("ViewModel should be busy when loading decks")]
  public async Task LoadDecks_IsBusy()
  {
    _dependensies.ContextFactory.Populate(MTGCardDeckDTOMocker.MockList(3).ToArray());
    var vm = new DeckSelectionViewModel(_dependensies.Repository, _dependensies.Importer);

    await WorkerAssert.IsBusy(vm, () => vm.LoadDecksCommand.ExecuteAsync(null));
  }
}

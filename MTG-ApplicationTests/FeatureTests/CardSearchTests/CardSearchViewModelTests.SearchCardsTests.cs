using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardSearch;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.CardSearchTests;

public partial class CardSearchViewModelTests
{
  [TestClass]
  public class SearchCardsTests
  {
    private readonly DeckRepositoryDependencies _dependensies = new();

    [TestMethod("Cards should be found with valid query")]
    public async Task SearchCards_WithValidQuery_CardsFound()
    {
      var query = "Black Lotus";
      _dependensies.Importer.ExpectedCards = [new CardImportResult.Card(MTGCardInfoMocker.MockInfo(name: query))];

      var search = new CardSearchViewModel(_dependensies.Importer);

      await search.SubmitSearchCommand.ExecuteAsync(query);

      Assert.IsTrue(search.Cards.TotalCardCount > 0, "Cards were not found");
    }

    [TestMethod("Cards should not be found with empty query")]
    public async Task SearchCards_WithEmptyQuery_CardsNotFound()
    {
      var query = string.Empty;
      _dependensies.Importer.ExpectedCards = [new CardImportResult.Card(MTGCardInfoMocker.MockInfo(name: query))];
      var search = new CardSearchViewModel(_dependensies.Importer);

      await search.SubmitSearchCommand.ExecuteAsync(query);

      Assert.AreEqual(0, search.Cards.TotalCardCount, "Cards should not have been found.");
    }

    [TestMethod("Card list should not be empty if cards have been found and loaded")]
    public async Task SearchCards_WithValidQuery_CardsAreLoaded()
    {
      var query = "Black Lotus";
      _dependensies.Importer.ExpectedCards = [new CardImportResult.Card(MTGCardInfoMocker.MockInfo(name: query))];
      var search = new CardSearchViewModel(_dependensies.Importer);

      await search.SubmitSearchCommand.ExecuteAsync(query);
      await search.Cards.Collection.LoadMoreItemsAsync((uint)search.Cards.TotalCardCount);

      Assert.IsTrue(search.Cards.Collection.Count > 0, "Collection should not be empty");
      Assert.AreEqual(search.Cards.Collection.Count, search.Cards.TotalCardCount);
    }

    [TestMethod("Worker should be busy when searching cards")]
    public async Task SearchCards_WithValidQuery_IsWorking()
    {
      var query = "Black Lotus";
      var search = new CardSearchViewModel(_dependensies.Importer);

      await WorkerAssert.IsBusy(search, () => search.SubmitSearchCommand.ExecuteAsync(query));
    }
  }
}

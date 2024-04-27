using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.API.CardAPI;
using MTGApplication.Features.CardSearch;
using MTGApplication.General.Models.Card;

namespace MTGApplicationTests.Features.CardSearch;
[TestClass]
public class CardSearchTests
{
  private readonly ICardAPI<MTGCard> CardAPI = new ScryfallAPI(); // TODO: mock

  [TestMethod("Cards should be found with valid query")]
  public async Task SearchCards_WithValidQuery_CardsFound()
  {
    var query = "asd";
    var search = new MTGCardSearchViewModel(CardAPI);

    await search.SubmitSearchCommand.ExecuteAsync(query);

    Assert.IsTrue(search.Cards.TotalCardCount > 0, "Cards were not found");
  }

  [TestMethod("Cards should not be found with empty query")]
  public async Task SearchCards_WithEmptyQuery_CardsNotFound()
  {
    var query = string.Empty;
    var search = new MTGCardSearchViewModel(CardAPI);

    await search.SubmitSearchCommand.ExecuteAsync(query);

    Assert.AreEqual(0, search.Cards.TotalCardCount, "Cards should not have been found.");
  }

  [TestMethod("Card list should not be empty if cards have been found")]
  public async Task SearchCards_WithValidQuery_CardsAreLoaded()
  {
    var query = "asd";
    var search = new MTGCardSearchViewModel(CardAPI);

    await search.SubmitSearchCommand.ExecuteAsync(query);
    await search.Cards.Collection.LoadMoreItemsAsync((uint)search.Cards.TotalCardCount);

    Assert.AreEqual(search.Cards.Collection.Count, search.Cards.TotalCardCount);
  }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.IntegrationTests.APITests.ScryfallAPITests;
public partial class ScryfallAPITests
{
  [TestClass]
  public class FetchFromDTOsTests
  {
    [TestMethod]
    public async Task Fetch_WithValidDTOs_CardsFound()
    {
      var api = new ScryfallAPI();
      var cards = new MTGCard[]
      {
        MTGCardModelMocker.CreateMTGCardModel(name: "Against All Odds", scryfallId: Guid.Parse("3cd8dd4e-6892-49d7-8fae-97d04f9f6c84")),
        MTGCardModelMocker.CreateMTGCardModel(name: "Annex Sentry", scryfallId: Guid.Parse("04baad61-1b51-4602-9e33-0de4a9f34793")),
        MTGCardModelMocker.CreateMTGCardModel(name: "Apostle of Invasion", scryfallId: Guid.Parse("8a973487-5def-4771-bb77-5748cbd2f469")),
      };

      var result = await api.FetchFromDTOs(cards.Select(x => new MTGCardDTO(x)).ToArray());

      CollectionAssert.AreEquivalent(
        cards.Select(x => x.Info.Name).ToArray(),
        result.Found.Select(x => x.Info.Name).ToArray());
    }
  }
}

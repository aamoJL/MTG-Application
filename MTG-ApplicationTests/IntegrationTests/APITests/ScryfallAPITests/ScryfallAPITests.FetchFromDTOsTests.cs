using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.IntegrationTests.APITests.ScryfallAPITests;
public partial class ScryfallAPITests
{
  [TestClass]
  public class ImportFromDTOsTests
  {
    [TestMethod]
    public async Task Import_WithValidDTOs_CardsFound()
    {
      var api = new ScryfallAPI();
      var dtos = new MTGCardDTO[]
      {
        MTGCardDTOMocker.Mock(name: "Against All Odds", scryfallId: Guid.Parse("3cd8dd4e-6892-49d7-8fae-97d04f9f6c84")),
        MTGCardDTOMocker.Mock(name: "Annex Sentry", scryfallId: Guid.Parse("04baad61-1b51-4602-9e33-0de4a9f34793")),
        MTGCardDTOMocker.Mock(name: "Apostle of Invasion", scryfallId: Guid.Parse("8a973487-5def-4771-bb77-5748cbd2f469")),
      };

      var result = await api.ImportWithDTOs(dtos);

      CollectionAssert.AreEquivalent(
        dtos.Select(x => x.Name).ToArray(),
        result.Found.Select(x => x.Info.Name).ToArray());
    }
  }
}

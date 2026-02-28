using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.Features.CardCollectionEditor.Models.Converters;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Models.Converters;

[TestClass]
public class CollectionListToDTOConverterTests
{
  [TestMethod]
  public void Convert()
  {
    var cards = MTGCardMocker.Mock(4);
    var list = new MTGCardCollectionList()
    {
      Name = "List",
      SearchQuery = "Query",
      Cards = [.. cards]
    };

    var actual = CollectionListToDTOConverter.Convert(list);
    var expected = new MTGCardCollectionListDTO(
      name: "List",
      searchQuery: "Query",
      cards: [.. cards.Select(x => new MTGCardDTO(x.Info))]);

    Assert.AreEqual(expected, actual);
  }
}

using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.Features.CardCollectionEditor.Models.Converters;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Models.Converters;

[TestClass]
public class CardCollectionToDTOConverterTests
{
  [TestMethod]
  public void Convert()
  {
    var collection = new MTGCardCollection()
    {
      Name = "Collection",
      CollectionLists = [new MTGCardCollectionList() { Name = "List" }]
    };

    var actual = CardCollectionToDTOConverter.Convert(collection);
    var expected = new MTGCardCollectionDTO(name: "Collection", collectionLists: [
      new MTGCardCollectionListDTO(name: "List", searchQuery: string.Empty, cards: [])
      ]);

    Assert.AreEqual(expected, actual);
  }
}
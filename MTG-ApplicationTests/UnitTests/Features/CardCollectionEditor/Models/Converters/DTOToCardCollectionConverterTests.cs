using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.Features.CardCollectionEditor.Models.Converters;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Importers;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Models.Converters;

[TestClass]
public class DTOToCardCollectionConverterTests
{
  [TestMethod]
  public async Task Convert()
  {
    var dto = new MTGCardCollectionDTO(name: "Collection", collectionLists: [
      new MTGCardCollectionListDTO(name: "List", searchQuery: string.Empty, cards: [])
      ]);

    var actual = await new DTOToCardCollectionConverter(new TestMTGCardImporter()
    {
      Result = CardImportResult.Empty()
    }).Convert(dto);
    var expected = new MTGCardCollection()
    {
      CollectionLists = [new MTGCardCollectionList() { Name = "List" }],
      Name = "Collection"
    };

    Assert.AreEqual(expected.Name, actual.Name);
    CollectionAssert.AreEquivalent(
      expected.CollectionLists.Select(x => x.Name).ToList(),
      actual.CollectionLists.Select(x => x.Name).ToList());
  }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class GetDeckNamesTests
{
  [TestMethod]
  public async Task Execute_ReturnsNames()
  {
    var factory = new Database.TestCardDbContextFactory();
    var repository = new TestDeckDTORepository(factory);
    var decks = MTGCardDeckDTOMocker.MockList(3);

    factory.Populate(decks);

    var result = await new GetDeckNames(repository).Execute();

    CollectionAssert.AreEqual(decks.Select(x => x.Name).ToArray(), result);
  }
}

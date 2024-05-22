using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.UseCaseTests;

[TestClass]
public class GetDeckNamesTests
{
  [TestMethod]
  public async Task Execute_ReturnsNames()
  {
    var factory = new TestCardDbContextFactory();
    var repository = new TestDeckDTORepository(factory);
    var decks = MTGCardDeckDTOMocker.MockList(3);

    factory.Populate(decks);

    var result = await new GetDeckNames(repository).Execute();

    CollectionAssert.AreEqual(decks.Select(x => x.Name).ToArray(), result);
  }
}

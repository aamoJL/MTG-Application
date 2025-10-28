using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.CardCollectionList.ViewModels;

public partial class CardCollectionListViewModelTests
{
  [TestClass]
  public class Init : CardCollectionListViewModelTestBase
  {
    [TestMethod]
    public async Task CardsUpdated()
    {
      _dependencies.Importer.ExpectedCards = [
        new(MTGCardInfoMocker.MockInfo(name: "1")),
        new(MTGCardInfoMocker.MockInfo(name: "2")),
        new(MTGCardInfoMocker.MockInfo(name: "3"))
        ];

      var mock = await new Mocker(_dependencies) { Model = _savedList }.MockVM();

      CollectionAssert.AreEquivalent(
        _dependencies.Importer.ExpectedCards.Select(x => x.Info.Name).ToArray(),
        mock.Cards.Select(x => x.Info.Name).ToArray());
    }
  }
}

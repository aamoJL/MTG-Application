using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionListViewModelTests
{
  [TestClass]
  public class InitTests : CardCollectionListViewModelTestsBase
  {
    [TestMethod]
    public async Task CardsUpdated()
    {
      _dependencies.Importer.ExpectedCards = [
        new(MTGCardInfoMocker.MockInfo(name: "1")),
        new(MTGCardInfoMocker.MockInfo(name: "2")),
        new(MTGCardInfoMocker.MockInfo(name: "3"))
        ];

      CollectionAssert.AreEquivalent(
        _dependencies.Importer.ExpectedCards.Select(x => x.Info.Name).ToArray(),
        (await new Mocker(_dependencies) { Model = _savedList }.MockVM()).Cards.Select(x => x.Info.Name).ToArray());
    }
  }
}

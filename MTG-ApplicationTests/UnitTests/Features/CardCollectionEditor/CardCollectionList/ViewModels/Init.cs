namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.CardCollectionList.ViewModels;

public partial class CardCollectionListViewModelTests
{
  [TestClass]
  public class Init : CardCollectionListViewModelTestBase
  {
    [TestMethod]
    public async Task CardsUpdated()
    {
      var mock = await new Mocker(_dependencies) { Model = _savedList }.MockVM();

      CollectionAssert.AreEquivalent(
        _savedList.Cards.Select(x => x.Info.Name).ToArray(),
        mock.Cards.Select(x => x.Info.Name).ToArray());
    }
  }
}

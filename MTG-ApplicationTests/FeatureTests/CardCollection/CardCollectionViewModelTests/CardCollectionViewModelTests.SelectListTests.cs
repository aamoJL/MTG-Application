using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class SelectListTests : CardCollectionViewModelTestsBase
  {
    [TestMethod]
    public async Task SelectList_ListSelected()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection
      }.MockVM();

      await viewmodel.SelectListCommand.ExecuteAsync(_savedCollection.CollectionLists[1].Name);

      Assert.AreSame(_savedCollection.CollectionLists[1], viewmodel.SelectedList);
    }

    [TestMethod]
    public async Task SelectList_QueryCardsUpdated()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection
      }.MockVM();

      viewmodel.SelectedList = viewmodel.Collection.CollectionLists[0];
      await viewmodel.QueryCardsViewModel.Collection.LoadMoreItemsAsync(10);

      Assert.AreEqual(0, viewmodel.QueryCardsViewModel.Collection.Count);

      _dependencies.CardAPI.ExpectedCards = [.. _savedCollection.CollectionLists[1].Cards];

      await viewmodel.SelectListCommand.ExecuteAsync(viewmodel.Collection.CollectionLists[1].Name);
      await viewmodel.QueryCardsViewModel.Collection.LoadMoreItemsAsync(10);

      Assert.AreEqual(_dependencies.CardAPI.ExpectedCards.Length, viewmodel.QueryCardsViewModel.Collection.Count);
    }
  }
}

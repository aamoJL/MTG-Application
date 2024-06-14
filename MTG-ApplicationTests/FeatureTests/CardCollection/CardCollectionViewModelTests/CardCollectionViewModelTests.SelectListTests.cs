using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardCollection;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class SelectListTests : CardCollectionViewModelTestsBase, ICanExecuteWithParameterCommandTests
  {
    [TestMethod("Should be able to execute if the parameter is null OR collection has the list and the selected list is not the given list")]
    public void ValidParameter_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection
      }.MockVM();
      viewmodel.SelectedList = _savedCollection.CollectionLists.First();

      Assert.IsTrue(viewmodel.SelectListCommand.CanExecute(null));
      Assert.IsTrue(viewmodel.SelectListCommand.CanExecute(viewmodel.Collection.CollectionLists[1]));
    }

    [TestMethod("Should not be able to execute if the given parameter is not null and collection does not have the list OR the list is same as the selected list")]
    public void InvalidParameter_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection
      }.MockVM();
      viewmodel.SelectedList = _savedCollection.CollectionLists.First();

      Assert.IsFalse(viewmodel.SelectListCommand.CanExecute(new MTGCardCollectionList()));
      Assert.IsFalse(viewmodel.SelectListCommand.CanExecute(viewmodel.Collection.CollectionLists.First()));
    }

    [TestMethod]
    public async Task SelectList_ListSelected()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection
      }.MockVM();

      await viewmodel.SelectListCommand.ExecuteAsync(_savedCollection.CollectionLists[1]);

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

      _dependencies.CardAPI.ExpectedCards = [.. _savedCollection.CollectionLists[1].Cards.Select(x => new CardImportResult.Card(x.Info))];

      await viewmodel.SelectListCommand.ExecuteAsync(viewmodel.Collection.CollectionLists[1]);
      await viewmodel.QueryCardsViewModel.Collection.LoadMoreItemsAsync(10);

      Assert.AreEqual(_dependencies.CardAPI.ExpectedCards.Length, viewmodel.QueryCardsViewModel.Collection.Count);
    }
  }
}

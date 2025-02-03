using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionListViewModelTests
{
  [TestClass]
  public class SwitchCardOwnershipTests : CardCollectionListViewModelTestsBase
  {
    [TestMethod("Should be able to execute if the given card is not null")]
    public async Task ValidParameter_CanExecute()
    {
      var card = new CardCollectionMTGCard(MTGCardInfoMocker.MockInfo());
      var viewmodel = await new Mocker(_dependencies)
      {
        Model = _savedList
      }.MockVM();

      Assert.IsTrue(viewmodel.SwitchCardOwnershipCommand.CanExecute(card));
    }

    [TestMethod("Should not be able to execute if the given card is null")]
    public async Task InvalidParameter_CanNotExecute()
    {
      var viewmodel = await new Mocker(_dependencies).MockVM();

      Assert.IsFalse(viewmodel.SwitchCardOwnershipCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task SwitchOwnership_Null_NoChanges()
    {
      var viewmodel = await new Mocker(_dependencies).MockVM();

      viewmodel.SwitchCardOwnershipCommand.Execute(null);

      Assert.AreEqual(0, viewmodel.CollectionList.Cards.Count);
    }

    [TestMethod]
    public async Task SwitchOwnership_New_CardAdded()
    {
      var card = new CardCollectionMTGCard(MTGCardInfoMocker.MockInfo());
      var viewmodel = await new Mocker(_dependencies).MockVM();

      viewmodel.SwitchCardOwnershipCommand.Execute(card);

      Assert.AreEqual(1, viewmodel.CollectionList.Cards.Count);
    }

    [TestMethod]
    public async Task SwitchOwnership_Existing_CardRemoved()
    {
      var card = new CardCollectionMTGCard(MTGCardInfoMocker.MockInfo());
      var viewmodel = await new Mocker(_dependencies)
      {
        Model = new() { Cards = [card] }
      }.MockVM();

      viewmodel.SwitchCardOwnershipCommand.Execute(card);

      Assert.AreEqual(0, viewmodel.CollectionList.Cards.Count);
    }
  }
}

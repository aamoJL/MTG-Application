using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.FeatureTests.CardCollection.CardCollectionViewModelTests;
public partial class CardCollectionViewModelTests
{
  [TestClass]
  public class SwitchCardOwnershipTests : CardCollectionViewModelTestsBase, ICanExecuteCommandTests, ICanExecuteWithParameterCommandTests
  {
    [TestMethod("Should be able to execute if a list is selected")]
    public void ValidState_CanExecute()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new() { Cards = [] }] }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      Assert.IsTrue(viewmodel.SwitchCardOwnershipCommand.CanExecute(card));
    }

    [TestMethod("Should not be able to execute if a list is not selected")]
    public void InvalidState_CanNotExecute()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new() { Cards = [] }] }
      }.MockVM();

      Assert.IsFalse(viewmodel.SwitchCardOwnershipCommand.CanExecute(card));
    }

    [TestMethod("Should be able to execute if the given card is not null")]
    public void ValidParameter_CanExecute()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      Assert.IsTrue(viewmodel.SwitchCardOwnershipCommand.CanExecute(card));
    }

    [TestMethod("Should not be able to execute if the given card is null")]
    public void InvalidParameter_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new() { Cards = [] }] }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      Assert.IsFalse(viewmodel.SwitchCardOwnershipCommand.CanExecute(null));
    }

    [TestMethod]
    public void SwitchOwnership_Null_NoChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new() { Cards = [] }] }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      viewmodel.SwitchCardOwnershipCommand.Execute(null);

      Assert.AreEqual(0, viewmodel.SelectedList.Cards.Count);
    }

    [TestMethod]
    public void SwitchOwnership_New_CardAdded()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new() { Cards = [] }] }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      viewmodel.SwitchCardOwnershipCommand.Execute(card);

      Assert.AreEqual(1, viewmodel.SelectedList.Cards.Count);
    }

    [TestMethod]
    public void SwitchOwnership_Existing_CardRemoved()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new() { Cards = [card] }] }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      viewmodel.SwitchCardOwnershipCommand.Execute(card);

      Assert.AreEqual(0, viewmodel.SelectedList.Cards.Count);
    }

    [TestMethod]
    public void SwitchOwnership_Failure_NoUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = _savedCollection
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      viewmodel.SwitchCardOwnershipCommand.Execute(null);

      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public void SwitchOwnership_Success_HasUnsavedChanges()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new Mocker(_dependencies)
      {
        Collection = new() { CollectionLists = [new() { Cards = [card] }] }
      }.MockVM();
      viewmodel.SelectedList = viewmodel.Collection.CollectionLists.First();

      viewmodel.SwitchCardOwnershipCommand.Execute(card);

      Assert.IsTrue(viewmodel.HasUnsavedChanges);
    }
  }
}

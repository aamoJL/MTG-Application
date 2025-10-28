namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Editor.ViewModels;

[TestClass]
public class Init : CardCollectionEditorViewModelTestBase
{
  [TestMethod]
  public void HasUnsavedChanges()
  {
    Assert.IsFalse(new Mocker(_dependencies).MockVM().HasUnsavedChanges);
    Assert.IsTrue(new Mocker(_dependencies) { HasUnsavedChanges = true }.MockVM().HasUnsavedChanges);
  }

  [TestMethod]
  public void Collection()
  {
    Assert.AreEqual(string.Empty, new Mocker(_dependencies).MockVM().CollectionName);
    Assert.AreEqual(_savedCollection, new Mocker(_dependencies).MockVM(_savedCollection).Collection);
  }

  [TestMethod]
  public void SelectedList()
  {
    Assert.AreEqual(string.Empty, new Mocker(_dependencies).MockVM().SelectedCardCollectionListViewModel.Name);
    Assert.AreEqual(_savedCollection.CollectionLists.First(), new Mocker(_dependencies).MockVM(_savedCollection).SelectedCardCollectionListViewModel.CollectionList);
  }
}
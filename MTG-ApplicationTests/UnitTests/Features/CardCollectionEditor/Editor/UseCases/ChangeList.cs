using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Editor.ViewModels;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.Editor.UseCases;

[TestClass]
public class ChangeList : CardCollectionEditorViewModelTestBase
{
  [TestMethod(DisplayName = "Should be able to execute if the list is null, or the collection contains the list and the list is not the same as the selected list")]
  public void ValidParameter_CanExecute()
  {
    var viewmodel = new Mocker(_dependencies).MockVM(_savedCollection);

    Assert.IsTrue(viewmodel.ChangeListCommand.CanExecute(_savedCollection.CollectionLists[1]));
  }

  [TestMethod(DisplayName = "Should not be able to execute if the collection does not contain the list or the list is the same as the selected list")]
  public void InvalidParameter_CanNotExecute()
  {
    var viewmodel = new Mocker(_dependencies).MockVM(_savedCollection);

    Assert.IsFalse(viewmodel.ChangeListCommand.CanExecute(null));
    Assert.IsFalse(viewmodel.ChangeListCommand.CanExecute(new MTGCardCollectionList()));
    Assert.IsFalse(viewmodel.ChangeListCommand.CanExecute(_savedCollection.CollectionLists.First()));
  }

  [TestMethod]
  public void SelectList_ListSelected()
  {
    var viewmodel = new Mocker(_dependencies).MockVM(_savedCollection);

    viewmodel.ChangeListCommand.Execute(_savedCollection.CollectionLists[1]);

    Assert.AreSame(_savedCollection.CollectionLists[1].Name, viewmodel.SelectedCardCollectionListViewModel.Name);
  }
}
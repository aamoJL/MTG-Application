using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;

namespace MTGApplicationTests.FeatureTests.CardCollectionEditor.CardCollectionEditorViewModelTests;

public partial class CardCollectionEditorViewModelTests
{
  [TestClass]
  public class ChangeListTests : CardCollectionEditorViewModelTestsBase
  {
    [TestMethod("Should be able to execute if the list is null, or the collection contains the list and the list is not the same as the selected list")]
    public void ValidParameter_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM(_savedCollection);

      Assert.IsTrue(viewmodel.ChangeListCommand.CanExecute(_savedCollection.CollectionLists[1]));
    }

    [TestMethod("Should not be able to execute if the collection does not contain the list or the list is the same as the selected list")]
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
}

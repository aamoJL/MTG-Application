using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.FeatureTests.CardCollectionEditor.CardCollectionEditorViewModelTests;

public partial class CardCollectionEditorTests
{
  [TestClass]
  public class ChangeListTests : CardCollectionEditorViewModelTestsBase, ICanExecuteWithParameterCommandAsyncTests
  {
    [TestMethod("Should be able to execute if the list is null, or the collection contains the list and the list is not the same as the selected list")]
    public async Task ValidParameter_CanExecute()
    {
      var viewmodel = await new Mocker(_dependencies).MockVM(_savedCollection);

      Assert.IsTrue(viewmodel.ChangeListCommand.CanExecute(null));
      Assert.IsTrue(viewmodel.ChangeListCommand.CanExecute(_savedCollection.CollectionLists[1]));
    }

    [TestMethod("Should not be able to execute if the collection does not contain the list or the list is the same as the selected list")]
    public async Task InvalidParameter_CanNotExecute()
    {
      var viewmodel = await new Mocker(_dependencies).MockVM(_savedCollection);

      Assert.IsFalse(viewmodel.ChangeListCommand.CanExecute(new MTGCardCollectionList()));
      Assert.IsFalse(viewmodel.ChangeListCommand.CanExecute(_savedCollection.CollectionLists.First()));
    }

    [TestMethod]
    public async Task SelectList_Null_NewListSelected()
    {
      var viewmodel = await new Mocker(_dependencies).MockVM(_savedCollection);

      await viewmodel.ChangeListCommand.ExecuteAsync(null);

      Assert.AreEqual(string.Empty, viewmodel.CardCollectionListViewModel.Name);
    }

    [TestMethod]
    public async Task SelectList_ListSelected()
    {
      var viewmodel = await new Mocker(_dependencies).MockVM(_savedCollection);

      await viewmodel.ChangeListCommand.ExecuteAsync(_savedCollection.CollectionLists[1]);

      Assert.AreSame(_savedCollection.CollectionLists[1].Name, viewmodel.CardCollectionListViewModel.Name);
    }
  }
}

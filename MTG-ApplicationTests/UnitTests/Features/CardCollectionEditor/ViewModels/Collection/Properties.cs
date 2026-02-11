using MTGApplication.Features.CardCollectionEditor.Models;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.Collection;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_CollectionName()
  {
    var model = new MTGCardCollection()
    {
      Name = "Name"
    };
    var vm = new TestCollectionViewModelFactory().Build(model);

    Assert.AreEqual(model.Name, vm.CollectionName);
  }

  [TestMethod]
  public void Change_Model_Name()
  {
    var model = new MTGCardCollection()
    {
      Name = string.Empty
    };
    var vm = new TestCollectionViewModelFactory().Build(model);

    var changed = false;
    vm.PropertyChanged += (_, _) => { changed = true; };

    model.Name = "Name";

    Assert.IsTrue(changed);
  }

  [TestMethod]
  public void Change_CollectionName_DeleteCollectionCommandCanExecuteChanged()
  {
    var model = new MTGCardCollection()
    {
      Name = string.Empty
    };
    var vm = new TestCollectionViewModelFactory().Build(model);

    var changed = false;
    vm.DeleteCollectionCommand.CanExecuteChanged += (_, _) => { changed = true; };

    model.Name = "Name";

    Assert.IsTrue(changed);
  }

  [TestMethod]
  public void Init_HasUnsavedChanges()
  {
    var model = new MTGCardCollection();
    var vm = new TestCollectionViewModelFactory().Build(model);

    Assert.IsFalse(vm.SaveStatus.HasUnsavedChanges);
  }

  [TestMethod]
  public void Init_CollectionListViewModels()
  {
    var model = new MTGCardCollection()
    {
      CollectionLists = [
        new(){ Name = "List 1"},
        new(){ Name = "List 2"},
        new(){ Name = "List 3"},
      ]
    };
    var vm = new TestCollectionViewModelFactory().Build(model);

    Assert.HasCount(3, vm.CollectionListViewModels);
  }

  [TestMethod]
  public void Init_ListViewModel()
  {
    var model = new MTGCardCollection();
    var vm = new TestCollectionViewModelFactory().Build(model);

    Assert.IsNull(vm.ListViewModel);
  }

  [TestMethod]
  public void Model_CollectionListsChanged_CollectionListViewModelsChanged()
  {
    var model = new MTGCardCollection()
    {
      CollectionLists = [
        new(){ Name = "List 1"},
        new(){ Name = "List 2"},
        new(){ Name = "List 3"},
      ]
    };
    var vm = new TestCollectionViewModelFactory().Build(model);

    model.CollectionLists.RemoveAt(0);

    Assert.HasCount(2, vm.CollectionListViewModels);

    model.CollectionLists.Add(new() { Name = "List 4" });

    Assert.HasCount(3, vm.CollectionListViewModels);
  }
}
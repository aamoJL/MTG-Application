using MTGApplication.Features.CardCollectionEditor.Models;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.EditorPage;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_CollectionName()
  {
    var factory = new TestEditorPageViewModelFactory();
    var vm = factory.Build();

    Assert.AreEqual(string.Empty, vm.CollectionName);
  }

  [TestMethod]
  public async Task CollectionViewModel_NameChanged_CollectionNameChanged()
  {
    var collection = new MTGCardCollection();
    var factory = new TestEditorPageViewModelFactory();
    var vm = factory.Build();

    var changed = false;
    vm.PropertyChanged += (_, _) => changed = true;

    await vm.ChangeCollectionCommand.ExecuteAsync(collection);

    collection.Name = "Name";

    Assert.IsTrue(changed);
  }

  [TestMethod]
  public void Init_CollectionViewModel()
  {
    var factory = new TestEditorPageViewModelFactory();
    var vm = factory.Build();

    Assert.AreEqual(string.Empty, vm.CollectionViewModel.CollectionName);
  }
}

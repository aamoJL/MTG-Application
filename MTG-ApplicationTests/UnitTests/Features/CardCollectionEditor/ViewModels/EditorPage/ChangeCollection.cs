using MTGApplication.Features.CardCollectionEditor.Models;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.EditorPage;

[TestClass]
public class ChangeCollection
{
  [TestMethod]
  public async Task Change_CollectionChanged()
  {
    var factory = new TestEditorPageViewModelFactory();
    var vm = factory.Build();

    var collection = new MTGCardCollection()
    {
      Name = "Name"
    };
    await vm.ChangeCollectionCommand.ExecuteAsync(collection);

    Assert.AreEqual("Name", vm.CollectionName);
  }

  [TestMethod]
  public async Task Change_CollectionListSelected()
  {
    var factory = new TestEditorPageViewModelFactory();
    var vm = factory.Build();

    var collection = new MTGCardCollection()
    {
      Name = "Name",
      CollectionLists = [new MTGCardCollectionList() { Name = "List" }]
    };
    await vm.ChangeCollectionCommand.ExecuteAsync(collection);

    Assert.AreEqual("List", vm.CollectionViewModel.ListViewModel.Name);
  }
}

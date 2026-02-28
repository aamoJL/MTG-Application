using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.EditorPage;

[TestClass]
public class NewCollection
{
  [TestMethod]
  public async Task New_IsNotDirty_CollectionChanged()
  {
    var collection = new MTGCardCollection()
    {
      Name = "Name"
    };
    var factory = new TestEditorPageViewModelFactory();
    var vm = factory.Build();

    await vm.ChangeCollectionCommand.ExecuteAsync(collection);

    Assert.AreEqual("Name", vm.CollectionName);

    vm.CollectionViewModel.SaveStatus.HasUnsavedChanges = false;

    await vm.NewCollectionCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.CollectionName);
  }

  [TestMethod]
  public async Task New_IsDirty_CancelSave_Return()
  {
    var collection = new MTGCardCollection()
    {
      Name = "Name"
    };
    var factory = new TestEditorPageViewModelFactory()
    {
      EditorPageConfirmers = new()
      {
        CollectionConfirmers = new()
        {
          ConfirmUnsavedChanges = _ => Task.FromResult(ConfirmationResult.Cancel)
        }
      }
    };
    var vm = factory.Build();

    await vm.ChangeCollectionCommand.ExecuteAsync(collection);

    Assert.AreEqual("Name", vm.CollectionName);

    vm.CollectionViewModel.SaveStatus.HasUnsavedChanges = true;

    await vm.NewCollectionCommand.ExecuteAsync(null);

    Assert.AreEqual("Name", vm.CollectionName);
  }

  [TestMethod]
  public async Task New_IsDirty_DeclineSave_CollectionChanged()
  {
    var collection = new MTGCardCollection()
    {
      Name = "Name"
    };
    var factory = new TestEditorPageViewModelFactory()
    {
      EditorPageConfirmers = new()
      {
        CollectionConfirmers = new()
        {
          ConfirmUnsavedChanges = _ => Task.FromResult(ConfirmationResult.No)
        }
      }
    };
    var vm = factory.Build();

    await vm.ChangeCollectionCommand.ExecuteAsync(collection);

    Assert.AreEqual("Name", vm.CollectionName);

    vm.CollectionViewModel.SaveStatus.HasUnsavedChanges = true;

    await vm.NewCollectionCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.CollectionName);
  }
}
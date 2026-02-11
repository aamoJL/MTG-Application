using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.EditorPage;

[TestClass]
public class OnCollectionDeleted
{
  [TestMethod]
  public async Task Deleted_CollectionChanged()
  {
    var factory = new TestEditorPageViewModelFactory()
    {
      Repository = new()
      {
        DeleteResult = (_) => Task.FromResult(true)
      },
      EditorPageConfirmers = new()
      {
        CollectionConfirmers = new()
        {
          ConfirmCollectionDelete = (_) => Task.FromResult(ConfirmationResult.Yes)
        },
      },
    };
    var vm = factory.Build();

    var collection = new MTGCardCollection()
    {
      Name = "Name"
    };
    await vm.ChangeCollectionCommand.ExecuteAsync(collection);

    await vm.CollectionViewModel.DeleteCollectionCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.CollectionName);
  }
}
namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;

[TestClass]
public class RenameGroup
{
  [TestMethod]
  public void Rename_DefaultGroup_CanNotExecute()
  {
    var factory = new TestDeckCardGroupViewModelFactory();
    var vm = factory.Build();

    Assert.IsFalse(vm.RenameGroupCommand.CanExecute(null));
  }

  [TestMethod]
  public void Rename_HasName_CanExecute()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [])
    };
    var vm = factory.Build();

    Assert.IsTrue(vm.DeleteGroupCommand.CanExecute(null));
  }

  [TestMethod]
  public async Task Rename()
  {
    var onRenameCalled = false;
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", []),
      Confirmers = new()
      {
        ConfirmRenameGroup = async _ => await Task.FromResult("Changed")
      },
      OnGroupRename = (_, _) => onRenameCalled = true
    };
    var vm = factory.Build();

    await vm.RenameGroupCommand.ExecuteAsync(null);

    Assert.IsTrue(onRenameCalled);
  }
}

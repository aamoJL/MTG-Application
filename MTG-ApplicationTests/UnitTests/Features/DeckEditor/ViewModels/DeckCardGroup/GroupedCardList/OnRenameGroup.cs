using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;

[TestClass]
public class OnRenameGroup
{
  [TestMethod]
  public async Task Rename()
  {
    var factory = new TestGroupedDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo()){Group = "1"},
        new(MTGCardInfoMocker.MockInfo()){Group = "2"},
        new(MTGCardInfoMocker.MockInfo()){Group = "3"},
        new(MTGCardInfoMocker.MockInfo()){Group = "5"},
        new(MTGCardInfoMocker.MockInfo()){Group = string.Empty},
      ],
      GroupConfirmers = new()
      {
        ConfirmRenameGroup = async _ => await Task.FromResult("4")
      }
    };
    var vm = factory.Build();

    // Rename Group '2' to Group '4'
    await vm.GroupViewModels[1].RenameGroupCommand.ExecuteAsync(null);

    Assert.HasCount(5, factory.Model);

    var expected = new string[] { "1", "3", "4", "5", string.Empty };
    var keys = vm.GroupViewModels.Select(x => x.GroupKey).ToArray();
    CollectionAssert.AreEqual(expected, keys);

    Assert.HasCount(1, vm.GroupViewModels.First(x => x.GroupKey == "4").CardViewModels);
  }

  [TestMethod]
  public async Task Rename_Undo()
  {
    var factory = new TestGroupedDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo()){Group = "1"},
        new(MTGCardInfoMocker.MockInfo()){Group = "2"},
        new(MTGCardInfoMocker.MockInfo()){Group = "3"},
        new(MTGCardInfoMocker.MockInfo()){Group = "5"},
        new(MTGCardInfoMocker.MockInfo()){Group = string.Empty},
      ],
      GroupConfirmers = new()
      {
        ConfirmRenameGroup = async _ => await Task.FromResult("4")
      }
    };
    var vm = factory.Build();

    // Rename Group '2' to Group '4'
    await vm.GroupViewModels[1].RenameGroupCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();

    Assert.HasCount(5, factory.Model);

    var expected = new string[] { "1", "2", "3", "5", string.Empty };
    var keys = vm.GroupViewModels.Select(x => x.GroupKey).ToArray();
    CollectionAssert.AreEqual(expected, keys);

    Assert.HasCount(1, vm.GroupViewModels.First(x => x.GroupKey == "2").CardViewModels);
  }

  [TestMethod]
  public async Task Rename_Redo()
  {
    var factory = new TestGroupedDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo()){Group = "1"},
        new(MTGCardInfoMocker.MockInfo()){Group = "2"},
        new(MTGCardInfoMocker.MockInfo()){Group = "3"},
        new(MTGCardInfoMocker.MockInfo()){Group = "5"},
        new(MTGCardInfoMocker.MockInfo()){Group = string.Empty},
      ],
      GroupConfirmers = new()
      {
        ConfirmRenameGroup = async _ => await Task.FromResult("4")
      }
    };
    var vm = factory.Build();

    // Rename Group '2' to Group '4'
    await vm.GroupViewModels[1].RenameGroupCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.HasCount(5, factory.Model);

    var expected = new string[] { "1", "3", "4", "5", string.Empty };
    var keys = vm.GroupViewModels.Select(x => x.GroupKey).ToArray();
    CollectionAssert.AreEqual(expected, keys);

    Assert.HasCount(1, vm.GroupViewModels.First(x => x.GroupKey == "4").CardViewModels);
  }
}

using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;

[TestClass]
public class OnDeleteGroup
{
  [TestMethod]
  public async Task Delete()
  {
    var factory = new TestGroupedDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo()){Group = "1"},
        new(MTGCardInfoMocker.MockInfo()){Group = "2"},
        new(MTGCardInfoMocker.MockInfo()){Group = "3"},
        new(MTGCardInfoMocker.MockInfo()){Group = "4"},
        new(MTGCardInfoMocker.MockInfo()){Group = string.Empty},
      ],
    };
    var vm = factory.Build();

    vm.GroupViewModels[1].DeleteGroupCommand.Execute(null);

    Assert.HasCount(4, vm.GroupViewModels);

    var expected = new string[] { "1", "3", "4", string.Empty };
    var keys = vm.GroupViewModels.Select(x => x.GroupKey).ToArray();
    CollectionAssert.AreEqual(expected, keys);
  }

  [TestMethod]
  public async Task Delete_Undo()
  {
    var factory = new TestGroupedDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo()){Group = "1"},
        new(MTGCardInfoMocker.MockInfo()){Group = "2"},
        new(MTGCardInfoMocker.MockInfo()){Group = "3"},
        new(MTGCardInfoMocker.MockInfo()){Group = "4"},
        new(MTGCardInfoMocker.MockInfo()){Group = string.Empty},
      ],
    };
    var vm = factory.Build();

    vm.GroupViewModels[1].DeleteGroupCommand.Execute(null);
    factory.UndoStack.Undo();

    Assert.HasCount(5, vm.GroupViewModels);

    var expected = new string[] { "1", "2", "3", "4", string.Empty };
    var keys = vm.GroupViewModels.Select(x => x.GroupKey).ToArray();
    CollectionAssert.AreEqual(expected, keys);
  }

  [TestMethod]
  public async Task Delete_Redo()
  {
    var factory = new TestGroupedDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo()){Group = "1"},
        new(MTGCardInfoMocker.MockInfo()){Group = "2"},
        new(MTGCardInfoMocker.MockInfo()){Group = "3"},
        new(MTGCardInfoMocker.MockInfo()){Group = "4"},
        new(MTGCardInfoMocker.MockInfo()){Group = string.Empty},
      ],
    };
    var vm = factory.Build();

    vm.GroupViewModels[1].DeleteGroupCommand.Execute(null);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.HasCount(4, vm.GroupViewModels);

    var expected = new string[] { "1", "3", "4", string.Empty };
    var keys = vm.GroupViewModels.Select(x => x.GroupKey).ToArray();
    CollectionAssert.AreEqual(expected, keys);
  }
}
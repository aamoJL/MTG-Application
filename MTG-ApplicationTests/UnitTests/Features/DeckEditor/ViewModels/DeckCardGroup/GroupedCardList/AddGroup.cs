using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;

[TestClass]
public class AddGroup
{
  [TestMethod]
  public async Task Add()
  {
    var factory = new TestGroupedDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo()){Group = "2"},
        new(MTGCardInfoMocker.MockInfo()){Group = "4"},
        new(MTGCardInfoMocker.MockInfo()){Group = "1"},
      ],
      GroupedListConfirmers = new()
      {
        ConfirmAddGroup = async _ => await Task.FromResult("3")
      }
    };
    var vm = factory.Build();

    await vm.AddGroupCommand.ExecuteAsync(null);

    Assert.HasCount(5, vm.GroupViewModels); // empty group is added by default

    var expected = new string[] { "1", "2", "3", "4", string.Empty };
    var keys = vm.GroupViewModels.Select(x => x.GroupKey).ToArray();
    CollectionAssert.AreEqual(expected, keys);
  }

  [TestMethod]
  public async Task Add_Undo()
  {
    var factory = new TestGroupedDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo()){Group = "2"},
        new(MTGCardInfoMocker.MockInfo()){Group = "4"},
        new(MTGCardInfoMocker.MockInfo()){Group = "1"},
      ],
      GroupedListConfirmers = new()
      {
        ConfirmAddGroup = async _ => await Task.FromResult("3")
      }
    };
    var vm = factory.Build();

    await vm.AddGroupCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();

    Assert.HasCount(4, vm.GroupViewModels); // empty group is added by default

    var expected = new string[] { "1", "2", "4", string.Empty };
    var keys = vm.GroupViewModels.Select(x => x.GroupKey).ToArray();
    CollectionAssert.AreEqual(expected, keys);
  }

  [TestMethod]
  public async Task Add_Redo()
  {
    var factory = new TestGroupedDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo()){Group = "2"},
        new(MTGCardInfoMocker.MockInfo()){Group = "4"},
        new(MTGCardInfoMocker.MockInfo()){Group = "1"},
      ],
      GroupedListConfirmers = new()
      {
        ConfirmAddGroup = async _ => await Task.FromResult("3")
      }
    };
    var vm = factory.Build();

    await vm.AddGroupCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.HasCount(5, vm.GroupViewModels); // empty group is added by default

    var expected = new string[] { "1", "2", "3", "4", string.Empty };
    var keys = vm.GroupViewModels.Select(x => x.GroupKey).ToArray();
    CollectionAssert.AreEqual(expected, keys);
  }

  [TestMethod]
  public async Task Add_EmptyKey_Cancelled()
  {
    var factory = new TestGroupedDeckCardListViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      GroupedListConfirmers = new()
      {
        ConfirmAddGroup = async _ => await Task.FromResult(string.Empty)
      }
    };
    var vm = factory.Build();

    Assert.HasCount(1, vm.GroupViewModels);

    await vm.AddGroupCommand.ExecuteAsync(null);

    Assert.HasCount(1, vm.GroupViewModels);
  }

  [TestMethod]
  public async Task Add_Exists_NotificationShown()
  {
    var factory = new TestGroupedDeckCardListViewModelFactory()
    {
      Model = [new(MTGCardInfoMocker.MockInfo()) { Group = "1" }],
      Notifier = new() { ThrowOnError = false },
      GroupedListConfirmers = new()
      {
        ConfirmAddGroup = async _ => await Task.FromResult("1")
      }
    };
    var vm = factory.Build();

    await vm.AddGroupCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Add_Exception_NotificationShown()
  {
    var factory = new TestGroupedDeckCardListViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      GroupedListConfirmers = new()
      {
        ConfirmAddGroup = async _ => throw new()
      }
    };
    var vm = factory.Build();

    await vm.AddGroupCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}

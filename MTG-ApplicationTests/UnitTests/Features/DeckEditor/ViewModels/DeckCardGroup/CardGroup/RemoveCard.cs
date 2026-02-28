using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;

[TestClass]
public class RemoveCard
{
  [TestMethod]
  public void Remove()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "Name");
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new(string.Empty, [
        new DeckEditorMTGCard(info) { Count = 3, Group = string.Empty },
      ]),
    };
    var vm = factory.Build();

    vm.RemoveCardCommand.Execute(new DeckEditorMTGCard(info));

    Assert.HasCount(0, vm.CardViewModels);
  }

  [TestMethod]
  public void Remove_Undo()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "Name");
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new(string.Empty, [
        new DeckEditorMTGCard(info) { Count = 3, Group = string.Empty },
      ]),
    };
    var vm = factory.Build();

    vm.RemoveCardCommand.Execute(new DeckEditorMTGCard(info));
    factory.UndoStack.Undo();

    Assert.HasCount(1, vm.CardViewModels);
  }

  [TestMethod]
  public void Remove_Redo()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "Name");
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new(string.Empty, [
        new DeckEditorMTGCard(info) { Count = 3, Group = string.Empty },
      ]),
    };
    var vm = factory.Build();

    vm.RemoveCardCommand.Execute(new DeckEditorMTGCard(info));
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.HasCount(0, vm.CardViewModels);
  }

  [TestMethod]
  public void Remove_Exception_NotificationShown()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", []),
      Notifier = new() { ThrowOnError = false }
    };
    var vm = factory.Build();

    vm.RemoveCardCommand.Execute(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}

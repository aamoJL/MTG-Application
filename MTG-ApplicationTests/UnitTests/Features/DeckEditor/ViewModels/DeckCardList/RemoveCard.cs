using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardList;

[TestClass]
public class RemoveCard
{
  [TestMethod]
  public void Remove()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info)]
    };
    var vm = factory.Build();

    vm.RemoveCardCommand.Execute(new(info));

    Assert.HasCount(0, vm.CardViewModels);
  }

  [TestMethod]
  public void Remove_Undo()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new DeckEditorMTGCard(info) { Count = 3, Group = "Group", CardTag = CardTag.Add }]
    };
    var vm = factory.Build();

    vm.RemoveCardCommand.Execute(new(info));
    factory.UndoStack.UndoCommand.Execute(null);

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(info, vm.CardViewModels.First().Info);
    Assert.AreEqual(3, vm.CardViewModels.First().Count);
    Assert.AreEqual("Group", vm.CardViewModels.First().Group);
    Assert.AreEqual(CardTag.Add, vm.CardViewModels.First().CardTag);
  }

  [TestMethod]
  public void Remove_Redo()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new DeckEditorMTGCard(info) { Count = 3, Group = "Group", CardTag = CardTag.Add }]
    };
    var vm = factory.Build();

    vm.RemoveCardCommand.Execute(new(info));
    factory.UndoStack.UndoCommand.Execute(null);
    factory.UndoStack.RedoCommand.Execute(null);

    Assert.HasCount(0, vm.CardViewModels);
  }

  [TestMethod]
  public void Remove_Exception_NotificationShown()
  {
    var factory = new TestDeckCardListViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false }
    };
    var vm = factory.Build();

    vm.RemoveCardCommand.Execute(null);

    NotificationAssert.NotificationSent(NotificationType.Error, factory.Notifier);
  }
}

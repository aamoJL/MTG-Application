using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;

[TestClass]
public class ExecuteMove
{
  [TestMethod]
  public void Execute()
  {
    var factory = new TestDeckCardGroupViewModelFactory();
    var vm = factory.Build();

    Assert.HasCount(0, factory.UndoStack.ActiveCombinedCommand.Commands);

    factory.UndoStack.ActiveCombinedCommand.Commands.Add(
      new ReversibleCollectionCommand<DeckEditorMTGCard>(new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo()) { Count = 1, Group = "Group" })
      {
        ReversibleAction = new ReversibleAddCardsToGroupSourceAction(factory.Model)
      });

    Assert.HasCount(1, factory.UndoStack.ActiveCombinedCommand.Commands);
    Assert.HasCount(0, factory.Model.Cards);

    vm.ExecuteMoveCommand.Execute(null);

    Assert.HasCount(0, factory.UndoStack.ActiveCombinedCommand.Commands);
    Assert.HasCount(1, factory.Model.Cards);
  }
}

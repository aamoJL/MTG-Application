using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardList;

[TestClass]
public class ExecuteMove
{
  [TestMethod]
  public void Execute()
  {
    var factory = new TestDeckCardListViewModelFactory();
    var vm = factory.Build();

    Assert.HasCount(0, factory.UndoStack.ActiveCombinedCommand.Commands);

    factory.UndoStack.ActiveCombinedCommand.Commands.Add(
      new ReversibleCollectionCommand<DeckEditorMTGCard>(new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo()))
      {
        ReversibleAction = new ReversibleAddCardsAction(factory.Model)
      });

    Assert.HasCount(1, factory.UndoStack.ActiveCombinedCommand.Commands);
    Assert.HasCount(0, factory.Model);

    vm.ExecuteMoveCommand.Execute(null);

    Assert.HasCount(0, factory.UndoStack.ActiveCombinedCommand.Commands);
    Assert.HasCount(1, factory.Model);
  }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplicationTests.GeneralTests.Services.ReversibleCommandServiceTests;
[TestClass]
public class ReversibleCommandStackTests
{
  public class TestReversibleCommand : IReversibleCommand
  {
    public Action OnExecute { get; set; } = () => { };
    public Action OnUndo { get; set; } = () => { };

    public void Execute() => OnExecute?.Invoke();
    public void Undo() => OnUndo?.Invoke();
  }

  [TestMethod]
  public void CanUndo()
  {
    var stack = new ReversibleCommandStack();

    Assert.IsFalse(stack.CanUndo);

    stack.PushAndExecute(new TestReversibleCommand());

    Assert.IsTrue(stack.CanUndo);
  }

  [TestMethod]
  public void CanRedo()
  {
    var stack = new ReversibleCommandStack();

    Assert.IsFalse(stack.CanRedo);

    stack.PushAndExecute(new TestReversibleCommand());

    Assert.IsFalse(stack.CanRedo);

    stack.Undo();

    Assert.IsTrue(stack.CanRedo);
  }

  [TestMethod]
  public void Undo()
  {
    var stack = new ReversibleCommandStack();
    var undoInvoked = false;

    stack.PushAndExecute(new TestReversibleCommand() { OnUndo = () => { undoInvoked = true; } });

    stack.Undo();

    Assert.IsTrue(undoInvoked);
  }

  [TestMethod]
  public void Redo()
  {
    var stack = new ReversibleCommandStack();
    var command = new TestReversibleCommand();
    var redoInvoked = false;

    stack.PushAndExecute(command);

    command.OnExecute = () => { redoInvoked = true; };

    stack.Undo();
    stack.Redo();

    Assert.IsTrue(redoInvoked);
  }

  [TestMethod]
  public void PushAndExecute()
  {
    var stack = new ReversibleCommandStack();
    var executeInvoked = false;
    var command = new TestReversibleCommand()
    {
      OnExecute = () => { executeInvoked = true; }
    };

    stack.PushAndExecute(command);

    Assert.IsTrue(executeInvoked);
  }

  [TestMethod("Redo stack should be cleared when pushing new command to the undo stack")]
  public void PushAndExecute_RedoStackCleared()
  {
    var stack = new ReversibleCommandStack();

    stack.PushAndExecute(new TestReversibleCommand());
    stack.PushAndExecute(new TestReversibleCommand());
    stack.Undo();

    Assert.IsTrue(stack.CanRedo);

    stack.PushAndExecute(new TestReversibleCommand());

    Assert.IsFalse(stack.CanRedo);
  }

  [TestMethod]
  public void PushAndExecuteActiveCombinedCommand()
  {
    var stack = new ReversibleCommandStack();

    var firstCommandExecuted = false;
    var firstCommand = new TestReversibleCommand()
    {
      OnExecute = () => { firstCommandExecuted = true; }
    };
    var secondCommandExecuted = false;
    var secondCommand = new TestReversibleCommand()
    {
      OnExecute = () => { secondCommandExecuted = true; }
    };

    stack.ActiveCombinedCommand.Commands.Add(firstCommand);
    stack.ActiveCombinedCommand.Commands.Add(secondCommand);

    stack.PushAndExecuteActiveCombinedCommand();

    Assert.IsTrue(firstCommandExecuted);
    Assert.IsTrue(secondCommandExecuted);
  }

  [TestMethod]
  public void PushAndExecuteActiveCombinedCommand_Undo()
  {
    var stack = new ReversibleCommandStack();

    var firstUndoInvoked = false;
    var firstCommand = new TestReversibleCommand()
    {
      OnUndo = () => { firstUndoInvoked = true; }
    };
    var secondUndoInvoked = false;
    var secondCommand = new TestReversibleCommand()
    {
      OnUndo = () => { secondUndoInvoked = true; }
    };

    stack.ActiveCombinedCommand.Commands.Add(firstCommand);
    stack.ActiveCombinedCommand.Commands.Add(secondCommand);

    stack.PushAndExecuteActiveCombinedCommand();
    stack.Undo();

    Assert.IsTrue(firstUndoInvoked);
    Assert.IsTrue(secondUndoInvoked);
  }

  [TestMethod]
  public void PushAndExecuteActiveCombinedCommand_Redo()
  {
    var stack = new ReversibleCommandStack();

    var firstRedoInvoked = false;
    var firstCommand = new TestReversibleCommand();
    var secondRedoInvoked = false;
    var secondCommand = new TestReversibleCommand();

    stack.ActiveCombinedCommand.Commands.Add(firstCommand);
    stack.ActiveCombinedCommand.Commands.Add(secondCommand);

    stack.PushAndExecuteActiveCombinedCommand();
    stack.Undo();

    firstCommand.OnExecute = () => { firstRedoInvoked = true; };
    secondCommand.OnExecute = () => { secondRedoInvoked = true; };

    stack.Redo();

    Assert.IsTrue(firstRedoInvoked);
    Assert.IsTrue(secondRedoInvoked);
  }

  [TestMethod]
  public void Clear()
  {
    var stack = new ReversibleCommandStack();

    stack.ActiveCombinedCommand.Commands.Add(new TestReversibleCommand());
    stack.PushAndExecute(new TestReversibleCommand());
    stack.PushAndExecute(new TestReversibleCommand());
    stack.PushAndExecute(new TestReversibleCommand());
    stack.PushAndExecute(new TestReversibleCommand());
    stack.Undo();
    stack.Undo();

    stack.Clear();

    Assert.IsFalse(stack.CanUndo);
    Assert.IsFalse(stack.CanRedo);
    Assert.AreEqual(0, stack.ActiveCombinedCommand.Commands.Count);
  }
}

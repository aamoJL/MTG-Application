using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCommanders;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCommanders;

public class TestCommanderViewModelFactory
{
  public DeckEditorMTGDeck Model { get; set; } = new();
  public TestDeckEditorDependencies Dependencies { get; set; } = new();
  public ReversibleCommandStack UndoStack { get; set; } = new();

  public CommanderViewModel Build()
  {
    return new(Model)
    {
      EditorDependencies = Dependencies,
      UndoStack = UndoStack,
    };
  }
}

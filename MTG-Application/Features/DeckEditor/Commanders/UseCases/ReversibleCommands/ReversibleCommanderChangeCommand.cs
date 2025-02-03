using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor.Commanders.UseCases.ReversibleCommands;

public class ReversibleCommanderChangeCommand(DeckEditorMTGCard? newCard, DeckEditorMTGCard? oldCard) : IReversibleCommand<DeckEditorMTGCard?>
{
  public required ReversibleAction<DeckEditorMTGCard?> ReversibleAction { get; set; }

  public DeckEditorMTGCard? NewCard { get; } = newCard;
  public DeckEditorMTGCard? OldCard { get; } = oldCard;

  public void Execute() => ReversibleAction?.Action?.Invoke(NewCard);
  public void Undo() => ReversibleAction?.ReverseAction?.Invoke(OldCard);
}


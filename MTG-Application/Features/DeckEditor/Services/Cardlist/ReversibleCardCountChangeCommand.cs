using MTGApplication.General.Models.Card;
using MTGApplication.General.Services;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor;

public class ReversibleCardCountChangeCommand(MTGCard card, int oldValue, int newValue, IClassCopier<MTGCard> copier) : IReversibleCommand<(MTGCard Card, int Count)>
{
  public ReversibleAction<(MTGCard Card, int Count)> ReversibleAction { get; set; }
  public MTGCard Card { get; } = copier.Copy(card);
  public int OldValue { get; } = oldValue;
  public int NewValue { get; } = newValue;
  public IClassCopier<MTGCard> Copier { get; } = copier;

  public void Execute() => ReversibleAction?.Action?.Invoke((Copier.Copy(Card), NewValue));
  public void Undo() => ReversibleAction?.ReverseAction?.Invoke((Copier.Copy(Card), OldValue));
}

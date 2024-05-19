using MTGApplication.General.Models.Card;
using MTGApplication.General.Services;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor;

public class ReversibleCommanderChangeCommand : IReversibleCommand<MTGCard>
{
  public ReversibleCommanderChangeCommand(MTGCard newCard, MTGCard oldCard, IClassCopier<MTGCard> copier)
  {
    Copier = copier;
    NewCard = CopyOrDefault(newCard);
    OldCard = CopyOrDefault(oldCard);
  }

  public ReversibleAction<MTGCard> ReversibleAction { get; set; }

  public MTGCard NewCard { get; }
  public MTGCard OldCard { get; }
  private IClassCopier<MTGCard> Copier { get; }

  public void Execute() => ReversibleAction?.Action?.Invoke(CopyOrDefault(NewCard));
  public void Undo() => ReversibleAction?.ReverseAction?.Invoke(CopyOrDefault(OldCard));

  private MTGCard CopyOrDefault(MTGCard card) => card != null ? Copier.Copy(card) : null;
}


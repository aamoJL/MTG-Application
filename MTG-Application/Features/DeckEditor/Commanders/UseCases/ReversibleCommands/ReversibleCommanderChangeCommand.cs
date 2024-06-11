using MTGApplication.General.Models.Card;
using MTGApplication.General.Services;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor.Services.Commanders;

public class ReversibleCommanderChangeCommand : IReversibleCommand<DeckEditorMTGCard>
{
  public ReversibleCommanderChangeCommand(DeckEditorMTGCard newCard, DeckEditorMTGCard oldCard, IClassCopier<DeckEditorMTGCard> copier)
  {
    Copier = copier;
    NewCard = CopyOrDefault(newCard);
    OldCard = CopyOrDefault(oldCard);
  }

  public ReversibleAction<DeckEditorMTGCard> ReversibleAction { get; set; }

  public DeckEditorMTGCard NewCard { get; }
  public DeckEditorMTGCard OldCard { get; }
  private IClassCopier<DeckEditorMTGCard> Copier { get; }

  public void Execute() => ReversibleAction?.Action?.Invoke(CopyOrDefault(NewCard));
  public void Undo() => ReversibleAction?.ReverseAction?.Invoke(CopyOrDefault(OldCard));

  private DeckEditorMTGCard CopyOrDefault(DeckEditorMTGCard card) => card != null ? Copier.Copy(card) : null;
}


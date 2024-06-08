using MTGApplication.General.Services;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.General.Models.Card;

public class DeckEditorMTGCardCopier : IClassCopier<DeckEditorMTGCard>
{
  public DeckEditorMTGCard Copy(DeckEditorMTGCard item) => new(item.Info, item.Count);

  public IEnumerable<DeckEditorMTGCard> Copy(IEnumerable<DeckEditorMTGCard> items)
    => items.Select(x => new DeckEditorMTGCard(x.Info with { }, x.Count));
}
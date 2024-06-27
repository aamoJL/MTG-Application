using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.Editor.Services;

public class DeckEditorMTGCardCopier : IClassCopier<DeckEditorMTGCard>
{
  public DeckEditorMTGCard Copy(DeckEditorMTGCard item) => new(item.Info, item.Count);

  public IEnumerable<DeckEditorMTGCard> Copy(IEnumerable<DeckEditorMTGCard> items)
    => items.Select(x => new DeckEditorMTGCard(x.Info, x.Count));
}
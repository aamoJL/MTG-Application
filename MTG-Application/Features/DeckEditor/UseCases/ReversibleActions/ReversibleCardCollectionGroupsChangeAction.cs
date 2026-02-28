using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public class ReversibleCardCollectionGroupsChangeAction : ReversibleAction<(IEnumerable<DeckEditorMTGCard> Cards, string Group)>
{
  private string? _oldGroup = null;

  protected override void ActionMethod((IEnumerable<DeckEditorMTGCard> Cards, string Group) param)
  {
    if (!param.Cards.Any())
      return;

    _oldGroup ??= param.Cards.First().Group;

    foreach (var item in param.Cards)
      item.Group = param.Group;
  }

  protected override void ReverseActionMethod((IEnumerable<DeckEditorMTGCard> Cards, string Group) param)
  {
    if (_oldGroup == null)
      return;

    ActionMethod(new(param.Cards, _oldGroup));
  }
}
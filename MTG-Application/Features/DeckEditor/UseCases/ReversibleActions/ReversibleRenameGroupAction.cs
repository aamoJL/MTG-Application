using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleRenameGroupAction : ReversibleAction<(DeckEditorCardGroup Group, string Key)>
  {
    private DeckEditorMTGCard[]? AffectedCards { get; set; }

    protected override void ActionMethod((DeckEditorCardGroup Group, string Key) param)
    {
      var (group, key) = param;

      if (string.IsNullOrEmpty(key) || key == group.GroupKey)
        return;

      group.GroupKey = key;

      AffectedCards ??= [.. group.Cards];

      foreach (var card in AffectedCards)
        card.Group = key;
    }

    protected override void ReverseActionMethod((DeckEditorCardGroup Group, string Key) param)
      => ActionMethod(param);
  }
}
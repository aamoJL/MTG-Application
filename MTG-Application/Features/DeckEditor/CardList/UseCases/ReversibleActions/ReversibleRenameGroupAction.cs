using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ReversibleCommandService;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleRenameGroupAction : ReversibleAction<(CardGroupViewModel Group, string Key)>
  {
    public ReversibleRenameGroupAction()
    {
      Action = ActionMethod;
      ReverseAction = ReverseActionMethod;
    }

    private DeckEditorMTGCard[]? AffectedCards { get; set; }

    protected void ActionMethod((CardGroupViewModel Group, string Key) param)
    {
      var (group, key) = param;

      if (string.IsNullOrEmpty(key) || key == group.Key)
        return;

      Rename(group, key);
    }

    protected void ReverseActionMethod((CardGroupViewModel Group, string Key) param)
      => ActionMethod(param);

    private void Rename(CardGroupViewModel group, string key)
    {
      group.Key = key;

      AffectedCards ??= [.. group.Cards];

      foreach (var card in AffectedCards)
        card.Group = key;
    }
  }
}
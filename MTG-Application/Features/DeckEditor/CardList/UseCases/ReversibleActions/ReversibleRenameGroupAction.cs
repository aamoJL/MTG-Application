using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases.ReversibleActions;

public partial class CardListViewModelReversibleActions
{
  public class ReversibleRenameGroupAction(CardGroupViewModel viewmodel) : ViewModelReversibleAction<CardGroupViewModel, string>(viewmodel)
  {
    protected override void ActionMethod(string key)
    {
      if (string.IsNullOrEmpty(key) || key == Viewmodel.Key)
        return;

      Rename(key);
    }

    protected override void ReverseActionMethod(string key)
      => ActionMethod(key);

    private void Rename(string key)
    {
      // Change key
      Viewmodel.Key = key;

      // Change item groups
      foreach (var card in Viewmodel.Items)
        card.Group = key;
    }
  }
}
using MTGApplication.Features.DeckEditor.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardList;

public partial class SideCardListViewModel(ObservableCollection<DeckEditorMTGCard> list) : DeckCardListViewModel(list)
{
  private class CardCopyFactory
  {
    // TODO: test
    public DeckEditorMTGCard Copy(DeckEditorMTGCard card)
    {
      var model = card.Copy();
      model.Group = string.Empty;
      model.CardTag = null;

      return model;
    }
  }

  private CardCopyFactory CopyFactory { get; } = new();

  // TODO: test
  protected override Task AddCard(DeckEditorMTGCard? card)
  {
    if (card != null)
      card = CopyFactory.Copy(card);

    return base.AddCard(card);
  }

  // TODO: test
  protected override Task BeginMoveTo(DeckEditorMTGCard? card)
  {
    if (card != null)
      card = CopyFactory.Copy(card);

    return base.BeginMoveTo(card);
  }
}

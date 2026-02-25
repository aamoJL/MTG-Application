using MTGApplication.Features.DeckEditor.Models;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardList;

public partial class SideCardListViewModel(ObservableCollection<DeckEditorMTGCard> list) : DeckCardListViewModel(list)
{
  protected override DeckEditorMTGCard TransformCardModel(DeckEditorMTGCard card)
  {
    var model = base.TransformCardModel(card);
    model.Group = string.Empty;
    model.CardTag = null;

    return model;
  }
}

using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckEditor.Editor.Services.Factories;
public class DeckEditorCardListFactory(DeckEditorViewModel viewmodel)
{
  public CardListViewModel CreateCardListViewModel(ObservableCollection<DeckEditorMTGCard> cards)
  {
    return new(cards: cards, importer: viewmodel.Importer)
    {
      Confirmers = viewmodel.Confirmers.CardListConfirmers,
      UndoStack = viewmodel.UndoStack,
      Worker = viewmodel,
      Notifier = viewmodel.Notifier
    };
  }

  public GroupedCardListViewModel CreateGroupedCardListViewModel(ObservableCollection<DeckEditorMTGCard> cards)
  {
    return new(cards: cards, importer: viewmodel.Importer)
    {
      Confirmers = viewmodel.Confirmers.CardListConfirmers,
      UndoStack = viewmodel.UndoStack,
      Worker = viewmodel,
      Notifier = viewmodel.Notifier,
    };
  }
}
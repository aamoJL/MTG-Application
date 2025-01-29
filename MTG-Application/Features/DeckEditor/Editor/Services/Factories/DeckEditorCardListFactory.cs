using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckEditor.Editor.Services.Factories;
public class DeckEditorCardListFactory(DeckEditorViewModel viewmodel)
{
  public CardListViewModel CreateCardListViewModel(ObservableCollection<DeckEditorMTGCard> cards)
  {
    return new(
      importer: viewmodel.Importer,
      confirmers: viewmodel.Confirmers.CardListConfirmers)
    {
      Cards = cards,
      UndoStack = viewmodel.UndoStack,
      Worker = viewmodel,
      Notifier = viewmodel.Notifier
    };
  }

  public GroupedCardListViewModel CreateGroupedCardListViewModel(ObservableCollection<DeckEditorMTGCard> cards)
  {
    return new(
      importer: viewmodel.Importer,
      confirmers: viewmodel.Confirmers.CardListConfirmers)
    {
      Cards = cards,
      UndoStack = viewmodel.UndoStack,
      Worker = viewmodel,
      Notifier = viewmodel.Notifier,
    };
  }
}
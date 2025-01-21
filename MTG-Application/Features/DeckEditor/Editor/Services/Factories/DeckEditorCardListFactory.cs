using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckEditor.Editor.Services.Factories;
internal class DeckEditorCardListFactory(DeckEditorViewModel viewmodel)
{
  public CardListViewModel CreateCardListViewModel(ObservableCollection<DeckEditorMTGCard> cards, Action onChange)
  {
    return new(
      importer: viewmodel.Importer,
      confirmers: viewmodel.Confirmers.CardListConfirmers)
    {
      Cards = cards,
      OnChange = onChange,
      UndoStack = viewmodel.UndoStack,
      Worker = viewmodel,
      Notifier = viewmodel.Notifier
    };
  }

  public GroupedCardListViewModel CreateGroupedCardListViewModel(ObservableCollection<DeckEditorMTGCard> cards, Action onChange)
  {
    return new(
      importer: viewmodel.Importer,
      confirmers: viewmodel.Confirmers.CardListConfirmers)
    {
      Cards = cards,
      OnChange = onChange,
      UndoStack = viewmodel.UndoStack,
      Worker = viewmodel,
      Notifier = viewmodel.Notifier,
    };
  }
}
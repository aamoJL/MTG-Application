using MTGApplication.Features.DeckEditor.ViewModels;

namespace MTGApplication.Features.DeckEditor.CardList.Services.Factories;

public class GroupedCardListCardGroupFactory(GroupedCardListViewModel viewmodel)
{
  public CardGroupViewModel CreateCardGroup(string key)
  {
    return new(key, viewmodel.Cards, viewmodel.Importer)
    {
      Confirmers = viewmodel.Confirmers,
      UndoStack = viewmodel.UndoStack,
      ClipboardService = viewmodel.ClipboardService,
      Worker = viewmodel.Worker,
      Notifier = viewmodel.Notifier,
    };
  }
}

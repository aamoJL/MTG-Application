using MTGApplication.Features.DeckEditor.Models;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;

public partial class GroupedDeckCardListViewModel
{
  public new class Factory : Factory<GroupedDeckCardListViewModel>
  {
    public required GroupedCardListConfirmers GroupConfirmers { protected get; init; }

    public override GroupedDeckCardListViewModel Build(ObservableCollection<DeckEditorMTGCard> list)
    {
      return new(list)
      {
        Worker = Worker,
        Importer = Importer,
        UndoStack = UndoStack,
        Notifier = Notifier,
        NetworkService = NetworkService,
        Confirmers = ListConfirmers,
        EdhrecImporter = EdhrecImporter,
        Exporter = Exporter,
        CardFilter = CardFilter,
        CardSorter = CardSorter,
        GroupedListConfirmers = GroupConfirmers,
      };
    }
  }
}
